using sirNugg3ts.HtmlToOmt.Contracts;
using sirNugg3ts.HtmlToOmt.Services;

if (args.Length < 1)
{
    Console.Error.WriteLine("Usage: HtmlToOmtCli <url> [source-name] [width] [height] [fps]");
    Console.Error.WriteLine("  url          URL to render");
    Console.Error.WriteLine("  source-name  OMT source name (default: url)");
    Console.Error.WriteLine("  width        Frame width in pixels (default: 1920)");
    Console.Error.WriteLine("  height       Frame height in pixels (default: 1080)");
    Console.Error.WriteLine("  fps          Frames per second (default: 30)");
    return 1;
}

var url = args[0];
var sourceName = args.Length >= 2 ? args[1] : url;

var width = 1920;
if (args.Length >= 3 && (!int.TryParse(args[2], out width) || width <= 0))
{
    Console.Error.WriteLine($"Invalid width: {args[2]}");
    return 1;
}

var height = 1080;
if (args.Length >= 4 && (!int.TryParse(args[3], out height) || height <= 0))
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

try
{
    var cts = new CancellationTokenSource();
    Console.CancelKeyPress += (_, e) =>
    {
        e.Cancel = true;
        cts.Cancel();
    };

    Console.WriteLine($"Starting: {url} -> OMT source '{sourceName}' at {width}x{height} {fps}fps");

    var renderOptions = new HtmlRenderHostOptions
    {
        InitialUrl = url,
        Width = width,
        Height = height,
        OmtSourceName = sourceName,
        Fps = fps,
        CefCommandLineArgs = ["disable-web-security"]
    };

    var renderHost = new HtmlRenderHost(renderOptions);

    renderHost.Diagnostic += (_, msg) => Console.WriteLine($"[diag] {msg}");
    renderHost.FatalLoadError += (_, msg) =>
    {
        Console.Error.WriteLine($"[error] {msg}");
        cts.Cancel();
    };

    await renderHost.StartAsync();

    try
    {
        await Task.Delay(Timeout.Infinite, cts.Token);
    }
    catch (OperationCanceledException) { }

    Console.WriteLine("Shutting down.");
    HtmlRenderHost.Shutdown();
}
catch (Exception ex)
{
    Console.Error.WriteLine($"[fatal] {ex}");
    return 1;
}

return 0;
