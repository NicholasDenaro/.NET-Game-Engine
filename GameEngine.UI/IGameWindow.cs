using System;
using System.Collections.Generic;
using System.Text;

namespace GameEngine.UI
{
    public interface IGameWindow : IHookable
    {
        bool HookMouse(Action<object, MouseEventArgs> frame_KeyInfo, Action<object, MouseEventArgs> frame_KeyDown, Action<object, MouseEventArgs> frame_KeyUp);

        bool HookKeyboard(Action<object, KeyEventArgs> frame_KeyDown, Action<object, KeyEventArgs> frame_KeyUp);

        IGamePanel Panel { get; }
    }
}
