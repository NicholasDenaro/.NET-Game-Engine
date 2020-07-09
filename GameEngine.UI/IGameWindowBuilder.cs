namespace GameEngine.UI
{
    public interface IGameWindowBuilder
    {
        (IGameWindow, ISoundPlayer) Run(IGameFrame frame);
    }
}