using GameEngine;
using GameEngine._2D;
using GameEngine.Interfaces;
using System.Collections.Generic;

namespace AnimationTransitionExample
{
    public class Skill : HotbarAction
    {
        public delegate bool SkillAction(Location location, IDescription description);

        public int Level { get; private set; }

        public string Name { get; private set; }

        public SkillIcon Icon { get; private set; }

        public SkillAction SAction { get; private set; }

        public int Stamina { get; private set; }

        public int CooldownDuration { get; private set; }

        public int CooldownTime { get; private set; }

        public Skill(string name, SkillIcon icon, SkillAction action, int stamina, int cooldown)
        {
            this.Name = name;
            this.SAction = action;
            this.Icon = icon;
            this.Stamina = stamina;
            this.CooldownDuration = cooldown;
            this.CooldownTime = 0;
            SkillManager.Instance.Add(this);
        }

        protected Skill()
        {
        }

        public void Tick()
        {
            if (CooldownTime > 0)
            {
                CooldownTime--;
            }
        }

        public bool IsReady()
        {
            return CooldownTime == 0;
        }

        public void Cooldown()
        {
            this.CooldownTime = CooldownDuration;
        }

        public void Action(LivingEntity entity)
        {
            entity.SetPreppedSkill(this);
        }

        protected virtual Skill CreateNew(Skill skill)
        {
            skill.Name = Name;
            skill.Icon = Icon;
            skill.SAction = SAction;
            skill.Stamina = Stamina;
            skill.CooldownDuration = CooldownDuration;

            return skill;
        }

        public virtual Skill CreateNew()
        {
            Skill skill = new Skill();
            return CreateNew(skill);
        }

        public BitmapSection Image()
        {
            return Icon.Image();
        }
    }

    public class SkillIcon : Description2D
    {
        public SkillIcon(int xIndex, int yIndex) : base(Sprite.Sprites["skills"], 0, 0)
        {
            ImageIndex = xIndex % Sprite.HImages + yIndex * Sprite.HImages;
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