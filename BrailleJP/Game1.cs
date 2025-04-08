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
  private BrailleTableParser _brailleParser;
  private SpeechSynthesizer speechSynthesizer = new();

  public static LibLouisLoggingClient LibLouisLoggingClient { get; set; } = new LibLouisLoggingClient();
  // Singleton
  public static Game1 Instance { get; private set; }
  public SpeechSynthesizer SpeechSynthesizer { get => speechSynthesizer; private set => speechSynthesizer = value; }
  public Game1()
  {
    _graphics = new GraphicsDeviceManager(this);
    Content.RootDirectory = "Content";
    IsMouseVisible = true;
    _gameState = new GameState();
    Instance = this; // Set the static instance
  }

  private void UpdateGameLogic(GameTime gameTime)
  {
    if (gameTime.TotalGameTime.Milliseconds % 1000 == 0)
    {
      _gameState.AddPoints(1);
    }
  }

  private void onExit(object sender, EventArgs e)
  {
    CrossSpeakManager.Instance.Close();
  }
}