namespace ConsoleZapp
{
    // Controls what happens to Body's scroll position when new content is written while the user is reviewing retained history (PageUp/PageDown)
    public enum ScrollMode
    {
        // Reviewed position stays fixed - new content is appended but doesn't move the view, like scrolling up in a terminal
        Manual,

        // View always snaps back to the live tail on write, even mid-review - e.g. for a tail-a-log-style consumer that wants to always see the newest content
        AutoScroll,
    }
}
