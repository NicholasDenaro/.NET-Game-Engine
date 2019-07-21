using GameEngine.Interfaces;
using System.Drawing;

namespace GameEngine._2D
{
    public class TileMap : IDescription
    {
        public Sprite Sprite { get; private set; }
        public Brush BackgroundColor { get; internal set; } = Brushes.Gray;
        public byte[] Tiles { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public int Columns { get; private set; }
        public int Rows { get; private set; }

        public TileMap(Sprite sprite, int width, int height)
        {
            Sprite = sprite;
            Width = width;
            Height = height;
            Columns = Width / sprite?.Width ?? 1;
            Rows = Height / sprite?.Height ?? 1;
            Tiles = new byte[Columns * Rows];
        }

        public Image Image(byte tile)
        {
            return Sprite.GetImage(tile);
        }

        public byte this[int x, int y]
        {
            get
            {
                return Tiles[x + y * Columns];
            }
            set
            {
                Tiles[x + y * Columns] = value;
            }
        }
    }
}
