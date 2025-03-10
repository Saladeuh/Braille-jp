using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Myra;
using Myra.Graphics2D.UI;
using CrossSpeak;
using AssetManagementBase;
using AccessibleMyraUI;
using Microsoft.Xna.Framework.Media;

namespace BrailleJP;

public class Game1 : Game
{
  private GraphicsDeviceManager _graphics;
  private SpriteBatch _spriteBatch;
  private Desktop _desktop;
  private GameState _gameState;

  private Panel _mainMenuPanel;
  private Panel _gamePanel;
  private Panel _settingsPanel;
  private KeyboardState _previousKeyboardState;
  private MouseState _previousMouseState;

  private Song titleScreenSong;
  public Game1()
  {
    _graphics = new GraphicsDeviceManager(this);
    Content.RootDirectory = "Content";
    IsMouseVisible = true;
    _gameState = new GameState();
  }

  protected override void Initialize()
  {
    base.Initialize();
    this.Exiting += onExit;
    CrossSpeakManager.Instance.Initialize();
    _previousKeyboardState = Keyboard.GetState();
    _previousMouseState = Mouse.GetState();
  }

  protected override void LoadContent()
  {
    _spriteBatch = new SpriteBatch(GraphicsDevice);
    titleScreenSong = Content.Load<Song>("GoodbyeGeno");
    MediaPlayer.Play(titleScreenSong);
    MediaPlayer.IsRepeating = true;
    MyraEnvironment.Game = this;
    _desktop = new Desktop();

    _desktop.HasExternalTextInput = true;
    Window.TextInput += (s, a) =>
    {
      _desktop.OnChar(a.Character);
    };

    CreateMainMenu();
    CreateGameUI();
    CreateSettingsUI();
    SwitchToScreen(GameScreen.MainMenu);
  }

  private void CreateMainMenu()
  {
    _mainMenuPanel = new Panel();

    var mainMenuGrid = new VerticalStackPanel
    {
      Spacing = 20,
      HorizontalAlignment = HorizontalAlignment.Center,
      VerticalAlignment = VerticalAlignment.Center
    };

    // Title
    var titleLabel = new Label
    {
      Text = "BRAILLE JP",
      HorizontalAlignment = HorizontalAlignment.Center
    };
    mainMenuGrid.Widgets.Add(titleLabel);

    // Space
    mainMenuGrid.Widgets.Add(new Label { Text = "" });

    // Bouton Jouer
    var playButton = new AccessibleButton("Jouer");
    playButton.Id = "playButton";
    playButton.Click += (s, a) =>
    {
      SwitchToScreen(GameScreen.Game);
      CrossSpeakManager.Instance.Speak("Jeu démarré");
    };
    mainMenuGrid.Widgets.Add(playButton);

    // Bouton Paramètres
    var settingsButton = new AccessibleButton("Paramètres");
    settingsButton.Id = "settingsButton";
    settingsButton.Click += (s, a) =>
    {
      SwitchToScreen(GameScreen.Settings);
      CrossSpeakManager.Instance.Speak("Menu des paramètres");
    };
    mainMenuGrid.Widgets.Add(settingsButton);

    // Bouton Quitter
    var quitButton = new AccessibleButton("Quitter");
    quitButton.Id = "quitButton";
    quitButton.Click += (s, a) =>
    {
      Exit();
    };
    mainMenuGrid.Widgets.Add(quitButton);

    _mainMenuPanel.Widgets.Add(mainMenuGrid);
    _desktop.FocusedKeyboardWidget = playButton;
  }

  private void CreateGameUI()
  {
    _gamePanel = new Panel();

    var gameGrid = new Grid
    {
      RowSpacing = 8,
      ColumnSpacing = 8
    };

    var scoreLabel = new Label
    {
      Id = "scoreLabel",
      Text = "Score: 0",
      TextColor = Color.White,
      HorizontalAlignment = HorizontalAlignment.Left,
      VerticalAlignment = VerticalAlignment.Top
    };
    gameGrid.Widgets.Add(scoreLabel);

    // Bouton de pause
    var pauseButton = new AccessibleButton("Pause", 100, HorizontalAlignment.Right);
    pauseButton.Id = "pauseButton";
    pauseButton.VerticalAlignment = VerticalAlignment.Top;
    pauseButton.AcceptsKeyboardFocus = false;

    pauseButton.Click += (s, a) =>
    {
      _gameState.IsPaused = !_gameState.IsPaused;
      ((Label)pauseButton.Content).Text = _gameState.IsPaused ? "Reprendre" : "Pause";

      if (_gameState.IsPaused)
      {
        var pauseMenu = new VerticalStackPanel
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
        var resumeButton = new AccessibleButton("Reprendre");
        resumeButton.Id = "resumeButton";
        resumeButton.Click += (sender, args) =>
        {
          _gameState.IsPaused = false;
          ((Label)pauseButton.Content).Text = "Pause";
          gameGrid.Widgets.Remove(gameGrid.FindWidgetById("pauseMenu"));
        };
        pauseMenu.Widgets.Add(resumeButton);

        // Bouton Menu Principal
        var returnToMenuButton = new AccessibleButton("Menu Principal");
        returnToMenuButton.Id = "returnButton";
        returnToMenuButton.Click += (sender, args) =>
        {
          SwitchToScreen(GameScreen.MainMenu);
        };
        pauseMenu.Widgets.Add(returnToMenuButton);

        gameGrid.Widgets.Add(pauseMenu);

        // Donner le focus au premier bouton du menu de pause
        resumeButton.SetKeyboardFocus();
      }
      else
      {
        var pauseMenu = gameGrid.FindWidgetById("pauseMenu");
        if (pauseMenu != null)
        {
          gameGrid.Widgets.Remove(pauseMenu);
        }

        // Redonner le focus au bouton de pause
        pauseButton.SetKeyboardFocus();
      }
    };

    gameGrid.Widgets.Add(pauseButton);

    _gamePanel.Widgets.Add(gameGrid);
  }

  private void CreateSettingsUI()
  {
    _settingsPanel = new Panel();

    var settingsGrid = new VerticalStackPanel
    {
      Spacing = 10,
      HorizontalAlignment = HorizontalAlignment.Center,
      VerticalAlignment = VerticalAlignment.Center
    };

    settingsGrid.Widgets.Add(new Label
    {
      Text = "PARAMÈTRES",
      TextColor = Color.White,
      HorizontalAlignment = HorizontalAlignment.Center
    });

    var volumePanel = new HorizontalStackPanel { Spacing = 5 };
    volumePanel.Widgets.Add(new Label { Text = "Volume:" });

    var volumeSlider = new HorizontalSlider
    {
      Id = "volumeSlider",
      Width = 200,
      Value = 0.8f
    };
    volumeSlider.AcceptsKeyboardFocus = true;

    // Annonce vocale du niveau de volume
    volumeSlider.ValueChanged += (s, a) =>
    {
      CrossSpeakManager.Instance.Speak($"Volume: {(int)(volumeSlider.Value * 100)} pourcent");
    };

    volumeSlider.KeyboardFocusChanged += (s, a) =>
    {
      if (volumeSlider.IsKeyboardFocused)
        CrossSpeakManager.Instance.Speak($"Slider de volume: {(int)(volumeSlider.Value * 100)} pourcent");
    };

    volumePanel.Widgets.Add(volumeSlider);
    settingsGrid.Widgets.Add(volumePanel);

    // Espace
    settingsGrid.Widgets.Add(new Label { Text = "" });

    // Bouton Retour au menu
    var backButton = new AccessibleButton("Retour au menu");
    backButton.Id = "backButton";
    backButton.Click += (s, a) =>
    {
      // TODO save settings
      SwitchToScreen(GameScreen.MainMenu);
    };
    settingsGrid.Widgets.Add(backButton);

    _settingsPanel.Widgets.Add(settingsGrid);
  }

  private void SwitchToScreen(GameScreen screen)
  {
    _gameState.CurrentScreen = screen;

    switch (screen)
    {
      case GameScreen.MainMenu:
        _desktop.Root = _mainMenuPanel;
        var playButton = _mainMenuPanel.FindWidgetById("playButton");
        playButton?.SetKeyboardFocus();
        break;
      case GameScreen.Game:
        _desktop.Root = _gamePanel;
        // Réinitialiser l'état du jeu ici si nécessaire
        _gameState.IsPaused = false;
        _gameState.Score = 0;
        UpdateUIState();
        break;
      case GameScreen.Settings:
        _desktop.Root = _settingsPanel;
        var volumeSlider = _settingsPanel.FindWidgetById("volumeSlider");
        volumeSlider?.SetKeyboardFocus();
        break;
    }
  }

  protected override void Update(GameTime gameTime)
  {
    var currentKeyboardState = Keyboard.GetState();
    var currentMouseState = Mouse.GetState();

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
          var pauseButton = _gamePanel.FindWidgetById("pauseButton") as Button;
          if (pauseButton != null)
          {
            ((Label)pauseButton.Content).Text = "Reprendre";

            // Créer et afficher le menu de pause
            var pauseMenu = new VerticalStackPanel
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
            var resumeButton = new AccessibleButton("Reprendre");
            resumeButton.Id = "resumeButton";
            resumeButton.Click += (sender, args) =>
            {
              _gameState.IsPaused = false;
              ((Label)pauseButton.Content).Text = "Pause";
              var grid = _gamePanel.Widgets[0] as Grid;
              grid?.Widgets.Remove(grid.FindWidgetById("pauseMenu"));
            };
            pauseMenu.Widgets.Add(resumeButton);

            // Bouton Menu Principal
            var returnToMenuButton = new AccessibleButton("Menu Principal");
            returnToMenuButton.Id = "returnButton";
            returnToMenuButton.Click += (sender, args) =>
            {
              SwitchToScreen(GameScreen.MainMenu);
            };
            pauseMenu.Widgets.Add(returnToMenuButton);

            var grid = _gamePanel.Widgets[0] as Grid;
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

    // Ne mettre à jour la logique de jeu que si on est en écran de jeu et non en pause
    if (_gameState.CurrentScreen == GameScreen.Game && !_gameState.IsPaused)
    {
      UpdateGameLogic(gameTime);
    }

    UpdateUIState();

    _previousKeyboardState = currentKeyboardState;
    _previousMouseState = currentMouseState;

    base.Update(gameTime);
  }

  private void HandleKeyboardNavigation(KeyboardState currentKeyboardState)
  {
    // navigation keys (arrows, tab, entrance)
    if (IsKeyPressed(Keys.Down, currentKeyboardState) || IsKeyPressed(Keys.Right, currentKeyboardState))
    {
      _desktop.FocusNext();
    }

    if (IsKeyPressed(Keys.Up, currentKeyboardState) || IsKeyPressed(Keys.Left, currentKeyboardState))
    {
      _desktop.FocusPrevious();
    }
    if (IsKeyPressed(Keys.Enter, currentKeyboardState))
    {
      Widget focused = _desktop.FocusedKeyboardWidget;
      if (focused is Button button)
      {
        button.DoClick();
      }
    }
  }

  private bool IsKeyPressed(Keys key, KeyboardState currentKeyboardState)
  {
    return currentKeyboardState.IsKeyDown(key) && !_previousKeyboardState.IsKeyDown(key);
  }

  private void UpdateGameLogic(GameTime gameTime)
  {
    if (gameTime.TotalGameTime.Milliseconds % 1000 == 0)
    {
      _gameState.AddPoints(1);
    }
  }

  private void UpdateUIState()
  {
    if (_gameState.CurrentScreen == GameScreen.Game)
    {
      var scoreLabel = _gamePanel.FindWidgetById("scoreLabel") as Label;
      if (scoreLabel != null)
      {
        scoreLabel.Text = $"Score: {_gameState.Score}";
      }
    }
  }

  protected override void Draw(GameTime gameTime)
  {
    GraphicsDevice.Clear(Color.Black);

    if (_gameState.CurrentScreen == GameScreen.Game && !_gameState.IsPaused)
    {
      _spriteBatch.Begin();
      _spriteBatch.End();
    }

    // Make myra interface
    _desktop.Render();

    base.Draw(gameTime);
  }

  private void onExit(object sender, EventArgs e)
  {
    CrossSpeakManager.Instance.Close();
  }
}