using GameEngine._2D;
using GameEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace GameEngine
{
    public class Location
    {
        private Dictionary<Guid,Entity> entities = new Dictionary<Guid, Entity>();
        private List<Entity> entityAddBuffer = new List<Entity>();
        private List<Guid> entityRemoveBuffer = new List<Guid>();

        public IEnumerable<Entity> Entities => entities.Values;
        public IDescription Description { get; set; }

        public Location(IDescription description)
        {
            Description = description;
        }

        public void AddEntity(Entity entity)
        {
            entityAddBuffer.Add(entity);
        }

        public void RemoveEntity(Guid id)
        {
            entityRemoveBuffer.Add(id);
        }

        public IEnumerable<T> GetEntities<T>() where T : class, IDescription
        {
            return Entities.Where(entity => entity.Description.GetType() == typeof(T)).Select(entity => entity.Description as T);
        }

        public void Tick()
        {
            foreach(Entity entity in this.entityAddBuffer)
            {
                entities.Add(entity.Id, entity);
            }
            this.entityAddBuffer.Clear();

            foreach (Guid id in this.entityRemoveBuffer)
            {
                entities.Remove(id);
            }
            this.entityRemoveBuffer.Clear();

            foreach (Entity entity in entities.Values)
            {
                entity.Tick(this);
            }
        }

        public List<IDescription> Draw()
        {
            var descrs = new List<IDescription>();
            descrs.Add(Description);
            descrs.AddRange(entities.Values.Select(entity => entity.Description));
            return descrs;
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

        public static byte[] Save(Location location)
        {
            using (MemoryStream stream = new MemoryStream())
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                TileMap description = location.Description as TileMap;
                if (description != null)
                {
                    writer.Write(description.Width);
                    writer.Write(description.Height);
                    writer.Write($"Sprites/{description.Sprite.Name}.png");
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
