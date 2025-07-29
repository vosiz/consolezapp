using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleZapp
{
    public class Config
    {
        public class Coloring {

            public Cli.Conclr Foreground = Cli.Conclr.DefaultForeground;
            public Cli.Conclr Background = Cli.Conclr.DefaultForeground;

            public Coloring(Cli.Conclr text, Cli.Conclr back) {

                Foreground = text;
                Background = back;
            }

            public Coloring() : this(Cli.Conclr.DefFg, Cli.Conclr.DefBg) { }

        }

        public enum LineType { 
        
            General,    // corner-line-corner
            Custom,     // fully customized
        }

        public class Line {

            public static readonly int MaxWidth = 80;

            public static Line Create(char line, char corner, int width) {

                var l = new Line();
                l.LineChar = line;
                l.LineCorner = corner;
                l.LineLength = width;

                return l;
            }

            public static Line CreateCustom(string fmt, Coloring coloring = null) {

                var l = new Line();
                l.Format = fmt;
                l.Type = LineType.Custom;
                if (coloring != null)
                    l.Coloring = coloring;

                return l;
            }


            public LineType Type { get; private set; } = LineType.General;
            public Coloring Coloring { get; private set; } = new Coloring();

            public string Format { get; private set; } = "{0}";
            public string[] Args { get; set; }
                = new string[1] {
                    ""
            };

            public char LineChar { get; private set; } = '=';
            public char LineCorner { get; private set; } = '+';
            public int LineLength { get; private set; } = MaxWidth;

            public override string ToString() {

                string tostr = string.Empty;

                try
                {
                    switch (Type)
                    {
                        // general line
                        case LineType.General:
                            var line = string.Empty;
                            line += LineCorner;
                            line += string.Concat(Enumerable.Repeat(LineChar, LineLength));
                            line += LineCorner;
                            tostr += string.Format("{1}{0} {2}{0}{3}",
                                System.Environment.NewLine,
                                line,
                                Args[0],
                                line);
                            break;

                        // custom format of line
                        case LineType.Custom:
                            tostr += string.Format(Format, Args);
                            break;

                        default:
                            throw new NotImplementedException("Unimplemnted state");
                    }
                }
                catch (Exception exc)
                {

                    throw new ApplicationException("Cannot ToString", exc);
                }

                return tostr;
            }
        }


        public Dictionary<string, Coloring> Colorings { get; } 
            = new Dictionary<string, Coloring>() {

                { "debug", new Coloring(Cli.Conclr.Grayd, Cli.Conclr.DefBg) },
                { "info", new Coloring(Cli.Conclr.DefFg, Cli.Conclr.DefBg) },
                { "warning", new Coloring(Cli.Conclr.Yellow, Cli.Conclr.DefBg) },
                { "error", new Coloring(Cli.Conclr.Red, Cli.Conclr.DefBg) },
                { "exception", new Coloring(Cli.Conclr.White, Cli.Conclr.Red) },
                { "success", new Coloring(Cli.Conclr.Green, Cli.Conclr.DefBg) },
                { "fail", new Coloring(Cli.Conclr.Redd, Cli.Conclr.DefBg) },
        };

        public Dictionary<string, Line> Lining { get; }
            = new Dictionary<string, Line>() {

                { "1", Line.Create('=', '+', 50)},
                { "2", Line.Create('-', '+', 24)},
        };


        // Adds new coloring
        public void AddColoring(string key, Coloring coloring) {

            if (Colorings.ContainsKey(key))
                throw new ArgumentException($"Key ({key}) already exists");

            Colorings.Add(key, coloring);
        }

        // Add new headline
        public void AddLine(string key, Line lining) {

            if (Colorings.ContainsKey(key))
                throw new ArgumentException($"Key ({key}) already exists");

            Lining.Add(key, lining);
        }

    }
}
