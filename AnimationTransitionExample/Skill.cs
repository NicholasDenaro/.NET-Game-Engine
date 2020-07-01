using GameEngine;
using GameEngine.Interfaces;
using System;
using System.Collections.Generic;

namespace AnimationTransitionExample
{
    public class Skill
    {
        public delegate bool SkillAction(Location location, IDescription description);

        public int Level { get; private set; }

        public string Name { get; private set; }

        public SkillAction Action { get; private set; }

        public Skill(string name, SkillAction action)
        {
            this.Name = name;
            this.Action = action;
            SkillManager.Instance.Add(this);
        }

        protected Skill()
        {
        }

        protected virtual Skill CreateNew(Skill skill)
        {
            skill.Name = Name;
            skill.Action = Action;

            return skill;
        }

        public virtual Skill CreateNew()
        {
            Skill skill = new Skill();
            return CreateNew(skill);
        }
    }

    public class SkillManager
    {
        private static SkillManager singleton;
        public static SkillManager Instance
        {
            get
            {
                if (singleton == null)
                {
                    singleton = new SkillManager();
                }

                return singleton;
            }
        }

        private Dictionary<string, Skill> skills;
        private SkillManager()
        {
            skills = new Dictionary<string, Skill>();
        }

        public void Add(Skill skill)
        {
            skills.Add(skill.Name, skill);
        }

        public Skill this[string name]
        {
            get
            {
                return skills[name].CreateNew();
            }
        }
    }
}