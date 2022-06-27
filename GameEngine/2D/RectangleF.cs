using System;

namespace GameEngine._2D
{
    public class RectangleF : IRectangle<float>
    {

        private float x;
        public float X
        {
            get
            {
                return x;
            }
            set
            {
                x = value;
            }
        }
        private float y;
        public float Y
        {
            get
            {
                return y;
            }
            set
            {
                y = value;
            }
        }
        private float width;
        public float Width
        {
            get
            {
                return width;
            }
            set
            {
                width = value;
            }
        }
        private float height;
        public float Height
        {
            get
            {
                return height;
            }
            set
            {
                height = value;
            }
        }

        public RectangleF(float x, float y, float width, float height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public bool IntersectsWith(Rectangle rect)
        {
            float x1 = X;
            float x2 = x1 + Width;
            float x3 = rect.X;
            float x4 = x3 + rect.Width;

            float y1 = Y;
            float y2 = y1 + Height;
            float y3 = rect.Y;
            float y4 = y3 + rect.Height;

            float xL = Math.Max(x1, x3);
            float xR = Math.Min(x2, x4);
            if (xR <= xL)
            {
                return false;
            }
            else
            {
                float yT = Math.Max(y1, y3);
                float yB = Math.Min(y2, y4);

                if (yB <= yT)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
    }
}
