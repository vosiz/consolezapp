namespace ConsoleZapp
{
    public readonly struct ColorPair
    {
        public readonly Cli.Conclr Foreground;
        public readonly Cli.Conclr Background;

        // Constructor with foreground/background
        public ColorPair(Cli.Conclr foreground, Cli.Conclr background)
        {
            Foreground = foreground;
            Background = background;
        }
    }
}
