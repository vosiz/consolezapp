namespace ConsoleZapp
{
    public static class ColorPresets
    {
        // Severity (7) - mirrors Cli.Print's existing severity colors (Config.Colorings); listed
        // first since these may be retuned over time, unlike the generated sets below

        public static readonly ColorPair Debug =
            new ColorPair(Cli.Conclr.Grayd, Cli.Conclr.DefBg);

        public static readonly ColorPair Info =
            new ColorPair(Cli.Conclr.DefFg, Cli.Conclr.DefBg);

        public static readonly ColorPair Warning =
            new ColorPair(Cli.Conclr.Yellow, Cli.Conclr.DefBg);

        public static readonly ColorPair Error =
            new ColorPair(Cli.Conclr.Red, Cli.Conclr.DefBg);

        public static readonly ColorPair Exception =
            new ColorPair(Cli.Conclr.White, Cli.Conclr.Red);

        public static readonly ColorPair Success =
            new ColorPair(Cli.Conclr.Green, Cli.Conclr.DefBg);

        public static readonly ColorPair Fail =
            new ColorPair(Cli.Conclr.Redd, Cli.Conclr.DefBg);

        // Cross combinations (240) - every distinct foreground/background pair

        public static readonly ColorPair WhiteOnGray =
            new ColorPair(Cli.Conclr.White, Cli.Conclr.Gray);

        public static readonly ColorPair WhiteOnBlack =
            new ColorPair(Cli.Conclr.White, Cli.Conclr.Black);

        public static readonly ColorPair WhiteOnRed =
            new ColorPair(Cli.Conclr.White, Cli.Conclr.Red);

        public static readonly ColorPair WhiteOnGreen =
            new ColorPair(Cli.Conclr.White, Cli.Conclr.Green);

        public static readonly ColorPair WhiteOnBlue =
            new ColorPair(Cli.Conclr.White, Cli.Conclr.Blue);

        public static readonly ColorPair WhiteOnCyan =
            new ColorPair(Cli.Conclr.White, Cli.Conclr.Cyan);

        public static readonly ColorPair WhiteOnMagenta =
            new ColorPair(Cli.Conclr.White, Cli.Conclr.Magenta);

        public static readonly ColorPair WhiteOnYellow =
            new ColorPair(Cli.Conclr.White, Cli.Conclr.Yellow);

        public static readonly ColorPair WhiteOnGrayd =
            new ColorPair(Cli.Conclr.White, Cli.Conclr.Grayd);

        public static readonly ColorPair WhiteOnRedd =
            new ColorPair(Cli.Conclr.White, Cli.Conclr.Redd);

        public static readonly ColorPair WhiteOnGreend =
            new ColorPair(Cli.Conclr.White, Cli.Conclr.Greend);

        public static readonly ColorPair WhiteOnBlued =
            new ColorPair(Cli.Conclr.White, Cli.Conclr.Blued);

        public static readonly ColorPair WhiteOnCyand =
            new ColorPair(Cli.Conclr.White, Cli.Conclr.Cyand);

        public static readonly ColorPair WhiteOnMagentad =
            new ColorPair(Cli.Conclr.White, Cli.Conclr.Magentad);

        public static readonly ColorPair WhiteOnYellowd =
            new ColorPair(Cli.Conclr.White, Cli.Conclr.Yellowd);

        public static readonly ColorPair GrayOnWhite =
            new ColorPair(Cli.Conclr.Gray, Cli.Conclr.White);

        public static readonly ColorPair GrayOnBlack =
            new ColorPair(Cli.Conclr.Gray, Cli.Conclr.Black);

        public static readonly ColorPair GrayOnRed =
            new ColorPair(Cli.Conclr.Gray, Cli.Conclr.Red);

        public static readonly ColorPair GrayOnGreen =
            new ColorPair(Cli.Conclr.Gray, Cli.Conclr.Green);

        public static readonly ColorPair GrayOnBlue =
            new ColorPair(Cli.Conclr.Gray, Cli.Conclr.Blue);

        public static readonly ColorPair GrayOnCyan =
            new ColorPair(Cli.Conclr.Gray, Cli.Conclr.Cyan);

        public static readonly ColorPair GrayOnMagenta =
            new ColorPair(Cli.Conclr.Gray, Cli.Conclr.Magenta);

        public static readonly ColorPair GrayOnYellow =
            new ColorPair(Cli.Conclr.Gray, Cli.Conclr.Yellow);

        public static readonly ColorPair GrayOnGrayd =
            new ColorPair(Cli.Conclr.Gray, Cli.Conclr.Grayd);

        public static readonly ColorPair GrayOnRedd =
            new ColorPair(Cli.Conclr.Gray, Cli.Conclr.Redd);

        public static readonly ColorPair GrayOnGreend =
            new ColorPair(Cli.Conclr.Gray, Cli.Conclr.Greend);

        public static readonly ColorPair GrayOnBlued =
            new ColorPair(Cli.Conclr.Gray, Cli.Conclr.Blued);

        public static readonly ColorPair GrayOnCyand =
            new ColorPair(Cli.Conclr.Gray, Cli.Conclr.Cyand);

        public static readonly ColorPair GrayOnMagentad =
            new ColorPair(Cli.Conclr.Gray, Cli.Conclr.Magentad);

        public static readonly ColorPair GrayOnYellowd =
            new ColorPair(Cli.Conclr.Gray, Cli.Conclr.Yellowd);

        public static readonly ColorPair BlackOnWhite =
            new ColorPair(Cli.Conclr.Black, Cli.Conclr.White);

        public static readonly ColorPair BlackOnGray =
            new ColorPair(Cli.Conclr.Black, Cli.Conclr.Gray);

        public static readonly ColorPair BlackOnRed =
            new ColorPair(Cli.Conclr.Black, Cli.Conclr.Red);

        public static readonly ColorPair BlackOnGreen =
            new ColorPair(Cli.Conclr.Black, Cli.Conclr.Green);

        public static readonly ColorPair BlackOnBlue =
            new ColorPair(Cli.Conclr.Black, Cli.Conclr.Blue);

        public static readonly ColorPair BlackOnCyan =
            new ColorPair(Cli.Conclr.Black, Cli.Conclr.Cyan);

        public static readonly ColorPair BlackOnMagenta =
            new ColorPair(Cli.Conclr.Black, Cli.Conclr.Magenta);

        public static readonly ColorPair BlackOnYellow =
            new ColorPair(Cli.Conclr.Black, Cli.Conclr.Yellow);

        public static readonly ColorPair BlackOnGrayd =
            new ColorPair(Cli.Conclr.Black, Cli.Conclr.Grayd);

        public static readonly ColorPair BlackOnRedd =
            new ColorPair(Cli.Conclr.Black, Cli.Conclr.Redd);

        public static readonly ColorPair BlackOnGreend =
            new ColorPair(Cli.Conclr.Black, Cli.Conclr.Greend);

        public static readonly ColorPair BlackOnBlued =
            new ColorPair(Cli.Conclr.Black, Cli.Conclr.Blued);

        public static readonly ColorPair BlackOnCyand =
            new ColorPair(Cli.Conclr.Black, Cli.Conclr.Cyand);

        public static readonly ColorPair BlackOnMagentad =
            new ColorPair(Cli.Conclr.Black, Cli.Conclr.Magentad);

        public static readonly ColorPair BlackOnYellowd =
            new ColorPair(Cli.Conclr.Black, Cli.Conclr.Yellowd);

        public static readonly ColorPair RedOnWhite =
            new ColorPair(Cli.Conclr.Red, Cli.Conclr.White);

        public static readonly ColorPair RedOnGray =
            new ColorPair(Cli.Conclr.Red, Cli.Conclr.Gray);

        public static readonly ColorPair RedOnBlack =
            new ColorPair(Cli.Conclr.Red, Cli.Conclr.Black);

        public static readonly ColorPair RedOnGreen =
            new ColorPair(Cli.Conclr.Red, Cli.Conclr.Green);

        public static readonly ColorPair RedOnBlue =
            new ColorPair(Cli.Conclr.Red, Cli.Conclr.Blue);

        public static readonly ColorPair RedOnCyan =
            new ColorPair(Cli.Conclr.Red, Cli.Conclr.Cyan);

        public static readonly ColorPair RedOnMagenta =
            new ColorPair(Cli.Conclr.Red, Cli.Conclr.Magenta);

        public static readonly ColorPair RedOnYellow =
            new ColorPair(Cli.Conclr.Red, Cli.Conclr.Yellow);

        public static readonly ColorPair RedOnGrayd =
            new ColorPair(Cli.Conclr.Red, Cli.Conclr.Grayd);

        public static readonly ColorPair RedOnRedd =
            new ColorPair(Cli.Conclr.Red, Cli.Conclr.Redd);

        public static readonly ColorPair RedOnGreend =
            new ColorPair(Cli.Conclr.Red, Cli.Conclr.Greend);

        public static readonly ColorPair RedOnBlued =
            new ColorPair(Cli.Conclr.Red, Cli.Conclr.Blued);

        public static readonly ColorPair RedOnCyand =
            new ColorPair(Cli.Conclr.Red, Cli.Conclr.Cyand);

        public static readonly ColorPair RedOnMagentad =
            new ColorPair(Cli.Conclr.Red, Cli.Conclr.Magentad);

        public static readonly ColorPair RedOnYellowd =
            new ColorPair(Cli.Conclr.Red, Cli.Conclr.Yellowd);

        public static readonly ColorPair GreenOnWhite =
            new ColorPair(Cli.Conclr.Green, Cli.Conclr.White);

        public static readonly ColorPair GreenOnGray =
            new ColorPair(Cli.Conclr.Green, Cli.Conclr.Gray);

        public static readonly ColorPair GreenOnBlack =
            new ColorPair(Cli.Conclr.Green, Cli.Conclr.Black);

        public static readonly ColorPair GreenOnRed =
            new ColorPair(Cli.Conclr.Green, Cli.Conclr.Red);

        public static readonly ColorPair GreenOnBlue =
            new ColorPair(Cli.Conclr.Green, Cli.Conclr.Blue);

        public static readonly ColorPair GreenOnCyan =
            new ColorPair(Cli.Conclr.Green, Cli.Conclr.Cyan);

        public static readonly ColorPair GreenOnMagenta =
            new ColorPair(Cli.Conclr.Green, Cli.Conclr.Magenta);

        public static readonly ColorPair GreenOnYellow =
            new ColorPair(Cli.Conclr.Green, Cli.Conclr.Yellow);

        public static readonly ColorPair GreenOnGrayd =
            new ColorPair(Cli.Conclr.Green, Cli.Conclr.Grayd);

        public static readonly ColorPair GreenOnRedd =
            new ColorPair(Cli.Conclr.Green, Cli.Conclr.Redd);

        public static readonly ColorPair GreenOnGreend =
            new ColorPair(Cli.Conclr.Green, Cli.Conclr.Greend);

        public static readonly ColorPair GreenOnBlued =
            new ColorPair(Cli.Conclr.Green, Cli.Conclr.Blued);

        public static readonly ColorPair GreenOnCyand =
            new ColorPair(Cli.Conclr.Green, Cli.Conclr.Cyand);

        public static readonly ColorPair GreenOnMagentad =
            new ColorPair(Cli.Conclr.Green, Cli.Conclr.Magentad);

        public static readonly ColorPair GreenOnYellowd =
            new ColorPair(Cli.Conclr.Green, Cli.Conclr.Yellowd);

        public static readonly ColorPair BlueOnWhite =
            new ColorPair(Cli.Conclr.Blue, Cli.Conclr.White);

        public static readonly ColorPair BlueOnGray =
            new ColorPair(Cli.Conclr.Blue, Cli.Conclr.Gray);

        public static readonly ColorPair BlueOnBlack =
            new ColorPair(Cli.Conclr.Blue, Cli.Conclr.Black);

        public static readonly ColorPair BlueOnRed =
            new ColorPair(Cli.Conclr.Blue, Cli.Conclr.Red);

        public static readonly ColorPair BlueOnGreen =
            new ColorPair(Cli.Conclr.Blue, Cli.Conclr.Green);

        public static readonly ColorPair BlueOnCyan =
            new ColorPair(Cli.Conclr.Blue, Cli.Conclr.Cyan);

        public static readonly ColorPair BlueOnMagenta =
            new ColorPair(Cli.Conclr.Blue, Cli.Conclr.Magenta);

        public static readonly ColorPair BlueOnYellow =
            new ColorPair(Cli.Conclr.Blue, Cli.Conclr.Yellow);

        public static readonly ColorPair BlueOnGrayd =
            new ColorPair(Cli.Conclr.Blue, Cli.Conclr.Grayd);

        public static readonly ColorPair BlueOnRedd =
            new ColorPair(Cli.Conclr.Blue, Cli.Conclr.Redd);

        public static readonly ColorPair BlueOnGreend =
            new ColorPair(Cli.Conclr.Blue, Cli.Conclr.Greend);

        public static readonly ColorPair BlueOnBlued =
            new ColorPair(Cli.Conclr.Blue, Cli.Conclr.Blued);

        public static readonly ColorPair BlueOnCyand =
            new ColorPair(Cli.Conclr.Blue, Cli.Conclr.Cyand);

        public static readonly ColorPair BlueOnMagentad =
            new ColorPair(Cli.Conclr.Blue, Cli.Conclr.Magentad);

        public static readonly ColorPair BlueOnYellowd =
            new ColorPair(Cli.Conclr.Blue, Cli.Conclr.Yellowd);

        public static readonly ColorPair CyanOnWhite =
            new ColorPair(Cli.Conclr.Cyan, Cli.Conclr.White);

        public static readonly ColorPair CyanOnGray =
            new ColorPair(Cli.Conclr.Cyan, Cli.Conclr.Gray);

        public static readonly ColorPair CyanOnBlack =
            new ColorPair(Cli.Conclr.Cyan, Cli.Conclr.Black);

        public static readonly ColorPair CyanOnRed =
            new ColorPair(Cli.Conclr.Cyan, Cli.Conclr.Red);

        public static readonly ColorPair CyanOnGreen =
            new ColorPair(Cli.Conclr.Cyan, Cli.Conclr.Green);

        public static readonly ColorPair CyanOnBlue =
            new ColorPair(Cli.Conclr.Cyan, Cli.Conclr.Blue);

        public static readonly ColorPair CyanOnMagenta =
            new ColorPair(Cli.Conclr.Cyan, Cli.Conclr.Magenta);

        public static readonly ColorPair CyanOnYellow =
            new ColorPair(Cli.Conclr.Cyan, Cli.Conclr.Yellow);

        public static readonly ColorPair CyanOnGrayd =
            new ColorPair(Cli.Conclr.Cyan, Cli.Conclr.Grayd);

        public static readonly ColorPair CyanOnRedd =
            new ColorPair(Cli.Conclr.Cyan, Cli.Conclr.Redd);

        public static readonly ColorPair CyanOnGreend =
            new ColorPair(Cli.Conclr.Cyan, Cli.Conclr.Greend);

        public static readonly ColorPair CyanOnBlued =
            new ColorPair(Cli.Conclr.Cyan, Cli.Conclr.Blued);

        public static readonly ColorPair CyanOnCyand =
            new ColorPair(Cli.Conclr.Cyan, Cli.Conclr.Cyand);

        public static readonly ColorPair CyanOnMagentad =
            new ColorPair(Cli.Conclr.Cyan, Cli.Conclr.Magentad);

        public static readonly ColorPair CyanOnYellowd =
            new ColorPair(Cli.Conclr.Cyan, Cli.Conclr.Yellowd);

        public static readonly ColorPair MagentaOnWhite =
            new ColorPair(Cli.Conclr.Magenta, Cli.Conclr.White);

        public static readonly ColorPair MagentaOnGray =
            new ColorPair(Cli.Conclr.Magenta, Cli.Conclr.Gray);

        public static readonly ColorPair MagentaOnBlack =
            new ColorPair(Cli.Conclr.Magenta, Cli.Conclr.Black);

        public static readonly ColorPair MagentaOnRed =
            new ColorPair(Cli.Conclr.Magenta, Cli.Conclr.Red);

        public static readonly ColorPair MagentaOnGreen =
            new ColorPair(Cli.Conclr.Magenta, Cli.Conclr.Green);

        public static readonly ColorPair MagentaOnBlue =
            new ColorPair(Cli.Conclr.Magenta, Cli.Conclr.Blue);

        public static readonly ColorPair MagentaOnCyan =
            new ColorPair(Cli.Conclr.Magenta, Cli.Conclr.Cyan);

        public static readonly ColorPair MagentaOnYellow =
            new ColorPair(Cli.Conclr.Magenta, Cli.Conclr.Yellow);

        public static readonly ColorPair MagentaOnGrayd =
            new ColorPair(Cli.Conclr.Magenta, Cli.Conclr.Grayd);

        public static readonly ColorPair MagentaOnRedd =
            new ColorPair(Cli.Conclr.Magenta, Cli.Conclr.Redd);

        public static readonly ColorPair MagentaOnGreend =
            new ColorPair(Cli.Conclr.Magenta, Cli.Conclr.Greend);

        public static readonly ColorPair MagentaOnBlued =
            new ColorPair(Cli.Conclr.Magenta, Cli.Conclr.Blued);

        public static readonly ColorPair MagentaOnCyand =
            new ColorPair(Cli.Conclr.Magenta, Cli.Conclr.Cyand);

        public static readonly ColorPair MagentaOnMagentad =
            new ColorPair(Cli.Conclr.Magenta, Cli.Conclr.Magentad);

        public static readonly ColorPair MagentaOnYellowd =
            new ColorPair(Cli.Conclr.Magenta, Cli.Conclr.Yellowd);

        public static readonly ColorPair YellowOnWhite =
            new ColorPair(Cli.Conclr.Yellow, Cli.Conclr.White);

        public static readonly ColorPair YellowOnGray =
            new ColorPair(Cli.Conclr.Yellow, Cli.Conclr.Gray);

        public static readonly ColorPair YellowOnBlack =
            new ColorPair(Cli.Conclr.Yellow, Cli.Conclr.Black);

        public static readonly ColorPair YellowOnRed =
            new ColorPair(Cli.Conclr.Yellow, Cli.Conclr.Red);

        public static readonly ColorPair YellowOnGreen =
            new ColorPair(Cli.Conclr.Yellow, Cli.Conclr.Green);

        public static readonly ColorPair YellowOnBlue =
            new ColorPair(Cli.Conclr.Yellow, Cli.Conclr.Blue);

        public static readonly ColorPair YellowOnCyan =
            new ColorPair(Cli.Conclr.Yellow, Cli.Conclr.Cyan);

        public static readonly ColorPair YellowOnMagenta =
            new ColorPair(Cli.Conclr.Yellow, Cli.Conclr.Magenta);

        public static readonly ColorPair YellowOnGrayd =
            new ColorPair(Cli.Conclr.Yellow, Cli.Conclr.Grayd);

        public static readonly ColorPair YellowOnRedd =
            new ColorPair(Cli.Conclr.Yellow, Cli.Conclr.Redd);

        public static readonly ColorPair YellowOnGreend =
            new ColorPair(Cli.Conclr.Yellow, Cli.Conclr.Greend);

        public static readonly ColorPair YellowOnBlued =
            new ColorPair(Cli.Conclr.Yellow, Cli.Conclr.Blued);

        public static readonly ColorPair YellowOnCyand =
            new ColorPair(Cli.Conclr.Yellow, Cli.Conclr.Cyand);

        public static readonly ColorPair YellowOnMagentad =
            new ColorPair(Cli.Conclr.Yellow, Cli.Conclr.Magentad);

        public static readonly ColorPair YellowOnYellowd =
            new ColorPair(Cli.Conclr.Yellow, Cli.Conclr.Yellowd);

        public static readonly ColorPair GraydOnWhite =
            new ColorPair(Cli.Conclr.Grayd, Cli.Conclr.White);

        public static readonly ColorPair GraydOnGray =
            new ColorPair(Cli.Conclr.Grayd, Cli.Conclr.Gray);

        public static readonly ColorPair GraydOnBlack =
            new ColorPair(Cli.Conclr.Grayd, Cli.Conclr.Black);

        public static readonly ColorPair GraydOnRed =
            new ColorPair(Cli.Conclr.Grayd, Cli.Conclr.Red);

        public static readonly ColorPair GraydOnGreen =
            new ColorPair(Cli.Conclr.Grayd, Cli.Conclr.Green);

        public static readonly ColorPair GraydOnBlue =
            new ColorPair(Cli.Conclr.Grayd, Cli.Conclr.Blue);

        public static readonly ColorPair GraydOnCyan =
            new ColorPair(Cli.Conclr.Grayd, Cli.Conclr.Cyan);

        public static readonly ColorPair GraydOnMagenta =
            new ColorPair(Cli.Conclr.Grayd, Cli.Conclr.Magenta);

        public static readonly ColorPair GraydOnYellow =
            new ColorPair(Cli.Conclr.Grayd, Cli.Conclr.Yellow);

        public static readonly ColorPair GraydOnRedd =
            new ColorPair(Cli.Conclr.Grayd, Cli.Conclr.Redd);

        public static readonly ColorPair GraydOnGreend =
            new ColorPair(Cli.Conclr.Grayd, Cli.Conclr.Greend);

        public static readonly ColorPair GraydOnBlued =
            new ColorPair(Cli.Conclr.Grayd, Cli.Conclr.Blued);

        public static readonly ColorPair GraydOnCyand =
            new ColorPair(Cli.Conclr.Grayd, Cli.Conclr.Cyand);

        public static readonly ColorPair GraydOnMagentad =
            new ColorPair(Cli.Conclr.Grayd, Cli.Conclr.Magentad);

        public static readonly ColorPair GraydOnYellowd =
            new ColorPair(Cli.Conclr.Grayd, Cli.Conclr.Yellowd);

        public static readonly ColorPair ReddOnWhite =
            new ColorPair(Cli.Conclr.Redd, Cli.Conclr.White);

        public static readonly ColorPair ReddOnGray =
            new ColorPair(Cli.Conclr.Redd, Cli.Conclr.Gray);

        public static readonly ColorPair ReddOnBlack =
            new ColorPair(Cli.Conclr.Redd, Cli.Conclr.Black);

        public static readonly ColorPair ReddOnRed =
            new ColorPair(Cli.Conclr.Redd, Cli.Conclr.Red);

        public static readonly ColorPair ReddOnGreen =
            new ColorPair(Cli.Conclr.Redd, Cli.Conclr.Green);

        public static readonly ColorPair ReddOnBlue =
            new ColorPair(Cli.Conclr.Redd, Cli.Conclr.Blue);

        public static readonly ColorPair ReddOnCyan =
            new ColorPair(Cli.Conclr.Redd, Cli.Conclr.Cyan);

        public static readonly ColorPair ReddOnMagenta =
            new ColorPair(Cli.Conclr.Redd, Cli.Conclr.Magenta);

        public static readonly ColorPair ReddOnYellow =
            new ColorPair(Cli.Conclr.Redd, Cli.Conclr.Yellow);

        public static readonly ColorPair ReddOnGrayd =
            new ColorPair(Cli.Conclr.Redd, Cli.Conclr.Grayd);

        public static readonly ColorPair ReddOnGreend =
            new ColorPair(Cli.Conclr.Redd, Cli.Conclr.Greend);

        public static readonly ColorPair ReddOnBlued =
            new ColorPair(Cli.Conclr.Redd, Cli.Conclr.Blued);

        public static readonly ColorPair ReddOnCyand =
            new ColorPair(Cli.Conclr.Redd, Cli.Conclr.Cyand);

        public static readonly ColorPair ReddOnMagentad =
            new ColorPair(Cli.Conclr.Redd, Cli.Conclr.Magentad);

        public static readonly ColorPair ReddOnYellowd =
            new ColorPair(Cli.Conclr.Redd, Cli.Conclr.Yellowd);

        public static readonly ColorPair GreendOnWhite =
            new ColorPair(Cli.Conclr.Greend, Cli.Conclr.White);

        public static readonly ColorPair GreendOnGray =
            new ColorPair(Cli.Conclr.Greend, Cli.Conclr.Gray);

        public static readonly ColorPair GreendOnBlack =
            new ColorPair(Cli.Conclr.Greend, Cli.Conclr.Black);

        public static readonly ColorPair GreendOnRed =
            new ColorPair(Cli.Conclr.Greend, Cli.Conclr.Red);

        public static readonly ColorPair GreendOnGreen =
            new ColorPair(Cli.Conclr.Greend, Cli.Conclr.Green);

        public static readonly ColorPair GreendOnBlue =
            new ColorPair(Cli.Conclr.Greend, Cli.Conclr.Blue);

        public static readonly ColorPair GreendOnCyan =
            new ColorPair(Cli.Conclr.Greend, Cli.Conclr.Cyan);

        public static readonly ColorPair GreendOnMagenta =
            new ColorPair(Cli.Conclr.Greend, Cli.Conclr.Magenta);

        public static readonly ColorPair GreendOnYellow =
            new ColorPair(Cli.Conclr.Greend, Cli.Conclr.Yellow);

        public static readonly ColorPair GreendOnGrayd =
            new ColorPair(Cli.Conclr.Greend, Cli.Conclr.Grayd);

        public static readonly ColorPair GreendOnRedd =
            new ColorPair(Cli.Conclr.Greend, Cli.Conclr.Redd);

        public static readonly ColorPair GreendOnBlued =
            new ColorPair(Cli.Conclr.Greend, Cli.Conclr.Blued);

        public static readonly ColorPair GreendOnCyand =
            new ColorPair(Cli.Conclr.Greend, Cli.Conclr.Cyand);

        public static readonly ColorPair GreendOnMagentad =
            new ColorPair(Cli.Conclr.Greend, Cli.Conclr.Magentad);

        public static readonly ColorPair GreendOnYellowd =
            new ColorPair(Cli.Conclr.Greend, Cli.Conclr.Yellowd);

        public static readonly ColorPair BluedOnWhite =
            new ColorPair(Cli.Conclr.Blued, Cli.Conclr.White);

        public static readonly ColorPair BluedOnGray =
            new ColorPair(Cli.Conclr.Blued, Cli.Conclr.Gray);

        public static readonly ColorPair BluedOnBlack =
            new ColorPair(Cli.Conclr.Blued, Cli.Conclr.Black);

        public static readonly ColorPair BluedOnRed =
            new ColorPair(Cli.Conclr.Blued, Cli.Conclr.Red);

        public static readonly ColorPair BluedOnGreen =
            new ColorPair(Cli.Conclr.Blued, Cli.Conclr.Green);

        public static readonly ColorPair BluedOnBlue =
            new ColorPair(Cli.Conclr.Blued, Cli.Conclr.Blue);

        public static readonly ColorPair BluedOnCyan =
            new ColorPair(Cli.Conclr.Blued, Cli.Conclr.Cyan);

        public static readonly ColorPair BluedOnMagenta =
            new ColorPair(Cli.Conclr.Blued, Cli.Conclr.Magenta);

        public static readonly ColorPair BluedOnYellow =
            new ColorPair(Cli.Conclr.Blued, Cli.Conclr.Yellow);

        public static readonly ColorPair BluedOnGrayd =
            new ColorPair(Cli.Conclr.Blued, Cli.Conclr.Grayd);

        public static readonly ColorPair BluedOnRedd =
            new ColorPair(Cli.Conclr.Blued, Cli.Conclr.Redd);

        public static readonly ColorPair BluedOnGreend =
            new ColorPair(Cli.Conclr.Blued, Cli.Conclr.Greend);

        public static readonly ColorPair BluedOnCyand =
            new ColorPair(Cli.Conclr.Blued, Cli.Conclr.Cyand);

        public static readonly ColorPair BluedOnMagentad =
            new ColorPair(Cli.Conclr.Blued, Cli.Conclr.Magentad);

        public static readonly ColorPair BluedOnYellowd =
            new ColorPair(Cli.Conclr.Blued, Cli.Conclr.Yellowd);

        public static readonly ColorPair CyandOnWhite =
            new ColorPair(Cli.Conclr.Cyand, Cli.Conclr.White);

        public static readonly ColorPair CyandOnGray =
            new ColorPair(Cli.Conclr.Cyand, Cli.Conclr.Gray);

        public static readonly ColorPair CyandOnBlack =
            new ColorPair(Cli.Conclr.Cyand, Cli.Conclr.Black);

        public static readonly ColorPair CyandOnRed =
            new ColorPair(Cli.Conclr.Cyand, Cli.Conclr.Red);

        public static readonly ColorPair CyandOnGreen =
            new ColorPair(Cli.Conclr.Cyand, Cli.Conclr.Green);

        public static readonly ColorPair CyandOnBlue =
            new ColorPair(Cli.Conclr.Cyand, Cli.Conclr.Blue);

        public static readonly ColorPair CyandOnCyan =
            new ColorPair(Cli.Conclr.Cyand, Cli.Conclr.Cyan);

        public static readonly ColorPair CyandOnMagenta =
            new ColorPair(Cli.Conclr.Cyand, Cli.Conclr.Magenta);

        public static readonly ColorPair CyandOnYellow =
            new ColorPair(Cli.Conclr.Cyand, Cli.Conclr.Yellow);

        public static readonly ColorPair CyandOnGrayd =
            new ColorPair(Cli.Conclr.Cyand, Cli.Conclr.Grayd);

        public static readonly ColorPair CyandOnRedd =
            new ColorPair(Cli.Conclr.Cyand, Cli.Conclr.Redd);

        public static readonly ColorPair CyandOnGreend =
            new ColorPair(Cli.Conclr.Cyand, Cli.Conclr.Greend);

        public static readonly ColorPair CyandOnBlued =
            new ColorPair(Cli.Conclr.Cyand, Cli.Conclr.Blued);

        public static readonly ColorPair CyandOnMagentad =
            new ColorPair(Cli.Conclr.Cyand, Cli.Conclr.Magentad);

        public static readonly ColorPair CyandOnYellowd =
            new ColorPair(Cli.Conclr.Cyand, Cli.Conclr.Yellowd);

        public static readonly ColorPair MagentadOnWhite =
            new ColorPair(Cli.Conclr.Magentad, Cli.Conclr.White);

        public static readonly ColorPair MagentadOnGray =
            new ColorPair(Cli.Conclr.Magentad, Cli.Conclr.Gray);

        public static readonly ColorPair MagentadOnBlack =
            new ColorPair(Cli.Conclr.Magentad, Cli.Conclr.Black);

        public static readonly ColorPair MagentadOnRed =
            new ColorPair(Cli.Conclr.Magentad, Cli.Conclr.Red);

        public static readonly ColorPair MagentadOnGreen =
            new ColorPair(Cli.Conclr.Magentad, Cli.Conclr.Green);

        public static readonly ColorPair MagentadOnBlue =
            new ColorPair(Cli.Conclr.Magentad, Cli.Conclr.Blue);

        public static readonly ColorPair MagentadOnCyan =
            new ColorPair(Cli.Conclr.Magentad, Cli.Conclr.Cyan);

        public static readonly ColorPair MagentadOnMagenta =
            new ColorPair(Cli.Conclr.Magentad, Cli.Conclr.Magenta);

        public static readonly ColorPair MagentadOnYellow =
            new ColorPair(Cli.Conclr.Magentad, Cli.Conclr.Yellow);

        public static readonly ColorPair MagentadOnGrayd =
            new ColorPair(Cli.Conclr.Magentad, Cli.Conclr.Grayd);

        public static readonly ColorPair MagentadOnRedd =
            new ColorPair(Cli.Conclr.Magentad, Cli.Conclr.Redd);

        public static readonly ColorPair MagentadOnGreend =
            new ColorPair(Cli.Conclr.Magentad, Cli.Conclr.Greend);

        public static readonly ColorPair MagentadOnBlued =
            new ColorPair(Cli.Conclr.Magentad, Cli.Conclr.Blued);

        public static readonly ColorPair MagentadOnCyand =
            new ColorPair(Cli.Conclr.Magentad, Cli.Conclr.Cyand);

        public static readonly ColorPair MagentadOnYellowd =
            new ColorPair(Cli.Conclr.Magentad, Cli.Conclr.Yellowd);

        public static readonly ColorPair YellowdOnWhite =
            new ColorPair(Cli.Conclr.Yellowd, Cli.Conclr.White);

        public static readonly ColorPair YellowdOnGray =
            new ColorPair(Cli.Conclr.Yellowd, Cli.Conclr.Gray);

        public static readonly ColorPair YellowdOnBlack =
            new ColorPair(Cli.Conclr.Yellowd, Cli.Conclr.Black);

        public static readonly ColorPair YellowdOnRed =
            new ColorPair(Cli.Conclr.Yellowd, Cli.Conclr.Red);

        public static readonly ColorPair YellowdOnGreen =
            new ColorPair(Cli.Conclr.Yellowd, Cli.Conclr.Green);

        public static readonly ColorPair YellowdOnBlue =
            new ColorPair(Cli.Conclr.Yellowd, Cli.Conclr.Blue);

        public static readonly ColorPair YellowdOnCyan =
            new ColorPair(Cli.Conclr.Yellowd, Cli.Conclr.Cyan);

        public static readonly ColorPair YellowdOnMagenta =
            new ColorPair(Cli.Conclr.Yellowd, Cli.Conclr.Magenta);

        public static readonly ColorPair YellowdOnYellow =
            new ColorPair(Cli.Conclr.Yellowd, Cli.Conclr.Yellow);

        public static readonly ColorPair YellowdOnGrayd =
            new ColorPair(Cli.Conclr.Yellowd, Cli.Conclr.Grayd);

        public static readonly ColorPair YellowdOnRedd =
            new ColorPair(Cli.Conclr.Yellowd, Cli.Conclr.Redd);

        public static readonly ColorPair YellowdOnGreend =
            new ColorPair(Cli.Conclr.Yellowd, Cli.Conclr.Greend);

        public static readonly ColorPair YellowdOnBlued =
            new ColorPair(Cli.Conclr.Yellowd, Cli.Conclr.Blued);

        public static readonly ColorPair YellowdOnCyand =
            new ColorPair(Cli.Conclr.Yellowd, Cli.Conclr.Cyand);

        public static readonly ColorPair YellowdOnMagentad =
            new ColorPair(Cli.Conclr.Yellowd, Cli.Conclr.Magentad);

        // Same-color pairs (16)

        public static readonly ColorPair FullWhite =
            new ColorPair(Cli.Conclr.White, Cli.Conclr.White);

        public static readonly ColorPair FullGray =
            new ColorPair(Cli.Conclr.Gray, Cli.Conclr.Gray);

        public static readonly ColorPair FullBlack =
            new ColorPair(Cli.Conclr.Black, Cli.Conclr.Black);

        public static readonly ColorPair FullRed =
            new ColorPair(Cli.Conclr.Red, Cli.Conclr.Red);

        public static readonly ColorPair FullGreen =
            new ColorPair(Cli.Conclr.Green, Cli.Conclr.Green);

        public static readonly ColorPair FullBlue =
            new ColorPair(Cli.Conclr.Blue, Cli.Conclr.Blue);

        public static readonly ColorPair FullCyan =
            new ColorPair(Cli.Conclr.Cyan, Cli.Conclr.Cyan);

        public static readonly ColorPair FullMagenta =
            new ColorPair(Cli.Conclr.Magenta, Cli.Conclr.Magenta);

        public static readonly ColorPair FullYellow =
            new ColorPair(Cli.Conclr.Yellow, Cli.Conclr.Yellow);

        public static readonly ColorPair FullGrayd =
            new ColorPair(Cli.Conclr.Grayd, Cli.Conclr.Grayd);

        public static readonly ColorPair FullRedd =
            new ColorPair(Cli.Conclr.Redd, Cli.Conclr.Redd);

        public static readonly ColorPair FullGreend =
            new ColorPair(Cli.Conclr.Greend, Cli.Conclr.Greend);

        public static readonly ColorPair FullBlued =
            new ColorPair(Cli.Conclr.Blued, Cli.Conclr.Blued);

        public static readonly ColorPair FullCyand =
            new ColorPair(Cli.Conclr.Cyand, Cli.Conclr.Cyand);

        public static readonly ColorPair FullMagentad =
            new ColorPair(Cli.Conclr.Magentad, Cli.Conclr.Magentad);

        public static readonly ColorPair FullYellowd =
            new ColorPair(Cli.Conclr.Yellowd, Cli.Conclr.Yellowd);

        // System default pinned (32) - one side fixed to the console's default fg/bg

        public static readonly ColorPair SystemOnWhite =
            new ColorPair(Cli.Conclr.DefaultForeground, Cli.Conclr.White);

        public static readonly ColorPair SystemOnGray =
            new ColorPair(Cli.Conclr.DefaultForeground, Cli.Conclr.Gray);

        public static readonly ColorPair SystemOnBlack =
            new ColorPair(Cli.Conclr.DefaultForeground, Cli.Conclr.Black);

        public static readonly ColorPair SystemOnRed =
            new ColorPair(Cli.Conclr.DefaultForeground, Cli.Conclr.Red);

        public static readonly ColorPair SystemOnGreen =
            new ColorPair(Cli.Conclr.DefaultForeground, Cli.Conclr.Green);

        public static readonly ColorPair SystemOnBlue =
            new ColorPair(Cli.Conclr.DefaultForeground, Cli.Conclr.Blue);

        public static readonly ColorPair SystemOnCyan =
            new ColorPair(Cli.Conclr.DefaultForeground, Cli.Conclr.Cyan);

        public static readonly ColorPair SystemOnMagenta =
            new ColorPair(Cli.Conclr.DefaultForeground, Cli.Conclr.Magenta);

        public static readonly ColorPair SystemOnYellow =
            new ColorPair(Cli.Conclr.DefaultForeground, Cli.Conclr.Yellow);

        public static readonly ColorPair SystemOnGrayd =
            new ColorPair(Cli.Conclr.DefaultForeground, Cli.Conclr.Grayd);

        public static readonly ColorPair SystemOnRedd =
            new ColorPair(Cli.Conclr.DefaultForeground, Cli.Conclr.Redd);

        public static readonly ColorPair SystemOnGreend =
            new ColorPair(Cli.Conclr.DefaultForeground, Cli.Conclr.Greend);

        public static readonly ColorPair SystemOnBlued =
            new ColorPair(Cli.Conclr.DefaultForeground, Cli.Conclr.Blued);

        public static readonly ColorPair SystemOnCyand =
            new ColorPair(Cli.Conclr.DefaultForeground, Cli.Conclr.Cyand);

        public static readonly ColorPair SystemOnMagentad =
            new ColorPair(Cli.Conclr.DefaultForeground, Cli.Conclr.Magentad);

        public static readonly ColorPair SystemOnYellowd =
            new ColorPair(Cli.Conclr.DefaultForeground, Cli.Conclr.Yellowd);

        public static readonly ColorPair WhiteOnSystem =
            new ColorPair(Cli.Conclr.White, Cli.Conclr.DefaultBackground);

        public static readonly ColorPair GrayOnSystem =
            new ColorPair(Cli.Conclr.Gray, Cli.Conclr.DefaultBackground);

        public static readonly ColorPair BlackOnSystem =
            new ColorPair(Cli.Conclr.Black, Cli.Conclr.DefaultBackground);

        public static readonly ColorPair RedOnSystem =
            new ColorPair(Cli.Conclr.Red, Cli.Conclr.DefaultBackground);

        public static readonly ColorPair GreenOnSystem =
            new ColorPair(Cli.Conclr.Green, Cli.Conclr.DefaultBackground);

        public static readonly ColorPair BlueOnSystem =
            new ColorPair(Cli.Conclr.Blue, Cli.Conclr.DefaultBackground);

        public static readonly ColorPair CyanOnSystem =
            new ColorPair(Cli.Conclr.Cyan, Cli.Conclr.DefaultBackground);

        public static readonly ColorPair MagentaOnSystem =
            new ColorPair(Cli.Conclr.Magenta, Cli.Conclr.DefaultBackground);

        public static readonly ColorPair YellowOnSystem =
            new ColorPair(Cli.Conclr.Yellow, Cli.Conclr.DefaultBackground);

        public static readonly ColorPair GraydOnSystem =
            new ColorPair(Cli.Conclr.Grayd, Cli.Conclr.DefaultBackground);

        public static readonly ColorPair ReddOnSystem =
            new ColorPair(Cli.Conclr.Redd, Cli.Conclr.DefaultBackground);

        public static readonly ColorPair GreendOnSystem =
            new ColorPair(Cli.Conclr.Greend, Cli.Conclr.DefaultBackground);

        public static readonly ColorPair BluedOnSystem =
            new ColorPair(Cli.Conclr.Blued, Cli.Conclr.DefaultBackground);

        public static readonly ColorPair CyandOnSystem =
            new ColorPair(Cli.Conclr.Cyand, Cli.Conclr.DefaultBackground);

        public static readonly ColorPair MagentadOnSystem =
            new ColorPair(Cli.Conclr.Magentad, Cli.Conclr.DefaultBackground);

        public static readonly ColorPair YellowdOnSystem =
            new ColorPair(Cli.Conclr.Yellowd, Cli.Conclr.DefaultBackground);
    }
}
