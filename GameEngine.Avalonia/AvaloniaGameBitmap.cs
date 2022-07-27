using Avalonia.Media;
using Avalonia.Media.Imaging;
using GameEngine._2D;
using GameEngine._2D.Interfaces;
using System;
using System.IO;
using System.Threading.Tasks;

namespace GameEngine.UI.AvaloniaUI
{
    public class AvaloniaGameBitmap : _2D.Bitmap, IGameBitmap
    {
        private RenderTargetBitmap bmp;
        private DrawingContext gfx;
        private _2D.Graphics gpx;
        private static int count = 0;

        public AvaloniaGameBitmap(RenderTargetBitmap bmp)
        {
            this.bmp = bmp;
            this.Width = this.bmp.PixelSize.Width;
            this.Height = this.bmp.PixelSize.Height;
            gfx = new DrawingContext(bmp.CreateDrawingContext(null));
            gpx = new AvaloniaGraphics(ref gfx, this);
            Console.WriteLine(count++);
        }

        public DrawingContext Context => gfx;

        public override T Image<T>() where T : class
        {
            if (typeof(T) != typeof(RenderTargetBitmap))
            {
                throw new ArgumentOutOfRangeException(nameof(T));
            }

            return this.bmp as T;
        }

        public override _2D.Graphics GetGraphics()
        {
            return gpx;
        }

        internal void Clear(_2D.Color color)
        {
            if (!Avalonia.Threading.Dispatcher.UIThread.CheckAccess())
            {
                Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
                {
                    ClearImpl(color);
                });
            }
            else
            {
                ClearImpl(color);
            }
        }

        private void ClearImpl(_2D.Color color)
        {
            gfx.PlatformImpl.Clear(Avalonia.Media.Color.FromArgb(color.A, color.R, color.G, color.B));
        }

        public override _2D.Color GetPixel(int x, int y)
        {
            return new _2D.Color(0, 0, 0);
        }

        public override void SetPixel(int x, int y, _2D.Color color)
        {
            this.GetGraphics().FillRectangleAsync(color, x, y, 1, 1);
        }

        public double GetDpi()
        {
            return bmp.Dpi.X;
        }
    }

    public class AvaloniaGraphics : _2D.Graphics
    {
        private DrawingContext gfx;
        AvaloniaGameBitmap container;

        public AvaloniaGraphics(ref DrawingContext context, AvaloniaGameBitmap container)
        {
            this.gfx = context;
            this.container = container;
        }

        protected override void disposeManaged()
        {
            gfx.Dispose();
        }

        protected override void disposeUnmanaged()
        {
        }

        public override void Clear(_2D.Color color)
        {
            container.Clear(color);
        }

        public override Task DrawImageAsync(_2D.Bitmap bmp, RectangleF source, RectangleF dest)
        {
            if (!Avalonia.Threading.Dispatcher.UIThread.CheckAccess())
            {
                Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
                {
                    DrawImageImpl(bmp, source, dest);
                });
            }
            else
            {
                DrawImageImpl(bmp, source, dest);
            }

            return Task.CompletedTask;
        }

        private void DrawImageImpl(_2D.Bitmap bmp, RectangleF source, RectangleF dest)
        {
            gfx.DrawImage(
                bmp.Image<RenderTargetBitmap>(),
                new Avalonia.Rect(source.X, source.Y, source.Width, source.Height),
                new Avalonia.Rect(dest.X, dest.Y, dest.Width, dest.Height));
        }

        public override void DrawText(string text, int x, int y, _2D.Color color, int size)
        {
            if (!Avalonia.Threading.Dispatcher.UIThread.CheckAccess())
            {
                Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
                {
                    DrawTextImpl(text, x, y, color, size);
                });
            }
            else
            {
                DrawTextImpl(text, x, y, color, size);
            }
        }

        private void DrawTextImpl(string text, int x, int y, _2D.Color color, int size)
        {
            gfx.DrawText(
                    new SolidColorBrush(new Avalonia.Media.Color(color.A, color.R, color.G, color.B)),
                    new Avalonia.Point(x, y),
                    new FormattedText(text, new Typeface("consola", FontStyle.Normal, FontWeight.Bold), size, TextAlignment.Left, TextWrapping.NoWrap, new Avalonia.Size(1000, 1000)));
        }

        public override void DrawRectangle(_2D.Color color, int x, int y, int width, int height)
        {
            if (!Avalonia.Threading.Dispatcher.UIThread.CheckAccess())
            {
                Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
                {
                    DrawRectangleImpl(color, x, y, width, height);
                });
            }
            else
            {
                DrawRectangleImpl(color, x, y, width, height);
            }
        }

        private void DrawRectangleImpl(_2D.Color color, int x, int y, int width, int height)
        {
            gfx.DrawRectangle(
                null,
                new Pen(new SolidColorBrush(new Avalonia.Media.Color(color.A, color.R, color.G, color.B))),
                new Avalonia.Rect(x, y, width, height));
        }

        public override Task FillRectangleAsync(_2D.Color color, int x, int y, int width, int height)
        {
            if (!Avalonia.Threading.Dispatcher.UIThread.CheckAccess())
            {
                Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
                {
                    FillRectangleImpl(color, x, y, width, height);
                });
            }
            else
            {
                FillRectangleImpl(color, x, y, width, height);
            }

            return Task.CompletedTask;
        }

        private void FillRectangleImpl(_2D.Color color, int x, int y, int width, int height)
        {
            gfx.DrawRectangle(
                new SolidColorBrush(new Avalonia.Media.Color(color.A, color.R, color.G, color.B)),
                null,
                new Avalonia.Rect(x, y, width, height));
        }

        public override void FillEllipse(_2D.Color color, int x, int y, int width, int height)
        {
            Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
            {   
                gfx.DrawEllipse(
                    new SolidColorBrush(new Avalonia.Media.Color(color.A, color.R, color.G, color.B)),
                    null,
                    new Avalonia.Point(x + width / 2, y + height / 2),
                    width / 2,
                    height / 2);
            });
        }

        public override Task DrawEllipseAsync(_2D.Color color, int x, int y, int width, int height)
        {
            if (!Avalonia.Threading.Dispatcher.UIThread.CheckAccess())
            {
                Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
                {
                    DrawEllipseImpl(color, x, y, width, height);
                });
            }
            else
            {
                DrawEllipseImpl(color, x, y, width, height);
            }

            return Task.CompletedTask;
        }

        public void DrawEllipseImpl(_2D.Color color, int x, int y, int width, int height)
        {
            gfx.DrawEllipse(
                    null,
                    new Pen(new SolidColorBrush(new Avalonia.Media.Color(color.A, color.R, color.G, color.B))),
                    new Avalonia.Point(x + width / 2, y + height / 2),
                    width / 2,
                    height / 2);
        }

        public override void DrawArc(_2D.Color color, float v1, float v2, int v3, int v4, int v5, int v6)
        {
            if (!Avalonia.Threading.Dispatcher.UIThread.CheckAccess())
            {
                Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
                {
                    DrawArcImpl(color, v1, v2, v3, v4, v5, v6);
                });
            }
            else
            {
                DrawArcImpl(color, v1, v2, v3, v4, v5, v6);
            }
        }

        public void DrawArcImpl(_2D.Color color, float v1, float v2, int v3, int v4, int v5, int v6)
        {
            ArcSegment arc = new ArcSegment();
            PathGeometry pg = new PathGeometry();
            PathFigure pf = new PathFigure();
            pf.Segments.Add(arc);
            pg.Figures.Add(pf);

            gfx.DrawGeometry(
                new SolidColorBrush(new Avalonia.Media.Color(color.A, color.R, color.G, color.B)),
                new Pen(new SolidColorBrush(new Avalonia.Media.Color(color.A, color.R, color.G, color.B))),
                pg
                );
        }

        public override void DrawLine(_2D.Color color, int x1, int y1, int x2, int y2)
        {
            gfx.DrawLine(
                new Pen(new SolidColorBrush(new Avalonia.Media.Color(color.A, color.R, color.G, color.B))),
                new Avalonia.Point(x1, y1),
                new Avalonia.Point(x2, y2));
        }

        public override IDisposable TranslateTransform(int x, int y)
        {
            return gfx.PushPreTransform(new Avalonia.Matrix(1, 0, 0, 1, x, y));
        }

        public override IDisposable ScaleTransform(float x, float y)
        {
            return gfx.PushPreTransform(new Avalonia.Matrix(x, 0, 0, y , 0, 0));
        }
    }

    public class AvaloniaBitmapCreator : IBitmapCreator
    {
        public Task<_2D.Bitmap> CreateAsync(string name, int width, int height)
        {
            return Task.FromResult<_2D.Bitmap>(new AvaloniaGameBitmap(new RenderTargetBitmap(new Avalonia.PixelSize(width, height))));
        }
        public Task<_2D.Bitmap> CreateAsync(string name,int width, int height, bool dpiMode)
        {
            double dpi = dpiMode ? AvaloniaWindow.Dpi / AvaloniaWindow.DesktopScaling : AvaloniaWindow.Dpi;
            return Task.FromResult<_2D.Bitmap>(new AvaloniaGameBitmap(new RenderTargetBitmap(new Avalonia.PixelSize(width, height), new Avalonia.Vector(dpi, dpi))));
        }

        public Task<_2D.Bitmap> CreateAsync(string name, Stream stream)
        {
            var bmps = new Avalonia.Media.Imaging.Bitmap(stream);
            var bmp = new RenderTargetBitmap(new Avalonia.PixelSize(bmps.PixelSize.Width, bmps.PixelSize.Height));
            using DrawingContext mgfx = new DrawingContext(bmp.CreateDrawingContext(null));
            mgfx.DrawImage(bmps, new Avalonia.Rect(0, 0, bmps.PixelSize.Width, bmps.PixelSize.Height));
            return Task.FromResult<_2D.Bitmap>(new AvaloniaGameBitmap(bmp));
        }
    }
}
