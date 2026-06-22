using System.Windows;
using CefSharp;
using CefSharp.OffScreen;
using HtmlToOmt.Contracts;
using HtmlToOmt.Services;

if (args.Length < 4)
{
    Console.Error.WriteLine("Usage: HtmlToOmtCli <url> <source-name> <width> <height> [fps]");
    Console.Error.WriteLine("  url          URL to render");
    Console.Error.WriteLine("  source-name  OMT source name");
    Console.Error.WriteLine("  width        Frame width in pixels");
    Console.Error.WriteLine("  height       Frame height in pixels");
    Console.Error.WriteLine("  fps          Frames per second (default: 30)");
    return 1;
}

var url = args[0];
var sourceName = args[1];

if (!int.TryParse(args[2], out var width) || width <= 0)
{
    Console.Error.WriteLine($"Invalid width: {args[2]}");
    return 1;
}

if (!int.TryParse(args[3], out var height) || height <= 0)
{
    Console.Error.WriteLine($"Invalid height: {args[3]}");
    return 1;
}

var fps = 30;
if (args.Length >= 5 && (!int.TryParse(args[4], out fps) || fps <= 0))
{
    Console.Error.WriteLine($"Invalid fps: {args[4]}");
    return 1;
}

var app = new Application { ShutdownMode = ShutdownMode.OnExplicitShutdown };

var cefSettings = new CefSettings
{
    WindowlessRenderingEnabled = true,
    LogSeverity = LogSeverity.Warning,
};
CefSharpSettings.SubprocessExitIfParentProcessClosed = true;
Cef.Initialize(cefSettings);

var cts = new CancellationTokenSource();
Console.CancelKeyPress += (_, e) =>
{
    e.Cancel = true;
    cts.Cancel();
};

Console.WriteLine($"Starting: {url} -> OMT source '{sourceName}' at {width}x{height} {fps}fps");

using var omtOutput = new OmtOutputService(sourceName, width, height, fps);

var renderOptions = new HtmlRenderHostOptions
{
    InitialUrl = url,
    Width = width,
    Height = height,
};

using var renderHost = new HtmlRenderHost(renderOptions);

renderHost.Diagnostic += (_, msg) => Console.WriteLine($"[diag] {msg}");
renderHost.FatalLoadError += (_, msg) =>
{
    Console.Error.WriteLine($"[error] {msg}");
    cts.Cancel();
};
renderHost.FrameReady += (_, frame) =>
{
    try
    {
        omtOutput.SendFrame(frame);
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine($"[error] SendFrame failed: {ex.Message}");
    }
};

await renderHost.StartAsync();

try
{
    await Task.Delay(Timeout.Infinite, cts.Token);
}
catch (OperationCanceledException) { }

Console.WriteLine("Shutting down.");
Cef.Shutdown();
app.Shutdown();

return 0;
