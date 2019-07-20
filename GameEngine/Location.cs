using GameEngine._2D;
using GameEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace GameEngine
{
    public class Location : ITicker
    {
        private Dictionary<Guid,Entity> entities = new Dictionary<Guid, Entity>();

        public int Width { get; private set; }
        public int Height { get; private set; }
        public Brush BackgroundColor { get; internal set; } = Brushes.Gray;
        public TileMap Map { get; set; }

        public Location(int width, int height)
        {
            Width = width;
            Height = height;
            Map = null;
        }

        public void AddEntity(Entity entity)
        {
            entities.Add(entity.Id, entity);
        }

        public void RemoveEntity(Entity entity)
        {
            RemoveEntity(entity.Id);
        }

        public void RemoveEntity(Guid id)
        {
            entities.Remove(id);
        }

        public void Tick()
        {
            foreach(Entity entity in entities.Values)
            {
                entity.Tick();
            }
        }

        public void Draw(Drawer drawer)
        {
            if (Map != null)
            {
                int x = 0;
                int y = 0;
                foreach (byte tile in Map.Tiles)
                {
                    drawer.Draw(Map.Image(tile), x * Map.Sprite.Width, y * Map.Sprite.Height);
                    x++;
                    if (x >= Map.Width)
                    {
                        x = 0;
                        y++;
                    }

                }
            }

            foreach (Entity entity in entities.Values)
            {
                drawer.Draw(entity);
            }
        }

        public static Location Load(string fname)
        {
            using (FileStream stream = File.OpenRead(fname))
            using (BinaryReader reader = new BinaryReader(stream))
            {
                int width = reader.ReadInt32();
                int height = reader.ReadInt32();
                string spName = reader.ReadString();
                int tileWidth = reader.ReadInt32();
                int tileHeight = reader.ReadInt32();
                Sprite sprite = new Sprite(Path.GetFileNameWithoutExtension(spName), spName, tileWidth, tileHeight);
                TileMap map = new TileMap(sprite);
                Location location = new Location(width, height);
                location.Map = map;
                map.Setup(location);
                int i = 0;
                while (reader.BaseStream.Position < reader.BaseStream.Length)
                {
                    map.Tiles[i++] = reader.ReadByte();
                }

                return location;
            }
        }
    }
}
