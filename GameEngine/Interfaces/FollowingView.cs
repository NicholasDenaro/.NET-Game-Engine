using System.Drawing;

namespace GameEngine.Interfaces
{
    public abstract class FollowingView : View
    {
        public IFollowable Following { get; private set; }

        public void Follow(IFollowable followable)
        {
            Following = followable;
        }
    }
}
