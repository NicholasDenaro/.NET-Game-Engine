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
    public class GamePanel : Panel
    {
        public bool Drawing { get; private set; }
        private Bitmap[] buffers;
        private byte currentBuffer;
        private int xScale;
        private int yScale;
        private GameFrame frame;
        Semaphore sem = new Semaphore(1, 1);

        public GamePanel(GameFrame frame, int width, int height, int xScale, int yScale)
        {
            this.frame = frame;
            this.Name = "panel";
            Drawing = false;
            this.xScale = xScale;
            this.yScale = yScale;
            this.Width = width * xScale;
            this.Height = height * yScale;
            currentBuffer = 0;
            buffers = new Bitmap[] { BitmapExtensions.CreateBitmap((int)this.Width, (int)this.Height), BitmapExtensions.CreateBitmap((int)this.Width, (int)this.Height) };
        }

        public void HookMouse(EventHandler<MouseEventArgs> mouseInfo, EventHandler<MouseEventArgs> mouseDown, EventHandler<MouseEventArgs> mouseUp)
        {
            this.frame.window.PointerMoved += (s, e) => mouseInfo(s, ScaleEvent(Convert(e)));
            this.frame.window.PointerPressed += (s, e) => mouseDown(s, ScaleEvent(Convert(e)));
            this.frame.window.PointerReleased += (s, e) => mouseUp(s, ScaleEvent(Convert(e)));
        }

        public static int Key(PointerUpdateKind puk)
        {
            int key;
            switch (puk)
            {
                case PointerUpdateKind.LeftButtonPressed:
                case PointerUpdateKind.LeftButtonReleased:
                    key = 0;
                    break;

                case PointerUpdateKind.RightButtonPressed:
                case PointerUpdateKind.RightButtonReleased:
                    key = 1;
                    break;

                case PointerUpdateKind.MiddleButtonPressed:
                case PointerUpdateKind.MiddleButtonReleased:
                    key = 2;
                    break;

                case PointerUpdateKind.Other:
                    key = 6;
                    break;

                default:
                    key = -1;
                    break;
            }

            return key;
        }

        public MouseEventArgs Convert(PointerEventArgs pea)
        {
            PointerPoint pp = pea.GetCurrentPoint(frame.window);
            Avalonia.Point point = pea.GetPosition(frame.window);
            int key = Key(pp.Properties.PointerUpdateKind);

            return new MouseEventArgs(key, 1, (int)point.X, (int)point.Y, 0);
        }

        public MouseEventArgs ScaleEvent(MouseEventArgs e)
        {
            return new MouseEventArgs(e.Button, e.Clicks, e.X / xScale, e.Y / yScale, e.Wheel);
        }

        public void Draw(Bitmap img, Bitmap overlay)
        {
            Drawing = true;

            Graphics gfx = Graphics.FromImage(buffers[++currentBuffer % 2]);
            gfx.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;

            gfx.DrawImage(img, 1, 1, (int)this.Width, (int)this.Height);
            gfx.DrawImage(overlay, 1, 1, overlay.Width, overlay.Height);
            if (frame.window != null)
            {
                if (sem.WaitOne())
                {
                    try
                    {
                        frame.Pane.InvalidateVisual();
                        frame.window.Renderer.Paint(new Rect(0, 0, this.Width, this.Height));
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
