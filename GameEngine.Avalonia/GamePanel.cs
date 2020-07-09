using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using GameEngine._2D;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using Bitmap = System.Drawing.Bitmap;

namespace GameEngine.UI.AvaloniaUI
{
    public class GamePanel : Panel, IGamePanel
    {
        public bool Drawing { get; private set; }
        public double ScaleX { get; private set; }
        public double ScaleY { get; private set; }

        public int WindowWidth { get; private set; }
        public int WindowHeight { get; private set; }

        private Bitmap[] buffers;
        private byte currentBuffer;
        private AvaloniaWindow window;
        Semaphore sem = new Semaphore(1, 1);

        public GamePanel(AvaloniaWindow window, int width, int height, double xScale, double yScale)
        {
            this.window = window;
            this.Name = "panel";
            Drawing = false;
            this.ScaleX = xScale;
            this.ScaleY = yScale;
            this.Width = width * xScale;
            this.Height = height * yScale;
            WindowWidth = (int)this.Width;
            WindowHeight = (int)this.Height;
            currentBuffer = 0;
            buffers = new Bitmap[] { BitmapExtensions.CreateBitmap((int)this.Width, (int)this.Height), BitmapExtensions.CreateBitmap((int)this.Width, (int)this.Height) };
        }

        public void Draw(Bitmap img, Bitmap overlay)
        {
            Drawing = true;

            Graphics gfx = Graphics.FromImage(buffers[++currentBuffer % 2]);
            gfx.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;

            gfx.DrawImage(img, 1, 1, WindowWidth, WindowHeight);
            gfx.DrawImage(overlay, 1, 1, overlay.Width, overlay.Height);
            if (window != null)
            {
                if (sem.WaitOne())
                {
                    try
                    {
                        Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
                        {
                            this.InvalidateVisual();
                            window.Renderer.Paint(new Rect(0, 0, WindowWidth, WindowHeight));
                        });
                    }
                    finally
                    {
                        sem.Release();
                        Drawing = false;
                    }
                }
            }
        }

        public void DrawHandle(object sender, View view)
        {
            GameView2D view2D = view as GameView2D;
            if (view2D != null)
            {
                Draw(view2D.Image, view2D.Overlay);
            }
        }

        public override void Render(DrawingContext context)
        {
            bool selfDraw = Drawing;

            if (selfDraw || sem.WaitOne())
            {
                try
                {
                    base.Render(context);
                    Bitmap bufferBmp = buffers[currentBuffer % 2];
                    using (MemoryStream stream = new MemoryStream())
                    {
                        bufferBmp.Save(stream, ImageFormat.Bmp);

                        stream.Position = 0;
                        Avalonia.Media.Imaging.Bitmap bmp = new Avalonia.Media.Imaging.Bitmap(stream);
                        context.DrawImage(bmp, 1, new Avalonia.Rect(0, 0, bufferBmp.Width, bufferBmp.Height), new Avalonia.Rect(0, 0, bufferBmp.Width, bufferBmp.Height));
                    }
                }
                finally
                {
                    if (!selfDraw)
                    {
                        sem.Release();
                    }
                }
            }
        }
    }
}
