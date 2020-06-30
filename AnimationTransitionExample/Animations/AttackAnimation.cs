using GameEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimationTransitionExample.Animations
{
    public class AttackAnimation : Animation
    {
        public AttackAnimation(string name, int duration, TriggerDelegate trigger = null, FirstDelegate first = null, TickDelegate tick = null, FinalDelegate final = null) 
            : base(name, duration, trigger, first, tick, final)
        {
        }

        protected AttackAnimation() : base()
        {
        }

        public override Animation CreateNew()
        {
            AttackAnimation ani = new AttackAnimation();
            return base.CreateNew(ani) as AttackAnimation;
        }
    }
}
