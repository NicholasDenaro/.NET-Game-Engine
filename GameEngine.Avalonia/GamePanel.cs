using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using GameEngine._2D;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
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

        private byte currentBuffer;
        internal AvaloniaWindow window;
        Semaphore sem = new Semaphore(1, 1);
        private MemoryStream[] mstreams;
        private RenderTargetBitmap rtb;
        private DrawingContext rtbdc;
        private Avalonia.Skia.ISkiaDrawingContextImpl skdc;
        private SkiaSharp.SKBitmap skbmp;
        private SkiaSharp.SKCanvas canvas;

        internal static GamePanel Panel { get; private set; }

        public GamePanel(AvaloniaWindow window, int width, int height, double xScale, double yScale)
        {
            Panel = this;
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

            mstreams = new MemoryStream[] { new MemoryStream(), new MemoryStream() };

            rtb = new RenderTargetBitmap(new PixelSize((int)this.Width, (int)this.Height));
            rtbdc = new DrawingContext(rtb.CreateDrawingContext(null));
            skdc = (Avalonia.Skia.ISkiaDrawingContextImpl)rtb.CreateDrawingContext(null);

            skbmp = new SkiaSharp.SKBitmap((int)this.Width, (int)this.Height);
            canvas = new SkiaSharp.SKCanvas(skbmp);
        }

        public void Draw(GameView2D view)
        {
            Drawing = true;

            skdc.Clear(Avalonia.Media.Color.FromArgb(0, 0, 0, 0));
            Drawer2DAvalonia d = view.Drawer as Drawer2DAvalonia;
            Avalonia.Media.Imaging.Bitmap b;
            Avalonia.Media.Imaging.Bitmap o;
            if (d != null)
            {
                b = d.Image(0)?.Image;
                o = d.Overlay(0)?.Image;
            }
            else
            {
                mstreams[0].Position = 0;
                (view.Drawer as Drawer2DSystemDrawing).Image(0).Image.Save(mstreams[0], ImageFormat.Png);
                mstreams[0].Position = 0;
                b = new Avalonia.Media.Imaging.Bitmap(mstreams[0]);


                mstreams[0].Position = 0;
                (view.Drawer as Drawer2DSystemDrawing).Overlay(0).Image.Save(mstreams[0], ImageFormat.Png);
                mstreams[0].Position = 0;
                o = new Avalonia.Media.Imaging.Bitmap(mstreams[0]);
            }

            if (b != null)
            {
                skdc.DrawBitmap(b.PlatformImpl, 1, new Rect(0, 0, WindowWidth, WindowHeight), new Rect(0, 0, WindowWidth * (float)ScaleX, WindowHeight * (float)ScaleY));
                skdc.DrawBitmap(o.PlatformImpl, 1, new Rect(0, 0, WindowWidth * (float)ScaleX, WindowHeight * (float)ScaleY), new Rect(0, 0, WindowWidth * (float)ScaleX, WindowHeight * (float)ScaleY));
            }

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
                        }).Wait();
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
                Draw(view2D);
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
                    MemoryStream stream = mstreams[0];
                    {
                        context.DrawImage(rtb, new Avalonia.Rect(0, 0, rtb.Size.Width - 0, rtb.Size.Height - 0), new Avalonia.Rect(-this.Bounds.X - window.PlatformImpl.DesktopScaling * ScaleX, -this.Bounds.Y - window.PlatformImpl.DesktopScaling * ScaleY, rtb.Size.Width / window.PlatformImpl.DesktopScaling + window.PlatformImpl.DesktopScaling * ScaleX, rtb.Size.Height / window.PlatformImpl.DesktopScaling + window.PlatformImpl.DesktopScaling * ScaleY));
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
