using Blazor.Extensions;
using Blazor.Extensions.Canvas.Canvas2D;
using BlazorUI.Shared;
using GameEngine._2D;
using GameEngine._2D.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorUI.Client
{


    public class BlazorBitmap : GameEngine._2D.Bitmap, IGameBitmap
    {
        private ElementReference bitmap;
        private BECanvasComponent canvas;

        private BlazorGraphics graphics;

        public BlazorBitmap()
        {
        }

        public BlazorBitmap(int width, int height)
        {
            Width = width;
            Height = height;
            this.graphics = new BlazorGraphics(this);
        }

        public void Init(ElementReference bitmap)
        {
            this.bitmap = bitmap;
            IsInitialized = true;
        }

        public async Task Init(BECanvasComponent canvas)
        {
            this.canvas = canvas;
            this.graphics.Init(await canvas.CreateCanvas2DAsync());
            IsInitialized = true;
        }

        public override Graphics GetGraphics()
        {
            return graphics;
        }

        public override Color GetPixel(int x, int y)
        {
            //throw new NotImplementedException();
            return Color.Transparent;
        }

        public override T Image<T>()
        {
            //throw new NotImplementedException();
            //if (typeof(T) != typeof(RenderTargetBitmap))
            //{
            //    throw new ArgumentOutOfRangeException(nameof(T));
            //}

            //return this.bmp as T;
            return null;
        }

        public ElementReference Image()
        {
            if (canvas != null)
            {
                if (IsInitialized)
                {
                    return canvas.CanvasReference;
                }
                else
                {
                    return new ElementReference();
                }
            }
            else
            {
                return bitmap;
            }
        }

        public override void SetPixel(int x, int y, Color color)
        {
            //throw new NotImplementedException();
        }
    }

    public class BlazorGraphics : GameEngine._2D.Graphics
    {
        private Canvas2DContext context;
        private BlazorBitmap bitmap;
        public BlazorGraphics(BlazorBitmap bmp)
        {
            bitmap = bmp;
        }

        private async Task WaitForGraphics()
        {
            while (this.context == null)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(10));
            }
        }

        private static string ColorToString(Color color)
        {
            return $"#{color.R:X2}{color.G:X2}{color.B:X2}{color.A:X2}";
        }

        public void Init(Canvas2DContext context)
        {
            this.context = context;
        }

        public async Task BatchStart()
        {
            await this.context.BeginBatchAsync();
        }

        public async Task BatchEnd()
        {
            await this.context.EndBatchAsync();
        }

        public override async Task Clear(Color color)
        {
            await this.context.ClearRectAsync(0, 0, bitmap.Width, bitmap.Height);
            await DrawRectangle(color, 0, 0, bitmap.Width, bitmap.Height);
        }

        public override async Task DrawArc(Color color, float v1, float v2, int v3, int v4, int v5, int v6)
        {
            //throw new NotImplementedException();
        }

        public override async Task DrawEllipseAsync(Color color, int x, int y, int width, int height)
        {
            await this.context.SetStrokeStyleAsync(ColorToString(color));
            await this.context.SetFillStyleAsync(ColorToString(Color.Transparent));
            await this.context.BeginBatchAsync();
            await this.context.TransformAsync(width * 1.0 / Math.Max(width, height), 0, 0, height * 1.0 / Math.Max(width, height), 0, 0);
            await this.context.ArcAsync(x + width / 2, y + height / 2, height / 2, 0, Math.PI * 2, false);
            await this.context.StrokeAsync();
            await this.context.TransformAsync(Math.Max(width, height) * 1.0 / width, 0, 0, Math.Max(width, height) * 1.0 / height, 0, 0);
            await this.context.EndBatchAsync();
        }

        public override async Task DrawImageAsync(Bitmap bmp, RectangleF source, RectangleF dest)
        {
            await context.DrawImageAsync((bmp as BlazorBitmap).Image(), source.X, source.Y, source.Width, source.Height, dest.X, dest.Y, dest.Width, dest.Height);
        }

        public override async Task DrawLine(Color color, int x1, int y1, int x2, int y2)
        {
            await this.context.LineToAsync(x1, y1);
            await this.context.SetStrokeStyleAsync(ColorToString(color));
            await this.context.SetFillStyleAsync(ColorToString(color));
            await this.context.BeginBatchAsync();
            await this.context.LineToAsync(x2, y2);
            await this.context.StrokeAsync();
            await this.context.EndBatchAsync();
        }

        public override async Task DrawRectangle(Color color, int x, int y, int width, int height)
        {
            await this.context.SetStrokeStyleAsync(ColorToString(color));
            await this.context.SetFillStyleAsync(ColorToString(Color.Transparent));
            await this.context.FillRectAsync(x, y, width, height);
        }

        public override async Task DrawText(string text, int x, int y, Color color, int size)
        {
            await this.context.SetStrokeStyleAsync(ColorToString(color));
            await this.context.SetFillStyleAsync(ColorToString(color));
            // TODO: set font size
            foreach (string line in text.Split('\n'))
            {
                await this.context.FillTextAsync(line, x + 1, y + 10);
                y += 10;
            }
        }

        public override async Task FillEllipse(Color color, int x, int y, int width, int height)
        {
            await this.context.SetStrokeStyleAsync(ColorToString(color));
            await this.context.SetFillStyleAsync(ColorToString(color));
            await this.context.TransformAsync(width * 1.0 / Math.Max(width, height), 0, 0, height * 1.0 / Math.Max(width, height), 0, 0);
            await this.context.ArcAsync(x + width / 2 , y + height / 2, height / 2, 0, Math.PI);
            await this.context.TransformAsync(1, 0, 0, 1, 0, 0);
        }

        public override async Task FillRectangleAsync(Color color, int x, int y, int width, int height)
        {
            await this.context.SetStrokeStyleAsync(ColorToString(color));
            await this.context.SetFillStyleAsync(ColorToString(color));
            await this.context.FillRectAsync(x, y, width, height);
        }

        public override async Task<IAsyncDisposable> ScaleTransform(float x, float y)
        {
            await this.context.TransformAsync(x, 0, 0, y, 0, 0);
            return new TransformDisposeable(async () => await this.context.TransformAsync(1.0 / x, 0, 0, 1.0 / y, 0, 0));
        }

        public override async Task<IAsyncDisposable> TranslateTransform(int x, int y)
        {
            await this.context.TransformAsync(1, 0, 0, 1, x, y);
            return new TransformDisposeable(async () => await this.context.TransformAsync(1, 0, 0, 1, -x, -y));
        }

        protected override void disposeManaged()
        {
            //throw new NotImplementedException();
        }

        protected override void disposeUnmanaged()
        {
            //throw new NotImplementedException();
        }
    }

    public class TransformDisposeable : IAsyncDisposable
    {
        private Func<Task> revert;

        public TransformDisposeable(Func<Task> revert)
        {
            this.revert = revert;
        }

        public async ValueTask DisposeAsync()
        {
            await revert();
        }
    }

    public class BlazorBitmapCreator : IBitmapCreator
    {
        public async Task<Bitmap> CreateAsync(string name, int width, int height)
        {
            var bmp = new BlazorBitmap(width, height);

            await MainLayout.WaitInitialized();

            var eref = await MainLayout.Instance.AddBitmap(name, width, height);
            await bmp.Init(eref);

            return bmp;
        }

        public async Task<Bitmap> CreateAsync(string name, int width, int height, bool dpiMode)
        {
            return await CreateAsync(name, width, height);
        }

        public async Task<Bitmap> CreateAsync(string name, Stream stream)
        {
            MemoryStream ms = new MemoryStream();
            byte[] buff = new byte[1024];
            int read;
            while((read = stream.Read(buff, 0, buff.Length)) > 0)
            {
                ms.Write(buff, 0, read);
            }

            ms.Position = 0;

            byte[] sig = new byte[8];

            ms.Read(sig, 0, sig.Length);

            byte[] bint = new byte[4];

            ms.Position = 16;
            ms.Read(bint, 0, bint.Length);

            int width = BitConverter.ToInt32(bint.Reverse().ToArray());

            ms.Position = 20;
            ms.Read(bint, 0, bint.Length);

            int height = BitConverter.ToInt32(bint.Reverse().ToArray());

            var bmp = new BlazorBitmap(width, height);

            ms.Position = 0;

            await MainLayout.WaitInitialized();

            var eref = await MainLayout.Instance.AddBitmap(name, new DotNetStreamReference(ms));
            bmp.Init(eref);

            return bmp;
        }
    }
}
