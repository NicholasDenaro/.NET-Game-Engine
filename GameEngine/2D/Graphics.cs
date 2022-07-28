﻿using System;
using System.Threading.Tasks;

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

        public abstract Task DrawImageAsync(Bitmap bmp, RectangleF source, RectangleF dest);
        public async Task DrawImageAsync(Bitmap bmp, int x, int y)
        {
            await DrawImageAsync(bmp, new RectangleF(0, 0, bmp.Width, bmp.Height), new RectangleF(x, y, bmp.Width, bmp.Height));
        }
        public async Task DrawImageAsync(BitmapSection bmp, int x, int y)
        {
            await DrawImageAsync(bmp.Bitmap, bmp.Bounds, new Rectangle(x, y, bmp.Bounds.Width, bmp.Bounds.Height));
        }

        public async Task DrawImageAsync(BitmapSection bmp, RectangleF dest)
        {
            await DrawImageAsync(bmp.Bitmap, bmp.Bounds, dest);
        }

        public abstract Task Clear(Color color);
        public abstract Task DrawText(string text, int x, int y, Color color, int size);
        public async Task DrawText(string text, Point point, Color color, int size)
        {
            await DrawText(text, point.X, point.Y, color, size);
        }
        public abstract Task DrawRectangle(Color color, int x, int y, int width, int height);
        public abstract Task FillRectangleAsync(Color color, int x, int y, int width, int height);
        public async Task FillRectangleAsync(Color color, Rectangle rect)
        {
            await FillRectangleAsync(color, rect.X, rect.Y, rect.Width, rect.Height);
        }
        public abstract Task DrawEllipseAsync(Color color, int x, int y, int width, int height);
        public abstract Task FillEllipse(Color color, int x, int y, int width, int height);
        public abstract Task DrawLine(Color color, int x1, int y1, int x2, int y2);
        public abstract Task DrawArc(Color color, float v1, float v2, int v3, int v4, int v5, int v6);
        public abstract Task<IAsyncDisposable> TranslateTransform(int x, int y);
        public abstract Task<IAsyncDisposable> ScaleTransform(float x, float y);
    }

    public class AsyncDisposableWrapper : IAsyncDisposable
    {
        private IDisposable disposable;

        public AsyncDisposableWrapper(IDisposable disposable)
        {
            this.disposable = disposable;
        }

        public ValueTask DisposeAsync()
        {
            this.disposable.Dispose();
            return ValueTask.CompletedTask;
        }
    }

#if NET48
    public interface IAsyncDisposable
    {
        ValueTask DisposeAsync();
    }

    public struct ValueTask
    {
        public bool IsCompleted { get; }
        public static ValueTask CompletedTask;
    }
#endif
}
