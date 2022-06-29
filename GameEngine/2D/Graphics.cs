using System;

namespace GameEngine._2D
{
    public abstract class Graphics : IDisposable
    {
        private bool _disposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {

            }

            _disposed = true;
        }

        protected abstract void disposeManaged();

        protected abstract void disposeUnmanaged();

        public abstract void DrawImage(Bitmap bmp, RectangleF source, RectangleF dest);
        public void DrawImage(Bitmap bmp, int x, int y)
        {
            DrawImage(bmp, new RectangleF(0, 0, bmp.Width, bmp.Height), new RectangleF(x, y, bmp.Width, bmp.Height));
        }
        public void DrawImage(BitmapSection bmp, int x, int y)
        {
            DrawImage(bmp.Bitmap, bmp.Bounds, new Rectangle(x, y, bmp.Bounds.Width, bmp.Bounds.Height));
        }

        public void DrawImage(BitmapSection bmp, RectangleF dest)
        {
            DrawImage(bmp.Bitmap, bmp.Bounds, dest);
        }

        public abstract void Clear(Color color);
        public abstract void DrawText(string text, int x, int y, Color color, int size);
        public void DrawText(string text, Point point, Color color, int size)
        {
            DrawText(text, point.X, point.Y, color, size);
        }
        public abstract void DrawRectangle(Color color, int x, int y, int width, int height);
        public abstract void FillRectangle(Color color, int x, int y, int width, int height);
        public void FillRectangle(Color color, Rectangle rect)
        {
            FillRectangle(color, rect.X, rect.Y, rect.Width, rect.Height);
        }
        public abstract void DrawEllipse(Color color, int x, int y, int width, int height);
        public abstract void FillEllipse(Color color, int x, int y, int width, int height);
        public abstract void DrawLine(Color color, int x1, int y1, int x2, int y2);
        public abstract void DrawArc(Color color, float v1, float v2, int v3, int v4, int v5, int v6);
        public abstract IDisposable TranslateTransform(int x, int y);
        public abstract IDisposable ScaleTransform(float x, float y);
    }
}
