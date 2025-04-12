using BrailleJP.MiniGames;
using BrailleJP.UI;
using CrossSpeak;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Myra.Graphics2D.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Speech.Synthesis;

namespace BrailleJP;

public partial class Game1 : Game
{
  private GameState _gameState;

  public static LibLouisLoggingClient LibLouisLoggingClient { get; set; } = new LibLouisLoggingClient();
  // Singleton
  public static Game1 Instance { get; private set; }
  public Random Random { get; set; }
  public BrailleTableParser BrailleParser { get; set; }
  public IMiniGame CurrentPlayingMiniGame { get; set; } = null;
  public Game1()
  {
    _graphics = new GraphicsDeviceManager(this);
    Content.RootDirectory = "Content";
    Random = new Random();
    IsMouseVisible = true;
    _gameState = new GameState();
    Instance = this; // Set the static instance
  }

  private void onExit(object sender, EventArgs e)
  {
    CrossSpeakManager.Instance.Close();
  }
}