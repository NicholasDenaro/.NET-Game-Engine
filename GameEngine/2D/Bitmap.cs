using System.IO;
using System.Threading.Tasks;

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

        public static async Task<Bitmap> CreateAsync(string name, int Width, int Height)
        {
            return await bitmapCreator.CreateAsync(name, Width, Height);
        }

        public static async Task<Bitmap> CreateAsync(string name, int Width, int Height, bool mode)
        {
            return await bitmapCreator.CreateAsync(name, Width, Height, mode);
        }

        public static async Task<Bitmap> CreateAsync(string name, Stream stream)
        {
            return await bitmapCreator.CreateAsync(name, stream);
        }

        public int Width { get; protected set; }
        public int Height { get; protected set; }

        public bool IsInitialized { get; protected set; }

        public abstract T Image<T>() where T : class;

        public abstract Graphics GetGraphics();

        public abstract Color GetPixel(int x, int y);
        public abstract void SetPixel(int x, int y, Color color);
    }

    public interface IBitmapCreator
    {
        Task<Bitmap> CreateAsync(string name, int width, int height);
        Task<Bitmap> CreateAsync(string name, int width, int height, bool dpiMode);
        Task<Bitmap> CreateAsync(string name, Stream stream);
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
