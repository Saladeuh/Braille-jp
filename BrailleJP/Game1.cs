using BrailleJP.MiniGames;
using BrailleJP.Save;
using CrossSpeak;
using Microsoft.Xna.Framework;
using SharpLouis;
using System;
using System.Collections.Generic;

namespace BrailleJP;
public partial class Game1 : Game
{
  private readonly GameState _gameState;

  public static LibLouisLoggingClient LibLouisLoggingClient { get; set; } = new LibLouisLoggingClient();
  // Singleton
  public static Game1 Instance { get; private set; }
  public Random Random { get; set; } = new();
  public Wrapper InputBrailleTranslator { get; set; }
  private BrailleTableParser BrailleParser { get; set; }
  public Dictionary<string, List<BrailleEntry>> BrailleTables { get; set; }
  private IMiniGame CurrentPlayingMiniGame { get; set; } = null;
  public Game1()
  {
    _graphics = new GraphicsDeviceManager(this);
    Content.RootDirectory = "Content";
    _basicPracticePanels = [];
    _wordPracticePanels = [];
    _choicePracticePanels = [];
    foreach (var culture in SUPPORTEDBRAILLETABLES.Keys)
    {
      _basicPracticePanels[culture] = null;
      _wordPracticePanels[culture] = null;
      _choicePracticePanels[culture] = null;
    }
    InputBrailleTranslator = Wrapper.Create("fr-bfu-comp8.utb", Game1.LibLouisLoggingClient);
    IsMouseVisible = true;
    _gameState = new GameState();
    BrailleParser = new BrailleTableParser(@"LibLouis\tables");
    BrailleTables = [];
    Instance = this; // Set the static instance
  }

  private void onExit(object sender, EventArgs e)
  {
    SaveManager.WriteSave(this.Save);
    CrossSpeakManager.Instance.Close();
  }
}