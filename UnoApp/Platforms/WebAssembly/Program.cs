using Uno.UI.Hosting;

namespace UnoApp.Platforms.WebAssembly;

public class Program
{
    public static async Task Main(string[] _)
    {
        App.InitializeLogging();

        var host = UnoPlatformHostBuilder.Create().App(() => new App()).UseWebAssembly().Build();

        await host.RunAsync();
    }
}
