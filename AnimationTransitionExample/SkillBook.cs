using System.Collections.Generic;
using System.Linq;

namespace AnimationTransitionExample
{
    public class SkillBook
    {
        private List<Skill> skills;

        public SkillBook(params Skill[] skills)
        {
            this.skills = new List<Skill>(skills);
        }

        public Skill this[string name]
        {
            get
            {
                return skills.FirstOrDefault(skill => skill.Name == name);
            }
        }

        public void Tick()
        {
            foreach (Skill skill in skills)
            {
                skill.Tick();
            }
        }
    }
}