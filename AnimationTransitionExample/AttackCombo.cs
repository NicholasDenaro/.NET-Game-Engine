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
            return attackAnimation < attackChainLength - 1;
        }

        public void Advance()
        {
            started = true;
            attackAnimation++;
            attackChainCooldown = attackGraceTime;
        }

        public bool Tick()
        {
            if (attackChainCooldown > 0)
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
