using System.Drawing;

namespace GameEngine._2D
{
    public class TileMap : Description2D
    {
        public Brush BackgroundColor { get; set; } = Brushes.Gray;
        public byte[] Tiles { get; private set; }
        public int Columns { get; private set; }
        public int Rows { get; private set; }

        public TileMap(Sprite sprite, int width, int height) : base(sprite, 0, 0, width, height)
        {
            Columns = 1;
            Rows = 1;
            if (Sprite != null)
            {
                Columns = ((width - 1) / Sprite.Width) + 1;
                Rows = ((height - 1) / Sprite.Height) + 1;
            }
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
                if(x + y * Columns >= Tiles.Length)
                {
                    return 0;
                }

                return Tiles[x + y * Columns];
            }
            set
            {
                if (x + y * Columns >= Tiles.Length)
                {
                    return;
                }

                Tiles[x + y * Columns] = value;
            }
        }
    }
}
