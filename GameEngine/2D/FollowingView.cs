using GameEngine._2D.Interfaces;
using GameEngine;

namespace GameEngine._2D
{
    public abstract class FollowingView : View
    {
        public IFollowable Following { get; private set; }

        public bool LockViewToLocation { get; set; }

        public Location Location { get; internal set; }

        public void Follow(IFollowable followable)
        {
            Following = followable;
        }
    }
}
