using System.Drawing;
using System.IO;
using System.Reflection;

namespace GameEngine.UI
{
    public class GameFrame : IGameFrame
    {
        private bool started = false;
        public Rectangle Bounds { get; private set; }

        public double ScaleX { get; private set; }

        public double ScaleY { get; private set; }

        public IGameWindow Window { get; private set; }

        internal IGameWindowBuilder windowBuilder;

        private ISoundPlayer soundPlayer;

        public GameFrame(IGameWindowBuilder windowBuilder, int x, int y, int width, int height, int xScale, int yScale)
        {
            ScaleX = xScale;
            ScaleY = yScale;
            Bounds = new Rectangle(x, y, width * xScale, height * yScale);
            this.windowBuilder = windowBuilder;
        }

        public void Start()
        {
            if (!started)
            {
                started = true;
                (Window, this.soundPlayer) = this.windowBuilder.Run(this);
            }
        }
        public void PlaySound(string resource)
        {
            Stream stream = Assembly.GetEntryAssembly().GetManifestResourceStream($"{Assembly.GetEntryAssembly().GetName().Name}.{resource}");

        }

        public void DrawHandle(object sender, View view)
        {
            Window?.Panel?.DrawHandle(sender, view);
        }
    }
}
