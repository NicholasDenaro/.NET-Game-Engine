using GameEngine.Interfaces;
using GameEngine.UI;
using GameEngine.UI.Audio;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace GameEngine
{
    public class GameBuilder
    {
        internal GameEngine Engine { get; private set; }
        internal GameUI Frame { get; private set; }

        private View view;
        private List<Controller> controllers = new List<Controller>();
        private Location location;
        private ISoundPlayer soundPlayer;

        public long tickTime;
        public long drawTime;
        public long tps;
        public long frameTime;

        public (GameEngine engine, GameUI frame) Build(BuildInfo buildInfo, int stateKey = 0)
        {
            return Build(stateKey);
        }

        public (GameEngine engine, GameUI frame) Build(int stateKey = 0)
        {
            if (!Engine.HasState(stateKey))
            {
                Engine.AddNewState(stateKey);
            }

            Engine.SetView(stateKey, view);
            if (view is ITicker)
            {
                Engine.TickEnd(stateKey) += (sender, state) => (view as ITicker).Tick(state);
            }

            Frame.SetSoundPlayer(this.soundPlayer);
            Frame.Start();
            Engine.Draw(stateKey) += Frame.DrawHandle;
            
            Engine.SetLocation(stateKey, location);

            foreach (Controller controller in controllers)
            {
                Engine.AddController(stateKey, controller);

                Frame.Window.Hook(controller);
            }

            Stopwatch swTick = new Stopwatch();
            Stopwatch swFrame = new Stopwatch();
            Engine.TickStart(stateKey) += (e, o) => { swTick.Restart(); swFrame.Restart(); };
            Engine.TickEnd(stateKey) += (e, o) => { swTick.Stop(); tickTime = swTick.ElapsedTicks; };

            Stopwatch swDraw = new Stopwatch();
            Engine.DrawStart(stateKey) += (e, o) => swDraw.Restart();
            Engine.DrawEnd(stateKey) += (e, o) => { swDraw.Stop(); drawTime = swDraw.ElapsedTicks; swFrame.Stop(); frameTime = swFrame.ElapsedTicks; };

            int ticks = 0;
            Stopwatch tpsWatch = Stopwatch.StartNew();
            Engine.TickStart(stateKey) += (e, o) => ticks++;
            Engine.TickEnd(stateKey) += (e, o) =>
            {
                if (tpsWatch.ElapsedTicks >= Stopwatch.Frequency)
                {
                    tpsWatch.Restart();
                    tps = ticks;
                    ticks = 0;
                }
            };

            return (Engine, Frame);
        }

        public GameBuilder GameEngine(GameEngine engine)
        {
            Engine = engine;

            return this;
        }

        public GameBuilder SoundPlayer(ISoundPlayer soundPlayer)
        {
            this.soundPlayer = soundPlayer;

            return this;
        }

        public GameBuilder GameView(View view)
        {
            this.view = view;

            return this;
        }

        public GameBuilder GameFrame(GameUI frame)
        {
            this.Frame = frame;

            return this;
        }

        public GameBuilder Controller(Controller controller)
        {
            this.controllers.Add(controller);

            return this;
        }

        public GameBuilder StartingLocation(Location location)
        {
            this.location = location;

            return this;
        }
    }

    public class BuildInfo
    {
        public Type EngineType { get; set; }
        public Type FrameBuilderType { get; set; }

        public Type ViewType { get; set; }
        public Type ViewDrawerType { get; set; }
    }
}
