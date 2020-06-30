using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimationTransitionExample
{
    public class AttackCombo
    {
        private int attackAnimation;
        private int attackGraceTime;
        private int attackChainLength;
        private int attackChainCooldown;
        private bool started;

        public int Attack => attackAnimation;

        public AttackCombo(int chainLength, int graceTime)
        {
            attackChainLength = chainLength;
            attackGraceTime = graceTime;
        }

        public bool CanChain()
        {
            return attackAnimation < attackChainLength;
        }

        public void Advance()
        {
            started = true;
            attackAnimation++;
            attackAnimation = attackAnimation % attackChainLength;
            attackChainCooldown = attackGraceTime;
        }

        public bool Tick()
        {
            if (CanChain())
            {
                attackChainCooldown--;
                return false;
            }
            else
            {
                attackAnimation = 0;
                started = false;
                return true;
            }
        }

        internal bool IsStarted()
        {
            return started;
        }

        public void Reset()
        {
            attackAnimation = 0;
            started = false;
        }
    }
}
