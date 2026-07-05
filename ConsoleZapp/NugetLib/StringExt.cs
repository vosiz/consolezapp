using System.Text;

namespace ConsoleZapp
{
    static public class StringExt
    {
        // Escapes curly braces so text is safe to pass as a string.Format template with no args
        static public string EscapeFormat(this string text)
        {
            if (text == null)
                return text;

            return text.Replace("{", "{{").Replace("}", "}}");
        }

        // Escapes text for safe embedding inside a JSON string literal
        static public string EscapeJson(this string text)
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
