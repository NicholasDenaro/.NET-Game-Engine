using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Visuals.Media.Imaging;
using GameEngine._2D;
using GameEngine._2D.Interfaces;
using System;
using System.IO;

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

        internal void Clear()
        {
            gfx.PlatformImpl.Clear(Avalonia.Media.Color.FromArgb(0, 0, 0, 0));
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
            container.Clear();
        }

        public override void DrawImage(_2D.Bitmap bmp, RectangleF source, RectangleF dest)
        {
            Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
            {
                gfx.DrawImage(
                bmp.Image<RenderTargetBitmap>(),
                new Avalonia.Rect(source.X, source.Y, source.Width, source.Height),
                new Avalonia.Rect(dest.X, dest.Y, dest.Width, dest.Height));
            });
        }

        public override void DrawText(string text, int x, int y, _2D.Color color, int size)
        {
            Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
            {
                gfx.DrawText(
                    new SolidColorBrush(new Avalonia.Media.Color(color.A, color.R, color.G, color.B)),
                    new Avalonia.Point(x, y),
                    new FormattedText(text, new Typeface("consola", FontStyle.Normal, FontWeight.Bold), size, TextAlignment.Left, TextWrapping.NoWrap, new Avalonia.Size(100, 100)));
            });
        }

        public override void DrawRectangle(_2D.Color color, int x, int y, int width, int height)
        {
            Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
            {
                gfx.DrawRectangle(
                new Pen(new SolidColorBrush(new Avalonia.Media.Color(color.A, color.R, color.G, color.B))),
                new Avalonia.Rect(x, y, width, height));
            });
        }

        public override void FillRectangle(_2D.Color color, int x, int y, int width, int height)
        {
            Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
            {
                gfx.FillRectangle(
                new SolidColorBrush(new Avalonia.Media.Color(color.A, color.R, color.G, color.B)),
                new Avalonia.Rect(x, y, width, height));
            });
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

        public override void DrawEllipse(_2D.Color color, int x, int y, int width, int height)
        {
            Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
            {
                gfx.DrawEllipse(
                    null,
                    new Pen(new SolidColorBrush(new Avalonia.Media.Color(color.A, color.R, color.G, color.B))),
                    new Avalonia.Point(x + width / 2, y + height / 2),
                    width / 2,
                    height / 2);
            });
        }

        public override void TranslateTransform(int x, int y)
        {
            throw new NotImplementedException();
        }
    }

    public class AvaloniaBitmapCreator : IBitmapCreator
    {
        public _2D.Bitmap Create(int width, int height)
        {
            return new AvaloniaGameBitmap(new RenderTargetBitmap(new Avalonia.PixelSize(width, height)));
        }

        public _2D.Bitmap Create(Stream stream)
        {
            var bmps = new Avalonia.Media.Imaging.Bitmap(stream);
            var bmp = new RenderTargetBitmap(new Avalonia.PixelSize(bmps.PixelSize.Width, bmps.PixelSize.Height));
            using DrawingContext mgfx = new DrawingContext(bmp.CreateDrawingContext(null));
            mgfx.DrawImage(bmps, new Avalonia.Rect(0, 0, bmps.PixelSize.Width, bmps.PixelSize.Height));
            return new AvaloniaGameBitmap(bmp);
        }
    }
}
