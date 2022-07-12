using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameEngine._2D
{
    public interface IRectangle<Type>
    {
        Type X { get; set; }
        Type Y { get; set; }
        Type Width { get; set; }
        Type Height { get; set; }

        bool IntersectsWith(Rectangle rect);
    }
}
