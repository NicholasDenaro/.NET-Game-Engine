using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using GameEngine._2D;
using System;
using System.Collections.Generic;
using System.Threading;

namespace GameEngine.UI.AvaloniaUI
{
    public class GamePanel : Panel, IGamePanel
    {
        public bool Drawing { get; private set; }
        public double ScaleX { get; private set; }
        public double ScaleY { get; private set; }

        public int WindowWidth { get; private set; }
        public int WindowHeight { get; private set; }

        internal AvaloniaWindow window;
        Semaphore sem = new Semaphore(1, 1);

        internal static GamePanel Panel { get; private set; }

        private List<Action<DrawingContext>> drawings;
        private GameView2D view;

        public GamePanel(AvaloniaWindow window, int width, int height, double xScale, double yScale)
        {
            Panel = this;
            this.window = window;
            this.Name = "panel";
            Drawing = false;
            this.ScaleX = xScale;
            this.ScaleY = yScale;
            this.Width = width * xScale;
            this.Height = height * yScale;
            WindowWidth = (int)this.Width;
            WindowHeight = (int)this.Height;
        }

        public void Draw(GameView2D view)
        {
            Drawing = true;

            Drawer2DAvalonia d = view.Drawer as Drawer2DAvalonia;
            drawings = d.Drawings;
            this.view = view;

            if (window != null)
            {
                if (sem.WaitOne())
                {
                    try
                    {
                        Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(DrawScreen).Wait();
                    }
                    finally
                    {
                        sem.Release();
                        Drawing = false;
                    }
                }
            }
        }

        public void DrawScreen()
        {
            if (this.window.WindowState != WindowState.Minimized)
            {
                this.InvalidateVisual();
                window?.Renderer?.Paint(new Rect(0, 0, WindowWidth, WindowHeight));
            }
        }

        public void DrawHandle(object sender, View view)
        {
            GameView2D view2D = view as GameView2D;
            if (view2D != null)
            {
                Draw(view2D);
            }
        }

        public override void Render(DrawingContext context)
        {
            bool selfDraw = Drawing;

            if (selfDraw || sem.WaitOne())
            {
                try
                {
                    base.Render(context);

                    using var undoTranform = context.PushPreTransform(context.CurrentContainerTransform.Invert());
                    using var scalePlat = context.PushPreTransform(new Matrix(1/this.window.PlatformImpl.DesktopScaling, 0, 0, 1/this.window.PlatformImpl.DesktopScaling, 0, 0));
                    using var scale = context.PushPreTransform(new Matrix(ScaleX, 0, 0, ScaleY, 0, 0));
                    using var translate = context.PushPreTransform(new Matrix(1, 0, 0, 1, -view?.ViewBounds.X ?? 0, -view?.ViewBounds.Y ?? 0));
                    drawings?.ForEach(act => act(context));
                }
                finally
                {
                    if (!selfDraw)
                    {
                        sem.Release();
                    }
                }
            }
        }
    }
}
