﻿using GameEngine.UI;
using System.Collections.Generic;
using System.Diagnostics;

namespace GameEngine
{
    public class GameBuilder
    {
        public GameEngine Engine { get; private set; }
        public GameFrame Frame { get; private set; }

        private View view;
        private List<Controller> controllers = new List<Controller>();
        private Location location;

        public long tickTime;
        public long drawTime;
        public long tps;

        public GameBuilder Build(int stateKey = 0)
        {
            if (!Engine.HasState(stateKey))
            {
                Engine.AddNewState(stateKey);
            }

            Engine.SetView(stateKey, view);

            Frame.Start();
            Engine.DrawEnd(stateKey) += Frame.DrawHandle;
            
            Engine.SetLocation(stateKey, location);

            foreach (Controller controller in controllers)
            {
                Engine.AddController(stateKey, controller);

                Frame.Window.Hook(controller);
            }

            Stopwatch swTick = new Stopwatch();
            Engine.TickStart(stateKey) += (e, o) => swTick.Restart();
            Engine.TickEnd(stateKey) += (e, o) => { swTick.Stop(); ; tickTime = swTick.ElapsedTicks; };

            Stopwatch swDraw = new Stopwatch();
            Engine.DrawStart(stateKey) += (e, o) => swDraw.Restart();
            Engine.DrawEnd(stateKey) += (e, o) => { swDraw.Stop(); drawTime = swDraw.ElapsedTicks; };

            int ticks = 0;
            Stopwatch tpsWatch = Stopwatch.StartNew();
            Engine.TickStart(stateKey) += (e, o) => ticks++;
            Engine.TickEnd(stateKey) += (e, o) =>
            {
                if (tpsWatch.ElapsedMilliseconds >= 1000)
                {
                    tpsWatch.Restart();
                    tps = ticks;
                    ticks = 0;
                }
            };

            return this;
        }

        public GameBuilder GameEngine(GameEngine engine)
        {
            Engine = engine;

            return this;
        }

        public GameBuilder GameView(View view)
        {
            this.view = view;

            return this;
        }

        public GameBuilder GameFrame(GameFrame frame)
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
}
