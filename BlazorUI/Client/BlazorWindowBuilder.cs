using BlazorUI.Shared;
using GameEngine.UI;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using System.Diagnostics;

namespace BlazorUI.Client
{
    public class BlazorWindowBuilder : IGameWindowBuilder
    {
        public IGameWindow Run(IGameUI frame)
        {
            GameEngine._2D.Bitmap.SetBitmapImpl(new BlazorBitmapCreator());
            var builder = WebAssemblyHostBuilder.CreateDefault(null);
            builder.RootComponents.Add<App>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            Task.Run(async () =>
            {
                await builder.Build().RunAsync();
            });

            Task.Run(async () =>
            {
                GameUI ui = frame as GameUI;
                Stopwatch sw = Stopwatch.StartNew();
                bool hooked = false;
                while (!hooked && sw.Elapsed.TotalSeconds < 5)
                {
                    if (ui.SoundPlayer != null && MainLayout.Instance != null)
                    {
                        ui.CacheImpl += MainLayout.Instance.CacheAudio;
                        ui.SoundPlayer.Hook(MainLayout.Instance.PlaySound);
                        //ui.SoundPlayer.Hook(MainLayout.Instance.SoundSink);
                        ui.SetInitialized();
                        hooked = true;
                    }
                    await Task.Delay(1);
                }
            });

            return new BlazorWindow(frame.Bounds.Width, frame.Bounds.Height, frame.ScaleX, frame.ScaleY);
        }
    }
}
