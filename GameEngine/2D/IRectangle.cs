using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameEngine._2D
{
    public interface IRectangle<Type>
    {
        public Type X { get; set; }
        public Type Y { get; set; }
        public Type Width { get; set; }
        public Type Height { get; set; }

        public bool IntersectsWith(Rectangle rect);
    }
}
