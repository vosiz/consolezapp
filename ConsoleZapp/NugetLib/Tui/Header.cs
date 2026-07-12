using System.Collections.Generic;

namespace ConsoleZapp
{
    public class Header
    {
        private readonly Dictionary<string, Container> Containers = new Dictionary<string, Container>();

        // Constructor
        public Header()
        {
            Containers["main"] = new Container();
        }

        // Adds a control to the given container, defaults to "main"
        public T AddControl<T>(string name, T control, string container_id = "main") where T : Control
        {
            return Containers[container_id].AddControl(name, control);
        }

        // Retrieves a control by name from the given container, defaults to "main"
        public Control GetControl(string name, string container_id = "main")
        {
            return Containers[container_id].GetControl(name);
        }

        // Prints all containers to the console, stacked vertically
        public void Print(int width)
        {
            foreach (var container in Containers.Values)
                container.Print(width);
        }
    }
}
