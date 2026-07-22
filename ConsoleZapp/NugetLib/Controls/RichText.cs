using System.Collections.Generic;
using System.Linq;

namespace ConsoleZapp
{
    public class RichText : Control
    {
        private readonly Dictionary<string, Part> Parts = new Dictionary<string, Part>();

        // Constructor
        public RichText() { }

        // Renders control content
        public override string Render()
        {
            return string.Concat(Parts.Values.Select(part => part.Text));
        }

        // Returns each part with its own color, in add order
        public override IEnumerable<Part> GetParts()
        {
            return Parts.Values;
        }

        // Adds or replaces a colored part under the given key
        public void AddText(string key, Cli.Conclr? fg, Cli.Conclr? bg, string fmt, params object[] args)
        {
            Parts[key] = new Part { Text = string.Format(fmt, args), Foreground = fg, Background = bg };
        }

        // Adds or replaces a plain (uncolored) part under the given key
        public void AddText(string key, string fmt, params object[] args)
        {
            AddText(key, null, null, fmt, args);
        }

        // Recolors a previously added part by key
        public void SetPartColor(string key, Cli.Conclr fg, Cli.Conclr bg)
        {
            if (!Parts.TryGetValue(key, out var part))
                return;

            part.Foreground = fg;
            part.Background = bg;
            Parts[key] = part;
        }
    }
}
