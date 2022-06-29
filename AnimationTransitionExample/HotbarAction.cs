using GameEngine._2D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimationTransitionExample
{
    public interface HotbarAction
    {
        BitmapSection Image();

        void Action(LivingEntity entity);
    }
}
