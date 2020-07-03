using GameEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GameEngine
{
    public class AnimationManager
    {
        private Dictionary<string, Animation> animations = new Dictionary<string, Animation>();

        private static AnimationManager singleton;
        public static AnimationManager Instance
        {
            get
            {
                if (singleton == null)
                {
                    singleton = new AnimationManager();
                }

                return singleton;
            }
        }

        public void Add(Animation ani)
        {
            animations.Add(ani.Name, ani);
        }

        public Animation this[string name]
        {
            get => animations[name].CreateNew();
        }
    }

    public class AnimationChain : Stack<Animation>
    {
        public AnimationChain(params Animation[] anis)
        {
            foreach (Animation ani in anis)
            {
                Push(ani);
            }
        }

        public bool Tick(IDescription description)
        {
            return Peek().Tick(description);
        }
    }

    public class AnimationStack
    {
        private List<AnimationChain> animations;

        public AnimationStack()
        {
            animations = new List<AnimationChain>();
        }

        public void Push(AnimationChain chain)
        {
            animations.Insert(0, chain);
        }

        public AnimationChain Peek()
        {
            return animations[0];
        }

        public AnimationChain Pop()
        {
            AnimationChain chain = animations[0];
            animations.RemoveAt(0);
            return chain;
        }

        public void Queue(AnimationChain chain)
        {
            animations.Add(chain);
        }

        public bool Any(Func<AnimationChain, bool> predicate = null)
        {
            return animations.Any(predicate ?? (ac => true));
        }
    }

    public class Animation
    {
        public delegate bool TriggerDelegate(IDescription description);
        public delegate void FirstDelegate(IDescription description);
        public delegate void TickDelegate(IDescription description);
        public delegate void FinalDelegate(IDescription description);

        public string Name { get; private set; }
        public int Duration { get; private set; }

        private int time;

        private bool interruptable;
        private bool pausing;

        private TriggerDelegate trigger;

        private TickDelegate onTick;

        private FirstDelegate onFirst;

        private FinalDelegate onFinal;
        public Animation(string name, int duration, TriggerDelegate trigger = null, FirstDelegate first = null, TickDelegate tick = null, FinalDelegate final = null)
        {
            Name = name;
            Duration = duration;
            AnimationManager.Instance.Add(this);
            this.trigger = trigger;
            this.onFirst = first;
            onTick = tick;
            onFinal = final;
        }

        protected Animation()
        {
        }

        protected virtual Animation CreateNew(Animation ani)
        {
            ani.Name = Name;
            ani.Duration = Duration;
            ani.time = Duration;
            ani.trigger = trigger;
            ani.onFirst = onFirst;
            ani.onTick = onTick;
            ani.onFinal = onFinal;
            return ani;
        }

        public virtual Animation CreateNew()
        {
            Animation ani = new Animation();
            return CreateNew(ani);
        }

        public void Reset()
        {
            this.time = Duration;
        }
        public void Kill()
        {
            this.time = 0;
        }

        public Animation Trigger(TriggerDelegate trigger)
        {
            this.trigger = trigger;
            return this;
        }

        public Animation MakeInterruptable()
        {
            this.interruptable = true;
            return this;
        }

        public Animation MakePausing()
        {
            this.pausing = true;
            return this;
        }

        public bool IsInterruptable()
        {
            return this.interruptable;
        }

        public bool IsPausing()
        {
            return this.pausing;
        }

        public int TicksLeft()
        {
            return time;
        }

        public bool Tick(IDescription description)
        {
            if (time == -1)
            {
                if (trigger(description))
                {
                    onFinal?.Invoke(description);
                    return true;
                }

                onTick?.Invoke(description);
            }
            else if (IsStarted() || (trigger?.Invoke(description) ?? true))
            {
                if (time == Duration)
                {
                    onFirst?.Invoke(description);
                }
                if (time > 1)
                {
                    onTick?.Invoke(description);
                }
                else if (time > 0)
                {
                    onFinal?.Invoke(description);
                    return true;
                }

                if (time > 0)
                {
                    time--;
                }
            }

            return false;
        }

        public bool IsDone()
        {
            return time == 0;
        }

        public bool IsStarted()
        {
            return time != Duration;
        }
    }
}
