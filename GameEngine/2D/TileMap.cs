using System.Drawing;

namespace GameEngine._2D
{
    public class TileMap
    {
        public Sprite Sprite { get; private set; }
        public byte[] Tiles { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }

        public TileMap(Sprite sprite)
        {
            Sprite = sprite;
        }

        public void Setup(Location location)
        {
            Width = location.Width / Sprite.Width;
            Height = location.Height / Sprite.Height;
            Tiles = new byte[Width * Height];
        }

        public Image Image(byte tile)
        {
            return Sprite.GetImage(tile);
        }

        public byte this[int x, int y]
        {
            get
            {
                return Tiles[x + y * Width];
            }
            set
            {
                Tiles[x + y * Width] = value;
            }
        }
    }
}
