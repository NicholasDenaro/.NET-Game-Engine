using GameEngine._2D;
using System;
using System.Drawing;

namespace GameEngine.Interfaces
{
    public abstract class View : ITicker
    {
        internal Drawer Drawer { get; private set; }

        public View()
        {
            Drawer = new Drawer();
        }

        public abstract void Open();

        public abstract void Close();

        internal abstract void Draw(Location location);
        public abstract void Tick();
    }

    public class Drawer
    {
        public bool IsSetup { get; private set; }
        private Action<Entity> drawEntity;
        private Action<Sprite, int, int, int> drawSprite;
        private Action<Image, int, int> drawImage;

        public void Draw(Entity entity)
        {
            drawEntity(entity);
        }

        public void Draw(Sprite sprite, int index, int x, int y)
        {
            drawSprite(sprite, index, x, y);
        }

        public void Draw(Image image, int x, int y)
        {
            drawImage(image, x, y);
        }

        public void Setup(Action<Entity> drawEntity, Action<Sprite, int, int, int> drawSprite, Action<Image, int, int> drawImage)
        {
            this.drawEntity = drawEntity;
            this.drawSprite = drawSprite;
            this.drawImage = drawImage;
            this.IsSetup = true;
        }
    }
}
