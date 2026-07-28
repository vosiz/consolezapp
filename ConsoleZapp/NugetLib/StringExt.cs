using System.Text;

namespace ConsoleZapp
{
    public static class StringExt
    {
        // Escapes braces for safe use as a string.Format template
        public static string EscapeFormat(this string text)
        {
            if (text == null)
                return text;

            return text.Replace("{", "{{").Replace("}", "}}");
        }

        // Escapes text for a JSON string literal
        public static string EscapeJson(this string text)
        {
            if (text == null)
                return text;

            var sb = new StringBuilder(text.Length);

            foreach (var c in text)
            {
                switch (c)
                {
                    case '\"': sb.Append("\\\""); break;
                    case '\\': sb.Append("\\\\"); break;
                    case '\b': sb.Append("\\b"); break;
                    case '\f': sb.Append("\\f"); break;
                    case '\n': sb.Append("\\n"); break;
                    case '\r': sb.Append("\\r"); break;
                    case '\t': sb.Append("\\t"); break;

                    default:
                        if (c < ' ')
                            sb.Append($"\\u{(int)c:x4}");
                        else
                            sb.Append(c);
                        break;
                }
            }

            return sb.ToString();
        }
    }
}
