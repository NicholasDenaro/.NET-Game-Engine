using GameEngine._2D;
using GameEngine.UI.Audio;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace GameEngine.UI
{
    public class GameUI : IGameUI
    {
        private bool started = false;

        private Rectangle bounds;
        public Rectangle Bounds => bounds;

        public double ScaleX { get; private set; }

        public double ScaleY { get; private set; }

        public IGameWindow Window { get; private set; }

        internal IGameWindowBuilder windowBuilder;

        public ISoundPlayer SoundPlayer { get; private set; }

        public GameUI(IGameWindowBuilder windowBuilder, int x, int y, int width, int height, int xScale, int yScale)
        {
            ScaleX = xScale;
            ScaleY = yScale;
            bounds = new Rectangle(x, y, width * xScale, height * yScale);
            this.windowBuilder = windowBuilder;
        }

        public static void OpenNewWindow(
            GameEngine engine,
            IGameWindowBuilder windowBuilder,
            int x, int y, int width, int height, int xScale, int yScale,
            IEnumerable<Controller> controllers = null,
            int state = 0)
        {
            GameUI ui = new GameUI(windowBuilder, x, y, width, height, xScale, yScale);
            engine.Draw(state) += (s, v) => ui.DrawHandle(s, v);
            ui?.Start();
            if (controllers != null)
            {
                foreach (Controller controller in controllers)
                {
                    ui?.Window.Hook(controller);
                }
            }
        }

        public void SetInitialized()
        {
            initialized = true;
        }

        private bool initialized;
        public async Task WaitInitialized()
        {
            while (!initialized)
            {
                await Task.Delay(1);
            }
        }

        internal void SetSoundPlayer(ISoundPlayer soundPlayer)
        {
            this.SoundPlayer = soundPlayer;
        }

        public void Start()
        {
            if (!started)
            {
                started = true;
                Window = this.windowBuilder.Run(this);
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

        public delegate Task CacheEvent(string name, Stream stream, int samples);

        public CacheEvent CacheImpl { get; set; }

        public async Task CacheAduio(ITrack track)
        {
            foreach (ISound sound in track.Channels())
            {
                await CacheAduio(sound);
            }
        }

        public async Task CacheAduio(ISound sound)
        {
            if (CacheImpl != null)
            {
                await CacheImpl(sound.Name, sound.GetStream(), sound.TotalSamples);
            }
        }

        public void PlayResource(Assembly assembly, string resource)
        {
            Stream stream = assembly.GetManifestResourceStream($"{assembly.GetName().Name}.{resource}");
            SoundPlayer.PlayStream(stream);
        }

        public void PlayStream(Stream stream)
        {
            SoundPlayer.PlayStream(stream);
        }

        public void PlaySound(ISound sound)
        {
            SoundPlayer.PlaySound(sound);
        }

        public void PlayTrack(ITrack track)
        {
            SoundPlayer.PlayTrack(track);
        }

        public void DrawHandle(object sender, View view)
        {
            Window?.Panel?.DrawHandle(sender, view);
        }
    }
}
