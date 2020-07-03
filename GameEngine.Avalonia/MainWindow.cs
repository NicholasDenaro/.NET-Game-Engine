using Avalonia.Controls;
using System;

namespace GameEngine.UI.AvaloniaUI
{
    public class MainWindow : Window
    {
        private GamePanel panel;

        public MainWindow(GamePanel panel)
        {
            this.panel = panel;
            this.VisualChildren.Add(this.panel);
            this.CanResize = false;
            this.Closing += (e, o) => Environment.Exit(-1);
        }
    }
}
