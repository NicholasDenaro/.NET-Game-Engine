namespace AnimationTransitionExample
{
    public class CombatSkill : Skill
    {
        protected CombatSkill() : base()
        {

        }

        public CombatSkill(string name, SkillIcon icon, SkillAction action, bool canMove) : base(name, icon, action, canMove)
        {
        }

        public override Skill CreateNew()
        {
            CombatSkill skill = new CombatSkill();
            return (CombatSkill)base.CreateNew(skill);
        }
    }
}