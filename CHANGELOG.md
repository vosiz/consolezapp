# Change log
## Current version
### 1.3.0 - Control base class
- rework: IControl -> abstract Control
- addition: SetWidth() with min/max clamp
- fix: ProgressBar bar-width crash on narrow render width

## History
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