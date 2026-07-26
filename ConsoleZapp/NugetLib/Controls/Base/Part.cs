namespace ConsoleZapp
{
    public struct Part
    {
        public string Text;
        public Cli.Conclr? Foreground;
        public Cli.Conclr? Background;

        // Constructor with text and color
        public Part(string text, ColorPair pair)
        {
            Text = text;
            Foreground = pair.Foreground;
            Background = pair.Background;
        }
    }
}
