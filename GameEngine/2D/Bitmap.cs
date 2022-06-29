using System.IO;

namespace GameEngine._2D
{
    public abstract class Bitmap
    {
        private static IBitmapCreator bitmapCreator;
        public static void SetBitmapImpl(IBitmapCreator bitmapCreator)
        {
            if (Bitmap.bitmapCreator != null)
            {
                throw new System.InvalidOperationException($"Can only call {nameof(SetBitmapImpl)} once.");
            }

            Bitmap.bitmapCreator = bitmapCreator;
        }

        public static Bitmap Create(int Width, int Height)
        {
            return bitmapCreator.Create(Width, Height);
        }

        public static Bitmap Create(int Width, int Height, bool mode)
        {
            return bitmapCreator.Create(Width, Height, mode);
        }

        public static Bitmap Create(Stream stream)
        {
            return bitmapCreator.Create(stream);
        }

        public int Width { get; protected set; }
        public int Height { get; protected set; }

        public abstract T Image<T>() where T : class;

        public abstract Graphics GetGraphics();

        public abstract Color GetPixel(int x, int y);
        public abstract void SetPixel(int x, int y, Color color);
    }

    public interface IBitmapCreator
    {
        public Bitmap Create(int width, int height);
        public Bitmap Create(int width, int height, bool dpiMode);
        public Bitmap Create(Stream stream);
    }

    public class BitmapSection
    {
        public Bitmap Bitmap { get; protected set; }
        public Rectangle Bounds { get; protected set; }

        public BitmapSection(Bitmap bitmap, Rectangle bounds)
        {
            this.Bitmap = bitmap;
            this.Bounds = bounds;
        }
    }
}
