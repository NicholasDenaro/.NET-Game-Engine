using GameEngine._2D;
using System.IO;
using System.Reflection;

namespace GameEngine.UI
{
    public class GameFrame : IGameFrame
    {
        private bool started = false;

        private Rectangle bounds;
        public Rectangle Bounds => bounds;

        public double ScaleX { get; private set; }

        public double ScaleY { get; private set; }

        public IGameWindow Window { get; private set; }

        internal IGameWindowBuilder windowBuilder;

        private ISoundPlayer soundPlayer;

        public GameFrame(IGameWindowBuilder windowBuilder, int x, int y, int width, int height, int xScale, int yScale)
        {
            ScaleX = xScale;
            ScaleY = yScale;
            bounds = new Rectangle(x, y, width * xScale, height * yScale);
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

        public void SetBounds(int x, int y, int width, int height)
        {
            this.bounds.X = x;
            this.bounds.Y = y;
            this.bounds.Width = width;
            this.bounds.Height = height;
            this.Window.SetBounds(x, y, width, height);
        }

        public void PlayResource(string resource)
        {
            Stream stream = Assembly.GetEntryAssembly().GetManifestResourceStream($"{Assembly.GetEntryAssembly().GetName().Name}.{resource}");
            soundPlayer.PlayStream(stream);
        }

        public void PlayStream(Stream stream)
        {
            soundPlayer.PlayStream(stream);
        }

        public void PlaySound(ISound sound)
        {
            soundPlayer.PlaySound(sound);
        }

        public void PlayTrack(ITrack track)
        {
            soundPlayer.PlayTrack(track);
        }

        public void DrawHandle(object sender, View view)
        {
            Window?.Panel?.DrawHandle(sender, view);
        }
    }
}
