using BlazorUI.Shared;
using GameEngine.UI;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace BlazorUI.Client
{
    public class BlazorWindowBuilder : IGameWindowBuilder
    {
        public IGameWindow Run(IGameUI frame)
        {
            Console.WriteLine("GameEngineUIMain");
            GameEngine._2D.Bitmap.SetBitmapImpl(new BlazorBitmapCreator());
            var builder = WebAssemblyHostBuilder.CreateDefault(null);
            builder.RootComponents.Add<App>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            Task.Run(async () =>
            {
                await builder.Build().RunAsync();
            });

            return new BlazorWindow();
        }
    }
}
