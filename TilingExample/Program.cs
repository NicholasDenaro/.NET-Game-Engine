using GameEngine;
using GameEngine._2D;
using GameEngine.UI.AvaloniaUI;
using Splat;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection;

namespace TilingExample
{
    class Program
    {
        public static void Main(string[] args)
        {
            GameBuilder builder = new GameBuilder();

            Bitmap bmp = BitmapExtensions.CreateBitmap(320 * 4, 240 * 4);
            bmp.MakeTransparent();
            Graphics gfx = Graphics.FromImage(bmp);
            gfx.Clear(Color.Transparent);

            VectorD[][] vectors = GenerateNoise(0, 20);

            double[][] val = new double[240 * 4][];
            Func<double, int, int, int> step = (val, steps, max) => ((int)(val * steps) * max) / steps;

            for (int y = 0; y < 240 * 4; y++)
            {
                val[y] = new double[320 * 4];
                for (int x = 0; x < 320 * 4; x++)
                {
                    //val[y][x] = (Noise(vectors, x, y, 320 * 5,  240 * 5) * 0.2 + Noise(vectors, x, y, 320, 240) * 0.1 + Noise(vectors, x, y, 320 * 10, 240 * 10) * 0.7);
                    double[] noises = new double[] {
                        Noise(vectors, x, y, 10, 10) * 0.1,
                        Noise(vectors, x, y, 20 * 5, 20 * 5) * 0.8,
                        Noise(vectors, x, y, 5, 5) * 0.08,
                        Noise(vectors, x, y, 3, 3) * 0.02,
                    };
                    val[y][x] = noises.Aggregate((total, v) => total + v);
                    int v = StepWithPercent(val[y][x], 255, 0.6, 0.65, 0.8, 1);
                    bmp.SetPixel(x, y, Color.FromArgb(255, v, v, v));
                }
            }

            MemoryStream stream = new MemoryStream();
            bmp.Save(stream, ImageFormat.Bmp);

            stream.Position = 0;
            new Sprite("tilemap", "Sprites/SunnysideWorld_Tileset_V0.1.png", 16, 16);
            //new Sprite("player", "Sprites/player.png", 16, 16);
            TileMap map = new TileMap(Sprite.Sprites["tilemap"], 320 * 2, 240 * 2);
            map.BackgroundColor = Brushes.Bisque;
            Tile[,] mapTiles = new Tile[map.Width / 16, map.Height / 16];
            for (int y = 0; y < map.Height / 16; y++)
            {
                for (int x = 0; x < map.Width / 16; x++)
                {
                    double[] noises = new double[] {
                        Noise(vectors, x * 16, y * 16, 10, 10) * 0.1,
                        Noise(vectors, x * 16, y * 16, 20 * 5, 20 * 5) * 0.8,
                        Noise(vectors, x * 16, y * 16, 5, 5) * 0.08,
                        Noise(vectors, x * 16, y * 16, 3, 3) * 0.02,
                    };
                    int v = 3 - StepWithPercent(noises.Aggregate((total, v) => total + v), 4, 0.5, 0.6, 0.8, 1);
                    mapTiles[x, y] = new Tile((TileType)v);
                    //map[x, y] = f(1 + ((x + y) % 2), 1);
                }
            }

            for (int y = 0; y < map.Height / 16; y++)
            {
                for (int x = 0; x < map.Width / 16; x++)
                {
                    NeighborCalc(mapTiles, x, y);
                }
            }

            for (int y = 0; y < map.Height / 16; y++)
            {
                for (int x = 0; x < map.Width / 16; x++)
                {
                    //NeighborCalc2(mapTiles, x, y);
                    NeighborTransform(mapTiles, x, y);
                    map[x, y] = mapTiles[x, y].Modifier;
                }
            }

            new Sprite("noise", stream, 0, 0);

            Description2D noiseImg = new Description2D(Sprite.Sprites["noise"], 0, 0, 320 * 4, 240 * 4);

            builder.GameEngine(new FixedTickEngine(60))
                .GameView(new GameView2D(320 * 2, 240 * 2, 2, 2, Color.Bisque))
                .GameFrame(new GameEngine.UI.GameFrame(new AvaloniaWindowBuilder(), 0, 0, 320 * 2, 240 * 2, 2, 2))
                .StartingLocation(new Location(map))
                .Build();

            ////builder.Engine.AddEntity(new Entity(noiseImg));

            ////builder.Engine.TickEnd += (sender, state) => { d2d.ChangeCoordsDelta(0.1, 0); };

            builder.Engine.Start();

            while(true)
            {

            }
        }

        public static int StepWithPercent(double val, int scale, params double[] levels)
        {
            int i = 0;
            for(i = 0; i < levels.Length; i++)
            {
                if (val < levels[i])
                {
                    break;
                }
            }
            return i * scale / levels.Length;
        }

        public static int TransformTileMapCoords(int x, int y)
        {
            return (x + y * 1024 / 16);
        }

        public static void NeighborCalc(Tile[,] map, int x, int y)
        {
            Tile v = map[x, y];

            if (y != 0 && v.Type != map[x, y - 1].Type)
            {
                v.North = map[x, y - 1].Type;
                v.NorthBorder = map[x, y - 1].Type;
            }
            if (x != 0 && v.Type != map[x - 1, y].Type)
            {
                v.West = map[x - 1, y].Type;
                v.WestBorder = map[x - 1, y].Type;
            }
            if (y < map.GetLength(1) - 1 && v.Type != map[x, y + 1].Type)
            {
                v.South = map[x, y + 1].Type;
                v.SouthBorder = map[x, y + 1].Type;
            }
            if (x < map.GetLength(0) - 1 && v.Type != map[x + 1, y].Type)
            {
                v.East = map[x + 1, y].Type;
                v.EastBorder = map[x + 1, y].Type;
            }

            if (v.Type == TileType.Sand)
            {
                if (new[] { v.North, v.West, v.South, v.East }.Count(d => d != v.Type) == 1)
                {
                    v.NorthBorder = v.Type;
                    v.WestBorder = v.Type;
                    v.SouthBorder = v.Type;
                    v.EastBorder = v.Type;
                }
            }
        }

        /*public static void NeighborCalc2(Tile[,] map, int x, int y)
        {
            Tile v = map[x, y];

            if (v.Type == TileType.Grass)
            {
                if (y != 0 && v.Type == map[x, y - 1].SouthBorder)
                {
                    v.North = v.Type;
                }
                if (x != 0 && v.Type == map[x - 1, y].EastBorder)
                {
                    v.West = v.Type;
                }
                if (y < map.GetLength(1) - 1 && v.Type == map[x, y + 1].NorthBorder)
                {
                    v.South = v.Type;
                }
                if (x < map.GetLength(0) - 1 && v.Type == map[x + 1, y].WestBorder)
                {
                    v.East = v.Type;
                }
            }
        }*/

        /*public static VectorD NeighborVector(Tile[,] map, int x, int y)
        {
            Tile v = map[x, y];
            int xVec = 0;
            int yVec = 0;

            if (y != 0 && v.Type != map[x, y - 1].Type)
            {
                yVec++;
            }
            if (x != 0 && v.Type != map[x - 1, y].Type)
            {
                xVec--;
            }
            if (y < map.GetLength(1) - 1 && v.Type != map[x, y + 1].Type)
            {
                yVec--;
            }
            if (x < map.GetLength(0) - 1 && v.Type != map[x + 1, y].Type)
            {
                xVec++;
            }

            return new VectorD(xVec, yVec);
        }*/

        public static int NeighborCount(Tile[,] neighbors, TileType type, bool corners)
        {
            int count = 0;
            for (int j = 0; j < 3; j++)
            {
                for (int i = 0; i < 3; i++)
                {
                    if ((i == 1 && j == 1) || (!corners && !(i == 1 || j == 1)))
                    {
                        continue;
                    }
                    if (neighbors[i, j].Type == type)
                    {
                        count++;
                    }
                }
            }

            return count;
        }

        public static void NeighborTransform(Tile[,] map, int x, int y)
        {
            Tile v = map[x, y];

            int xx = 1 + (int)v.Type;
            int yy = 1;
            TileType border = v.Type;
            bool auto = false;
            int xVec = 0;
            int yVec = 0;
            bool keepOrigin = false;
            bool corners = false;
            bool inner = true;
            int neighborCount = 0;

            Tile[,] neighbors = new Tile[3, 3];
            for(int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    neighbors[i + 1, j + 1] = map[x, y];
                    if (!(x + i < 0 || x + i >= map.GetLength(0)
                        || y + j < 0 || y + j >= map.GetLength(1)))
                    {
                        neighbors[i + 1, j + 1] = map[x + i, y + j];
                    }
                }
            }

            ////if (v.Type == TileType.Sand)
            ////{
            ////    xx = 16;
            ////    yy = 4;
            ////    auto = true;
            ////    keepOrigin = true;
            ////    border = TileType.Grass;
            ////    inner = true;
            ////}

            if (v.Type == TileType.Grass)
            {
                xx = 16;
                yy = 4;
                keepOrigin = true;
                auto = true;
                corners = true;
                border = TileType.Sand;
                inner = false;
            }

            neighborCount = NeighborCount(neighbors, border, false);

            if (neighborCount > 2)
            {
                yy += 4;
            }

            if (auto)
            {

                if (neighborCount == 4)
                {

                }
                if (v.North == border)
                {
                    yVec++;
                }
                if (v.South == border)
                {
                    yVec--;
                }
                if (v.East == border)
                {
                    xVec--;
                }
                if (v.West == border)
                {
                    xVec++;
                }

                if (Math.Abs(xVec) + Math.Abs(yVec) == 1 && neighborCount < 3)
                {
                    xVec *= 2;
                    yVec *= 2;
                }

                if (corners && xVec == 0 && yVec == 0 && neighborCount < 3)
                {
                    if (neighbors[0, 0].Type == border)
                    {
                        xVec = 2;
                        yVec = 1;
                    }
                    else if (neighbors[2, 0].Type == border)
                    {
                        xVec = -2;
                        yVec = 1;
                    }
                    else if (neighbors[0, 2].Type == border)
                    {
                        xVec = 2;
                        yVec = -1;
                    }
                    else if (neighbors[2, 2].Type == border)
                    {
                        xVec = -2;
                        yVec = -1;
                    }
                }

                if (xVec == 0 && yVec == 0 && neighborCount == 0 && keepOrigin)
                {
                    v.Modifier = TransformTileMapCoords(1 + (int)v.Type, 1);
                }
                else
                {
                    v.Modifier = TransformTileMapCoords(xx + xVec, yy + yVec);
                }
            }

            /*if (auto)
            {
                if (inner)
                {
                    if (v.East == v.Type)
                    {
                        xVec--;
                    }

                    if (v.North == v.Type)
                    {
                        yVec++;
                    }

                    if (v.West == v.Type)
                    {
                        xVec++;
                    }

                    if (v.South == v.Type)
                    {
                        yVec--;
                    }
                }
                else
                {
                    if (v.East == v.Type)
                    {
                        xVec += 2;
                    }

                    if (v.North == v.Type)
                    {
                        yVec -= 2;
                    }

                    if (v.West == v.Type)
                    {
                        xVec -= 2;
                    }

                    if (v.South == v.Type)
                    {
                        yVec += 2;
                    }
                }

                if (yVec == 0 && xVec == 0 && corners)
                {
                    if (!(x == 0 || x == map.GetLength(0) - 1 || y == 0)
                        && map[x - 1, y - 1].Type == border
                        && map[x, y - 1].SouthBorder == v.Type
                        && map[x - 1, y].EastBorder == v.Type
                        && (map[x - 1, y - 1].SouthBorder != v.Type
                            || map[x - 1, y - 1].EastBorder != v.Type)
                        && map[x - 1, y - 1].Type != v.Type
                        && map[x + 1, y - 1].Type != border)//topleft
                    {
                        xVec = 1;
                        yVec = 2;
                    }
                    if (!(x == 0 || x == map.GetLength(0) - 1 || y == 0)
                        && map[x + 1, y - 1].Type == border
                        && map[x, y - 1].SouthBorder == v.Type
                        && map[x + 1, y].WestBorder == v.Type
                        && (map[x + 1, y - 1].SouthBorder != v.Type
                            || map[x + 1, y - 1].WestBorder != v.Type)
                        && map[x + 1, y - 1].Type != v.Type
                        && map[x - 1, y - 1].Type != border)//topright
                    {
                        xVec = -1;
                        yVec = 2;
                    }
                    if (!(x == 0 || x == map.GetLength(0) - 1 || y == 0 || y == map.GetLength(1) - 1)
                        && map[x - 1, y + 1].Type == border
                        && map[x, y + 1].NorthBorder == v.Type
                        && map[x - 1, y].EastBorder == v.Type
                        && (map[x - 1, y + 1].NorthBorder != v.Type
                            || map[x - 1, y + 1].EastBorder != v.Type)
                        && map[x - 1, y + 1].Type != v.Type
                        && map[x + 1, y + 1].Type != border)//bottomleft
                    {
                        xVec = 1;
                        yVec = -2;
                    }
                    if (!(x == 0 || x == map.GetLength(0) - 1 || y == 0 || y == map.GetLength(1) - 1)
                        && map[x + 1, y + 1].Type == border
                        && map[x, y + 1].NorthBorder == v.Type
                        && map[x + 1, y].WestBorder == v.Type
                        && (map[x + 1, y + 1].NorthBorder != v.Type
                            || map[x + 1, y + 1].WestBorder != v.Type)
                        && map[x + 1, y + 1].Type != v.Type
                        && map[x - 1, y + 1].Type != border)//bottomright
                    {
                        xVec = -1;
                        yVec = -2;
                    }
                }

                if (keepOrigin && yVec == 0 && xVec == 0)
                {
                    xx = 1 + (int)v.Type;
                    yy = 1;
                }

            if (auto)
            {
                if (y != 0 && v.Type != map[x, y - 1].Type && border == map[x, y - 1].Type)
                {
                    yVec++;
                }
                if (x != 0 && v.Type != map[x - 1, y].Type && border == map[x - 1, y].Type)
                {
                    xVec--;
                }
                if (y < map.GetLength(1) - 1 && v.Type != map[x, y + 1].Type && border == map[x, y + 1].Type)
                {
                    yVec--;
                }
                if (x < map.GetLength(0) - 1 && v.Type != map[x + 1, y].Type && border == map[x + 1, y].Type)
                {
                    xVec++;
                }

                if (corners)
                {
                    if (!(x == 0 || y == 0) && v.Type != map[x - 1, y - 1].Type && border == map[x - 1, y - 1].Type)
                    {
                        xVec--;
                        yVec++;
                    }
                    if (!(x >= map.GetLength(0) - 1 || y == 0) && v.Type != map[x + 1, y - 1].Type && border == map[x + 1, y - 1].Type)
                    {
                        xVec++;
                        yVec++;
                    }
                    if (!(x == 0 || y >= map.GetLength(1) - 1) && v.Type != map[x - 1, y + 1].Type && border == map[x - 1, y + 1].Type)
                    {
                        xVec--;
                        yVec--;
                    }
                    if (!(x >= map.GetLength(0) - 1 || y >= map.GetLength(1) - 1) && v.Type != map[x + 1, y + 1].Type && border == map[x + 1, y + 1].Type)
                    {
                        xVec++;
                        yVec--;
                    }
                }
            }

            if (yVec < 0 && v.Type == TileType.Grass)
            {

            }

            if (xVec != 0 || yVec != 0 || !keepOrigin)
            {
                int xDelta = 0;
                int yDelta = 0;
                int dir = ((int)(Math.Atan2(yVec, xVec) * 360 / 2 / Math.PI) + 360) % 360;
                switch(dir)
                {
                    case 0:
                        xDelta = ring;
                        break;
                    case 18: // TODO
                        xDelta = 0;
                        yDelta = 0;
                        break;
                    case 26: // TODO
                        xDelta = 0;
                        yDelta = 0;
                        break;
                    case 45:
                        xDelta = ring;
                        yDelta = -ring;
                        break;
                    case 63: // TODO
                        xDelta = 0;
                        yDelta = 0;
                        break;
                    case 71: // TODO
                        xDelta = 0;
                        yDelta = 0;
                        break;
                    case 90:
                        yDelta = -ring;
                        break;
                    case 108: // TODO
                        xDelta = 0;
                        yDelta = 0;
                        break;
                    case 116: // TODO
                        xDelta = 0;
                        yDelta = 0;
                        break;
                    case 135:
                        xDelta = -ring;
                        yDelta = -ring;
                        break;
                    case 153: // TODO
                        xDelta = 0;
                        yDelta = 0;
                        break;
                    case 180:
                        xDelta = -ring;
                        break;
                    case 207: // TODO
                        xDelta = 0;
                        yDelta = 0;
                        break;
                    case 225:
                        xDelta = -ring;
                        yDelta = ring;
                        break;
                    case 244: // TODO
                        xDelta = 0;
                        yDelta = 0;
                        break;
                    case 252: // TODO
                        xDelta = 0;
                        yDelta = 0;
                        break;
                    case 270:
                        yDelta = ring;
                        break;
                    case 289: // TODO
                        xDelta = 0;
                        yDelta = 0;
                        break;
                    case 297: // TODO
                        xDelta = 0;
                        yDelta = 0;
                        break;
                    case 315:
                        xDelta = ring;
                        yDelta = ring;
                        break;
                    case 334: // TODO
                        xDelta = 0;
                        yDelta = 0;
                        break;
                    case 342: // TODO
                        xDelta = 0;
                        yDelta = 0;
                        break;
                    default:
                        break;
                }

                v.Modifier = TransformTileMapCoords(xx + xDelta, yy + yDelta);
            }*/
        }

        public static VectorD[][] GenerateNoise(int seed, int size)
        {
            Random rand = new Random(seed);
            VectorD[][] vec = new VectorD[size][];
            for (int c = 0; c < size; c++)
            {
                vec[c] = new VectorD[size];
                for (int r = 0; r < size; r++)
                {
                    double direction = rand.NextDouble() * Math.PI * 2;
                    vec[c][r] = new VectorD(Math.Cos(direction), Math.Sin(direction));
                }
            }

            return vec;
        }

        public static double Noise(VectorD[][] vectors, int x, int y, double xScale, double yScale)
        {
            int length = vectors.Length;
            double xx = (((x / xScale) % length) + length) % length;
            double yy = (((y / yScale) % length) + length) % length;

            VectorD vectl = vectors[(int)yy][(int)xx];
            VectorD vectr = vectors[(int)yy][((int)xx + 1) % vectors.Length];
            VectorD vecbl = vectors[((int)yy + 1) % vectors.Length][(int)xx];
            VectorD vecbr = vectors[((int)yy + 1) % vectors.Length][((int)xx + 1) % vectors.Length];

            VectorD vec = new VectorD(xx - (int)xx, yy - (int)yy);
            double s = vec.Dot(vectl);
            vec = new VectorD(xx - ((int)xx + 1), yy - (int)yy);
            double t = vec.Dot(vectr);
            vec = new VectorD(xx - (int)xx, yy - ((int)yy + 1));
            double u = vec.Dot(vecbl);
            vec = new VectorD(xx - ((int)xx + 1), yy - ((int)yy + 1));
            double v = vec.Dot(vecbr);

            double xW = xx - (int)xx;
            xW = 6 * xW * xW * xW * xW * xW - 15 * xW * xW * xW * xW + 10 * xW * xW * xW;
            double yW = yy - (int)yy;
            yW = 6 * yW * yW * yW * yW * yW - 15 * yW * yW * yW * yW + 10 * yW * yW * yW;

            ////return (((s * (1 - xW) + t * xW) * (1 - yW)
            ////    + (u * (1 - xW) + v * xW) * yW) / 0.65 + 1) / 2;
            double calc = ((s * (1 - xW) + t * xW) * (1 - yW)
                + (u * (1 - xW) + v * xW) * yW);
            return ((calc / 0.65) + 1) / 2;
        }
    }

    public enum TileType { Water = 3, Sand = 2, Grass = 1, Grasser = 0};

    public class Tile
    {
        public TileType Type { get; private set; }
        public TileType North { get; set; }
        public TileType South { get; set; }
        public TileType East { get; set; }
        public TileType West { get; set; }
        public TileType NorthBorder { get; set; }
        public TileType SouthBorder { get; set; }
        public TileType EastBorder { get; set; }
        public TileType WestBorder { get; set; }

        public int Modifier { get; set; }

        public Tile(TileType type)
        {
            Type = type;
            North = type;
            East = type;
            South = type;
            West = type;
            NorthBorder = type;
            EastBorder = type;
            SouthBorder = type;
            WestBorder = type;
            Modifier = Program.TransformTileMapCoords(1 + (int)type, 1);
        }
    }

    public class VectorD
    {
        public double X { get; set; }
        public double Y { get; set; }

        public VectorD(Vector2 vec)
        {
            X = vec.X;
            Y = vec.Y;
        }

        public VectorD(double x, double y)
        {
            X = x;
            Y = y;
        }

        public double Dot(VectorD other)
        {
            return (X * other.X) + (Y * other.Y);
        }
    }
}
