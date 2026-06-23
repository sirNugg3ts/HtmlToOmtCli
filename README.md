# sirNugg3ts.HtmlToOmtCli

A command-line wrapper around [sirNugg3ts.HtmlToOmt](https://github.com/sirNugg3ts/HtmlToOmt) that renders an HTML page offscreen and streams it as a live video source over OMT (Open Media Transport).

## Requirements

- Windows x64
- [.NET 8 Runtime](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- [Visual C++ Redistributable 2019+](https://aka.ms/vs/17/release/vc_redist.x64.exe)

## Usage

```
HtmlToOmtCli <url> [source-name] [width] [height] [fps]
```

| Argument | Required | Default | Description |
|---|---|---|---|
| `url` | ✅ | | URL to render (supports `http://`, `https://`, `file:///`) |
| `source-name` | | URL | OMT source name visible to receivers |
| `width` | | `1920` | Frame width in pixels |
| `height` | | `1080` | Frame height in pixels |
| `fps` | | `30` | Frames per second |

### Examples

```
HtmlToOmtCli http://localhost:9090/my-graphic "My Graphic"
```

```
HtmlToOmtCli http://localhost:9090/my-graphic "My Graphic" 1920 1080 60
```

```
HtmlToOmtCli "file:///C:/graphics/scoreboard.html" Scoreboard
```

Press `Ctrl+C` to stop.

## License

MIT — Copyright (c) 2026 Diogo Pascoal
