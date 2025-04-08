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


  protected override void Update(GameTime gameTime)
  {
    KeyboardState nativeKeyboardState = Keyboard.GetState();
    List<Keys> allPressedKeys = nativeKeyboardState.GetPressedKeys().ToList();
    lock (_keyLock)
    {
      allPressedKeys.AddRange(_hookPressedKeys);
      if (!_updateProcessed)
      {
        allPressedKeys.AddRange(_keysToProcess);
        allPressedKeys = allPressedKeys.Distinct().ToList();
      }
    }
    KeyboardState currentKeyboardState = new(allPressedKeys.ToArray());
    MouseState currentMouseState = Mouse.GetState();

    _desktop.UpdateInput();
    HandleKeyboardNavigation(currentKeyboardState);
    // quit on escape key
    if (IsKeyPressed(Keys.Escape, currentKeyboardState))
    {
      if (_gameState.CurrentScreen != GameScreen.MainMenu)
      {
        if (_gameState.CurrentScreen == GameScreen.Game && !_gameState.IsPaused)
        {
          // paused in game
          _gameState.IsPaused = true;
          Button pauseButton = _gamePanel.FindChildById("pauseButton") as Button;
          if (pauseButton != null)
          {
            ((Label)pauseButton.Content).Text = "Reprendre";

            // Créer et afficher le menu de pause
            VerticalStackPanel pauseMenu = new()
            {
              Id = "pauseMenu",
              Spacing = 10,
              HorizontalAlignment = HorizontalAlignment.Center,
              VerticalAlignment = VerticalAlignment.Center
            };

            pauseMenu.Widgets.Add(new Label
            {
              Text = "PAUSE",
              TextColor = Color.Yellow
            });

            // Bouton Reprendre
            CustomButton resumeButton = new("Reprendre")
            {
              Id = "resumeButton"
            };
            resumeButton.Click += (sender, args) =>
            {
              _gameState.IsPaused = false;
              ((Label)pauseButton.Content).Text = "Pause";
              Grid grid = _gamePanel.Widgets[0] as Grid;
              grid?.Widgets.Remove(grid.FindChildById("pauseMenu"));
            };
            pauseMenu.Widgets.Add(resumeButton);

            // Bouton Menu Principal
            CustomButton returnToMenuButton = new("Menu Principal")
            {
              Id = "returnButton"
            };
            returnToMenuButton.Click += (sender, args) =>
            {
              SwitchToScreen(GameScreen.MainMenu);
            };
            pauseMenu.Widgets.Add(returnToMenuButton);

            Grid grid = _gamePanel.Widgets[0] as Grid;
            grid?.Widgets.Add(pauseMenu);

            // Donner le focus au premier bouton
            resumeButton.SetKeyboardFocus();
          }
        }
        else
        {
          SwitchToScreen(GameScreen.MainMenu);
        }
      }
      else
      {
        Exit();
      }
    }
    if (_gameState.CurrentScreen == GameScreen.Game && !_gameState.IsPaused)
    {
      UpdateGameLogic(gameTime);
    }

    UpdateUIState();

    _previousKeyboardState = currentKeyboardState;
    _previousMouseState = currentMouseState;
    lock (_keyLock)
    {
      _updateProcessed = true;
      _keysToProcess.Clear(); // Vider les touches à traiter puisqu'elles ont été traitées
    }
    base.Update(gameTime);
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