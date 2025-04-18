﻿using BrailleJP.MiniGames;
using CrossSpeak;
using Microsoft.Xna.Framework;
using SharpLouis;
using System;

namespace BrailleJP;

public partial class Game1 : Game
{
  private GameState _gameState;

  public static LibLouisLoggingClient LibLouisLoggingClient { get; set; } = new LibLouisLoggingClient();
  // Singleton
  public static Game1 Instance { get; private set; }
  public Random Random { get; set; }
  public Wrapper InputBrailleTranslator { get; set; }
  public BrailleTableParser BrailleParser { get; set; }
  public IMiniGame CurrentPlayingMiniGame { get; set; } = null;
  public Game1()
  {
    _graphics = new GraphicsDeviceManager(this);
    Content.RootDirectory = "Content";
    Random = new Random();
    InputBrailleTranslator = SharpLouis.Wrapper.Create("fr-bfu-comp8.utb", Game1.LibLouisLoggingClient);
    IsMouseVisible = true;
    _gameState = new GameState();
    BrailleParser = new BrailleTableParser(@"LibLouis\tables");
    Instance = this; // Set the static instance
  }

  private void onExit(object sender, EventArgs e)
  {
    CrossSpeakManager.Instance.Close();
  }
}