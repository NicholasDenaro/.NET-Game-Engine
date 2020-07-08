namespace AnimationTransitionExample
{
    public class CombatSkill : Skill
    {
        protected CombatSkill() : base()
        {

        }

        public CombatSkill(string name, SkillIcon icon, SkillAction action, int stamina, int cooldown) : base(name, icon, action, stamina, cooldown)
        {
        }

        public override Skill CreateNew()
        {
            CombatSkill skill = new CombatSkill();
            return (CombatSkill)base.CreateNew(skill);
        }
    }
}