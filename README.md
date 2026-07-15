# consolezapp
Console apps toolkit

## Motivation
ConsoleZapp is a personal toolkit for writing console apps in C# — a thin wrapper around `System.Console` with formatted printing, colored messages, structured lines/headlines, reusable line-based Controls (progress bars, labeled values, percentages...) and a small Tui engine for fixed-header, scrolling console screens.
Built primarily to support a Windows 7 console client project reliably, favoring classic Win32 console APIs over VT/ANSI escape codes, which aren't consistently supported on older Windows consoles.

MIT — use it however you like.

## Requirements
- .NET Standard 2.0 target — works with .NET Framework 4.6.1+, .NET Core 2.0+, .NET 5+
- Depends on [Vosiz.UtilsLib](https://www.nuget.org/packages/Vosiz.UtilsLib) (NuGet) for number/unit formatting used by some Controls

## Bug tracker
No tracked

## Features
- Formatted/colored console printing, structured lines & headlines
- Reusable line-based Controls (text, labeled values, progress, percentage, progress bar with colors)
- Tui engine — fixed header + scrolling body for full console screens

See [Roadmap](#roadmap) for full details.

## Roadmap
### Printing
- [x] Basic wrapping (console write)
- [x] Coloring
- [x] Lining
- [x] Headlines
- [x] Custom lines
- [x] Custom headlines

### Controls
- [x] Text
- [x] LabeledControl
- [x] Progress
- [x] Percentage
- [x] ProgressBar with colors

### Tui
- [x] Header, Container
- [x] Body - command prompt
- [x] Body - scrolling output
- [ ] Menu

## Test projects
The solution includes two extra projects, not published as packages, used during development:
- `CzappTester` — automated + visual tests for Printer/Controls
- `CzappTuiTester` — interactive sandbox for the Tui engine (Header/Body)

## Manual
See [Test projects](#test-projects) above for more usage examples.

### Setup
```csharp
Cli.Init(); // call this first, then configure further via Cli.Config
```

### Printing
```csharp
Cli.Print.WriteLine("Hello {0}", name);
Cli.Print.Info("Informative message");
Cli.Print.Error("Something went wrong");

Cli.Print.Line(1);
Cli.Print.Headline(1, "Title");
```
Covers formatted writes, leveled messages (Debug/Info/Warning/Error/Exception/Success/Fail/Result, plus `Custom` for your own registered coloring) and structured lines/headlines (register your own via `Cli.Config.AddLine`).

### Controls
```csharp
var progress = new Progress("items");
progress.SetTotal(100);
progress.SetCurrent(42);

Console.WriteLine(progress.Render());
```
`Text`, `LabeledControl`, `Progress`, `Percentage` and `ProgressBar` (with colors) all share the same `Render()`/`Print()` shape.

### Tui
```csharp
var header = new Header();
header.AddControl("title", new Text()).SetText("My App");

var body = new Body();
var tui = new Tui(header, body);

tui.Print();
tui.WriteLine("Hello!");
var command = tui.ReadCommand();
```
Fixed header on top, scrolling body below — see `CzappTuiTester` for a full interactive example.
