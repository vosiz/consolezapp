# Change log
## Current version
### 1.4.3 - Overflow truncation
- addition: Body.WriteLine clamps to window width
- addition: ellipsis marker on truncated lines

## History
### 1.4.2 - Colored Body output
- addition: Body.WriteLine colored overload
- addition: Tui.WriteLine colored overload
### 1.4.1 - Percent precision
- addition: ProgressBar SetDecimals
- cleanup: remove CzappTester Tui tests
### 1.4.0 - Tui module
- addition: Header, Container, Tui
- addition: Body ReadCommand prompt
- addition: Body WriteLine scrolling
- fix: native scroll on Enter at bottom row
- fix: Tui.Print() clears, anchors header
### 1.3.0 - Control base class
- rework: IControl -> abstract Control
- addition: SetWidth() with min/max clamp
- fix: ProgressBar bar-width crash on narrow render width
### 1.2.1 - Controls
- addition: Text, LabeledControl controls
- addition: Progress, Percentage controls
- addition: ProgressBar control with colors
- addition: IControl.Print()
- dependency: Vosiz.UtilsLib

### 1.2.0 - Test rework
- rework: CzappTester reflection-based runner

### 1.1.0 - Print safety
- fix: Sprintf double format
- addition: PrintException
- addition: StringExt escapes
### 1.0.8 - Version cleanup
- fix: version cleanup
- docs: README methods

### 1.0.7 - Print helpers
- extension for: NewLine/Nl
- extension for: Result

### 1.0.0 - First release
- simple printing
- coloring line
- headlines