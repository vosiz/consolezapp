# consolezapp
Console apps toolkit

## Bug tracker
No tracked

## Roadmap
- [x] Basic wrapping (console write)
- [x] Coloring
- [x] Lining
- [x] Headlines
- [x] Custom lines
- [x] Custom headlines
- [ ] Menu
- [ ] Customized menu

## Manual
See testing project for more referenes

Main use: Cli.xxx - a static class

### Setup
```csharp
Cli.Init(); // this you want to call first
// you can setup config later by Cli.Config.
```

### Basic printing
```csharp
Cli.Printer.xxx // a static class reference
```

#### Basics
- Write - writes formatted string
- WriteLine - writes formatted string with new line
- Sprintf - (alias) writes formatted string
- Sprintfln - (alias) writes formatted string with new line

#### Messages
- Debug - debug level message (grey)
- Info - informative-like message
- Warning - slighly more noticible messages
- Error - red-ish messages
- Exception - more red-ish
- Success - success-liek message
- Fail - failure-like message

#### Customization
- Custom - uset to call printing with certain key registered

### Structure printing
#### Basic
- line - basically writes line with cornering
- headline - basically writes 2 lines and headline between

#### Custom structures
- line - register own lining
- headline - register own headlining
  
