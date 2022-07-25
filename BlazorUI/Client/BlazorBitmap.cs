using GameEngine._2D;
using GameEngine._2D.Interfaces;

namespace BlazorUI.Client
{


    public class BlazorBitmap : GameEngine._2D.Bitmap, IGameBitmap
    {
        public int Width { get; private set; }

        public int Height { get; private set; }


        private BlazorGraphics graphics;

        public BlazorBitmap(int width, int height)
        {
            Width = width;
            Height = height;
            graphics = new BlazorGraphics();
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

        public override void SetPixel(int x, int y, Color color)
        {
            //throw new NotImplementedException();
        }
    }

    public class BlazorGraphics : GameEngine._2D.Graphics
    {
        public override void Clear(Color color)
        {
            //throw new NotImplementedException();
        }

        public override void DrawArc(Color color, float v1, float v2, int v3, int v4, int v5, int v6)
        {
            //throw new NotImplementedException();
        }

        public override void DrawEllipse(Color color, int x, int y, int width, int height)
        {
            //throw new NotImplementedException();
        }

        public override void DrawImage(Bitmap bmp, RectangleF source, RectangleF dest)
        {
            //throw new NotImplementedException();
        }

        public override void DrawLine(Color color, int x1, int y1, int x2, int y2)
        {
            //throw new NotImplementedException();
        }

        public override void DrawRectangle(Color color, int x, int y, int width, int height)
        {
            //throw new NotImplementedException();
        }

        public override void DrawText(string text, int x, int y, Color color, int size)
        {
            //throw new NotImplementedException();
        }

        public override void FillEllipse(Color color, int x, int y, int width, int height)
        {
            //throw new NotImplementedException();
        }

        public override void FillRectangle(Color color, int x, int y, int width, int height)
        {
            //throw new NotImplementedException();
        }

        public override IDisposable ScaleTransform(float x, float y)
        {
            //throw new NotImplementedException();
            return new Disposeable();
        }

        public override IDisposable TranslateTransform(int x, int y)
        {
            //throw new NotImplementedException();
            return new Disposeable();
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

    public class Disposeable : IDisposable
    {
        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }

    public class BlazorBitmapCreator : IBitmapCreator
    {
        public Bitmap Create(int width, int height)
        {
            //throw new NotImplementedException();
            return new BlazorBitmap(width, height);
        }

        public Bitmap Create(int width, int height, bool dpiMode)
        {
            //throw new NotImplementedException();
            return new BlazorBitmap(width, height);
        }

        public Bitmap Create(Stream stream)
        {
            //throw new NotImplementedException();
            return new BlazorBitmap(0, 0);
        }
    }
}
