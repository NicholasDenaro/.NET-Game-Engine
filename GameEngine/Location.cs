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

        public IDescription Description { get; set; }

        public Location(IDescription description)
        {
            Description = description;
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

        public void Draw(IDrawer drawer)
        {
            drawer.Draw(this);

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
                TileMap map = new TileMap(sprite, width, height);
                Location location = new Location(map);
                int tiles = reader.ReadInt32();
                for (int i = 0; i < tiles; i++)
                {
                    map.Tiles[i] = reader.ReadByte();
                }

                return location;
            }
        }

        public static byte[] Save(Location location, string spritePath)
        {
            using (MemoryStream stream = new MemoryStream())
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                TileMap description = location.Description as TileMap;
                if (description != null)
                {
                    writer.Write(description.Width);
                    writer.Write(description.Height);
                    writer.Write(spritePath);
                    writer.Write(description.Sprite.Width);
                    writer.Write(description.Sprite.Height);
                    writer.Write(description.Tiles.Length);
                    foreach (byte b in description.Tiles)
                    {
                        writer.Write(b);
                    }
                }

                return stream.GetBuffer();
            }
        }
    }
}
