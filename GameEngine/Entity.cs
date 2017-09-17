using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameEngine
{
    public class Entity : ITicker, IFocusable
    {
        public static Dictionary<Guid, Entity> Entities { get; private set; } = new Dictionary<Guid, Entity>();

        public Guid Id { get; private set; }

        public double X { get; private set; }
        public double Y { get; private set; }
        public Sprite Sprite { get; private set; }
        public int ImageIndex { get; set; }


        public Entity(int x, int y)
        {
            this.Id = Guid.NewGuid();
            Entities.Add(Id, this);
            SetCoords(x, y);
        }

        public Entity(int x, int y, Sprite sprite)
        {
            this.Id = Guid.NewGuid();
            SetCoords(x, y);
            this.Sprite = sprite;
        }

        public void SetCoords(double x , double y)
        {
            // Listener here?
            this.X = x;
            this.Y = y;
        }

        public void SetCoordsRel(double dx, double dy)
        {
            SetCoords(X + dx, Y + dy);
        }

        public Action TickAction;

        virtual public void Tick()
        {
            TickAction?.Invoke();
        }

        public Image Image()
        {
            return Sprite.GetImage(ImageIndex);
        }
    }
}
