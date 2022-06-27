﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameEngine._2D
{
    public class Point
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Point() : this(0, 0) { }

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}
