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
        public abstract void Clear(Color color);
        public abstract void DrawText(string text, int x, int y, Color color, int size);
        public abstract void DrawRectangle(Color color, int x, int y, int width, int height);
        public abstract void FillRectangle(Color color, int x, int y, int width, int height);
        public abstract void DrawEllipse(Color color, int x, int y, int width, int height);
        public abstract void FillEllipse(Color color, int x, int y, int width, int height);
        public abstract void TranslateTransform(int x, int y);
    }
}
