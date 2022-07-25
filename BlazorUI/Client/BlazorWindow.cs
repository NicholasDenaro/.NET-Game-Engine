using GameEngine;
using GameEngine.UI;
using GameEngine.UI.Controllers;

namespace BlazorUI.Client
{
    public class BlazorWindow : IGameWindow
    {
        private BlazorPanel panel = new BlazorPanel();
        public IGamePanel Panel => panel;

        

        public bool Hook(Controller controller)
        {
            return false;
        }

        public bool HookKeyboard(Action<object, KeyEventArgs> frame_KeyDown, Action<object, KeyEventArgs> frame_KeyUp)
        {
            return false;
        }

        public bool HookMouse(Action<object, MouseEventArgs> frame_KeyInfo, Action<object, MouseEventArgs> frame_KeyDown, Action<object, MouseEventArgs> frame_KeyUp)
        {
            return false;
        }

        public void SetBounds(int x, int y, int width, int height)
        {
        }
    }
}
