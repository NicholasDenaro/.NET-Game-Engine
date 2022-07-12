namespace GameEngine.UI.Controllers
{
    public class KeyControllerInfo
    {

    }

    public class KeyEventArgs
    {
        public int KeyCode { get; private set; }

        public KeyEventArgs(int keyCode)
        {
            KeyCode = keyCode;
        }
    }
}