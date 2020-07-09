using System.Threading.Tasks;
using System.Windows.Forms;

namespace GameEngine.UI.WinForms
{
    public class WinFormWindowBuilder : IGameWindowBuilder
    {
        public (IGameWindow, ISoundPlayer) Run(IGameFrame frame)
        {
            WinFormWindow window = new WinFormWindow();
            window.Add(new GamePanel((int)(frame.Bounds.Width / frame.ScaleX), (int)(frame.Bounds.Height / frame.ScaleY), frame.ScaleX, frame.ScaleY));
            window.StartPosition = FormStartPosition.CenterScreen;
            window.FormBorderStyle = FormBorderStyle.None;
            window.AutoScaleMode = AutoScaleMode.Dpi;
            window.Width = frame.Bounds.Width;
            window.Height = frame.Bounds.Height;
            
            Task.Run(() => Application.Run(window));

            return (window, new WinFormSoundPlayer());
        }
    }
}
