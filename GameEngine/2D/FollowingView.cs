using GameEngine._2D.Interfaces;

namespace GameEngine._2D
{
    public abstract class FollowingView : View
    {
        public IFollowable Following { get; private set; }

        public bool LockViewToLocation { get; set; }

        public void Follow(IFollowable followable)
        {
            Following = followable;
        }
    }
}
