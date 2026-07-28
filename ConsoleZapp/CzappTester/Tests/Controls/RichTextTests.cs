using System.Linq;
using ConsoleZapp;

namespace CzappTester.Tests.Controls
{
    public static class RichTextTests
    {
        public static void RendersPartsInAddOrder()
        {
            var text = new RichText();
            text.AddText("a", "First");
            text.AddText("b", "Second");
            text.AddText("c", "Third");

            Check.Equal("FirstSecondThird", text.Render());
        }

        public static void ReAddingAnExistingKeyKeepsItsOriginalPosition()
        {
            var text = new RichText();
            text.AddText("a", "First");
            text.AddText("b", "Second");
            text.AddText("c", "Third");

            text.AddText("a", "Updated");

            var order = text.GetParts().Select(part => part.Text).ToList();
            Check.Equal("Updated,Second,Third", string.Join(",", order));
        }
    }
}
