using GameEngine._2D;
using GameEngine._2D.Interfaces;
using GameEngine.Interfaces;

namespace BlazorUI.Client
{
    public class Drawer2DBlazor : IDrawer2D<BlazorBitmap>
    {
        public void Clear(int buffer, Color color)
        {
            //throw new NotImplementedException();
        }

        public void Draw(int buffer, IDescription description)
        {
            //throw new NotImplementedException();
        }

        public void FillRectangle(int buffer, Color color, int x, int y, int w, int h)
        {
            //throw new NotImplementedException();
        }

        public BlazorBitmap Image(int buf)
        {
            throw new NotImplementedException();
        }

        public void Init(int width, int height, int xScale, int yScale)
        {
            //throw new NotImplementedException();
        }

        public BlazorBitmap Overlay(int buf)
        {
            throw new NotImplementedException();
        }

        public void TranslateTransform(int buffer, int x, int y)
        {
            //throw new NotImplementedException();
        }
    }
}
