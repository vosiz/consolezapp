namespace ConsoleZapp
{
    // Controls how a Dialog's question and options are rendered - purely a rendering choice, both read the same way (typed answer + Enter, no navigation)
    public enum DialogLayout
    {
        // Question and options on a single line: "Question Label1 [answer1], Label2 [answer2]"
        Inline,

        // Question on its own line, one "answer) Label" line per option below it
        Stacked,
    }
}
