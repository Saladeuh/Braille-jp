using System.Threading.Tasks;
using Velopack;
using Velopack.Sources;

internal class Program
{
  private static void Main(string[] args)
  {
    using BrailleJP.Game1 game = new();
//#if !DEBUG
    VelopackApp.Build().Run();
//#endif
    game.Run();
  }
  private static async Task UpdateMyApp()
  {
    var mgr = new UpdateManager(new GithubSource("https://github.com/Saladeuh/Braille-jp/", "", false));

    // check for new version
    var newVersion = await mgr.CheckForUpdatesAsync();
    if (newVersion == null)
      return; // no update available

    // download new version
    await mgr.DownloadUpdatesAsync(newVersion);

    // install new version and restart app
    mgr.ApplyUpdatesAndRestart(newVersion);
  }
}