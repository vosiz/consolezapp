using ConsoleZapp;

namespace CzappTester.Tests.Basic
{
    public static class ColorPairTests
    {
        // Constructor assigns foreground/background as given
        public static void ConstructorAssignsFields()
        {
            var pair = new ColorPair(Cli.Conclr.Magenta, Cli.Conclr.Cyand);

            Check.Equal(Cli.Conclr.Magenta, pair.Foreground);
            Check.Equal(Cli.Conclr.Cyand, pair.Background);
        }

        // ColorPresets has exactly 295 fields
        // - 240 cross + 16 diagonal + 32 system-pinned + 7 severity
        public static void PresetCountIsComplete()
        {
            var fields = typeof(ColorPresets).GetFields(
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);

            Check.Equal(295, fields.Length);
        }

        // Spot-checks one preset from each of the four generated groups
        public static void PresetSpotChecks()
        {
            Check.Equal(Cli.Conclr.White, ColorPresets.WhiteOnGreen.Foreground);
            Check.Equal(Cli.Conclr.Green, ColorPresets.WhiteOnGreen.Background);

            Check.Equal(Cli.Conclr.Red, ColorPresets.FullRed.Foreground);
            Check.Equal(Cli.Conclr.Red, ColorPresets.FullRed.Background);

            Check.Equal(Cli.Conclr.DefaultForeground, ColorPresets.SystemOnBlued.Foreground);
            Check.Equal(Cli.Conclr.Blued, ColorPresets.SystemOnBlued.Background);

            Check.Equal(Cli.Conclr.Redd, ColorPresets.SystemOnRedd.Background);
        }

        // Severity presets mirror Cli.Print's severity colors
        // - Config.Colorings
        public static void SeverityMirrorsConfigColorings()
        {
            var config_error = Cli.Config.Colorings["error"];
            Check.Equal(config_error.Foreground, ColorPresets.Error.Foreground);
            Check.Equal(config_error.Background, ColorPresets.Error.Background);

            var config_success = Cli.Config.Colorings["success"];
            Check.Equal(config_success.Foreground, ColorPresets.Success.Foreground);
            Check.Equal(config_success.Background, ColorPresets.Success.Background);
        }

        // Part(text, ColorPair) constructor assigns text and both colors
        public static void PartConstructorAssignsFields()
        {
            var part = new Part("hello", ColorPresets.WhiteOnGreen);

            Check.Equal("hello", part.Text);
            Check.Equal(Cli.Conclr.White, part.Foreground);
            Check.Equal(Cli.Conclr.Green, part.Background);
        }

        // Control.SetColor(ColorPair) matches the (fg, bg) overload
        public static void ControlSetColorAcceptsPair()
        {
            var text = new Text();
            text.SetColor(ColorPresets.FullRed);

            Check.Equal(Cli.Conclr.Red, text.Foreground);
            Check.Equal(Cli.Conclr.Red, text.Background);
        }

        // RichText.AddText/SetPartColor(ColorPair) match their (fg, bg) overloads
        public static void RichTextAcceptsPair()
        {
            var rich = new RichText();
            rich.AddText("part", ColorPresets.WhiteOnGreen, "text");

            var part = System.Linq.Enumerable.First(rich.GetParts());
            Check.Equal(Cli.Conclr.White, part.Foreground);
            Check.Equal(Cli.Conclr.Green, part.Background);

            rich.SetPartColor("part", ColorPresets.FullRed);
            part = System.Linq.Enumerable.First(rich.GetParts());
            Check.Equal(Cli.Conclr.Red, part.Foreground);
            Check.Equal(Cli.Conclr.Red, part.Background);
        }

        // Body.SetPromptColor(ColorPair)
        // - no public getter to assert, smoke-tested only
        public static void BodySetPromptColorAcceptsPair()
        {
            var body = new Body();
            body.SetPromptColor(ColorPresets.WhiteOnGreen);
        }
    }
}
