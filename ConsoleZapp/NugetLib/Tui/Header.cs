using System;

namespace ConsoleZapp
{
    public class Header
    {
        private readonly OrderedMap<Container> Containers = new OrderedMap<Container>();

        // Constructor
        public Header()
        {
            Containers.Set("main", new Container());
        }

        // Adds a new container, printed after existing ones
        // - throws if the id is already taken
        public void AddContainer(string container_id)
        {
            if (Containers.TryGetValue(container_id, out var existing))
                throw new ArgumentException($"Container ({container_id}) already exists");

            Containers.Set(container_id, new Container());
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

        // Re-renders a single control's row in place, defaults to "main" container
        public void UpdateControl(string name, string container_id = "main")
        {
            Containers[container_id].UpdateControl(name);
        }

        // Overrides border chars for a container, defaults to "main"
        public void SetBorderChars(char horizontal, char vertical, char top_left, char top_right, char bottom_left, char bottom_right, string container_id = "main")
        {
            Containers[container_id].SetBorderChars(horizontal, vertical, top_left, top_right, bottom_left, bottom_right);
        }

        // Sets the border color of the given container, defaults to "main"
        public void SetBorderColor(Cli.Conclr fg, Cli.Conclr bg, string container_id = "main")
        {
            Containers[container_id].SetBorderColor(fg, bg);
        }

        // Switches every container to plain ASCII borders
        // - fallback for consoles that can't render Unicode
        public void UseAsciiBorders()
        {
            foreach (var container in Containers.Values)
                container.SetBorderChars('-', '|', '+', '+', '+', '+');
        }

        // Returns the total printed row count, all containers
        public int GetHeight()
        {
            var height = 0;

            foreach (var container in Containers.Values)
                height += container.GetHeight();

            return height;
        }

        // Prints all containers to the console, stacked vertically
        public void Print(int width)
        {
            foreach (var container in Containers.Values)
                container.Print(width);
        }
    }
}
