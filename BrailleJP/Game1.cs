using System;
using SharpHook;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Myra;
using Myra.Graphics2D.UI;
using CrossSpeak;
using AccessibleMyraUI;
using Microsoft.Xna.Framework.Media;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using SharpHook.Native;
using System.Collections.Generic;
using System.Globalization;
using System.Speech;
using System.Speech.Synthesis;

namespace BrailleJP;

public class Game1 : Game
{
  private GraphicsDeviceManager _graphics;
  private SpriteBatch _spriteBatch;
  private Desktop _desktop;
  private GameState _gameState;
  private SpeechSynthesizer _speechSynthesizer;
  private Panel _mainMenuPanel;
  private Panel _gamePanel;
  private Panel _settingsPanel;
  private KeyboardState _previousKeyboardState;
  private MouseState _previousMouseState;
  private BrailleTableParser _brailleParser;
  private readonly HashSet<Keys> _hookPressedKeys = new HashSet<Keys>();
  private Song _titleScreenSong;
  private Song _brailleTableViewSong;
  private readonly HashSet<Keys> _keysToProcess = new HashSet<Keys>(); // Nouvelles touches à traiter
  private readonly object _keyLock = new object();

  private bool _updateProcessed = false;
  private Panel _brailleTableViewPanel;

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
    _speechSynthesizer = new SpeechSynthesizer();
    // create hook to get keyboard and simulated keyboard (e.g. screen readers inputs) 
    var hook = new TaskPoolGlobalHook();
    hook.KeyPressed += OnKeyPressed;
    hook.KeyReleased += OnKeyReleased;
    Task.Run(() => hook.Run());
    _previousKeyboardState = Keyboard.GetState();
    _previousMouseState = Mouse.GetState();
    _brailleParser = new BrailleTableParser(@"LibLouis\tables");
  }

  private void OnKeyPressed(object sender, KeyboardHookEventArgs e)
  {
    Keys monogameKey = Utils.ConvertKeyCodeToMonogameKey(e.Data.KeyCode);

    if (monogameKey != Keys.None)
    {
      lock (_keyLock)
      {
        _hookPressedKeys.Add(monogameKey);
        _keysToProcess.Add(monogameKey); // Ajouter aux touches à traiter
        _updateProcessed = false; // Réinitialiser le flag
      }
    }
  }

  private void OnKeyReleased(object sender, KeyboardHookEventArgs e)
  {
    Keys monogameKey = Utils.ConvertKeyCodeToMonogameKey(e.Data.KeyCode);

    if (monogameKey != Keys.None)
    {
      lock (_keyLock)
      {
        _hookPressedKeys.Remove(monogameKey);
        // Do not withdraw from _KeStoprocess - These keys must be treated at least once      }
      }
    }
  }
  protected override void LoadContent()
  {
    _spriteBatch = new SpriteBatch(GraphicsDevice);
    _titleScreenSong = Content.Load<Song>("GoodbyeGeno");
    _brailleTableViewSong = Content.Load<Song>("music/PinnaPark");
    MediaPlayer.IsRepeating = true;
    MyraEnvironment.Game = this;
    _desktop = new Desktop
    {
      HasExternalTextInput = true
    };
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
    var titleLabel = new Label
    {
      Text = "BRAILLE JP",
      HorizontalAlignment = HorizontalAlignment.Center
    };
    mainMenuGrid.Widgets.Add(titleLabel);

    // Space
    mainMenuGrid.Widgets.Add(new Label { Text = "" });

    var playButton = new AccessibleButton("Jouer")
    {
      Id = "playButton"
    };
    playButton.Click += (s, a) =>
    {
      SwitchToScreen(GameScreen.BrailleTableView);
      CrossSpeakManager.Instance.Output("Jeu démarré");
    };
    mainMenuGrid.Widgets.Add(playButton);

    var settingsButton = new AccessibleButton("Paramètres")
    {
      Id = "settingsButton"
    };
    settingsButton.Click += (s, a) =>
    {
      SwitchToScreen(GameScreen.Settings);
      CrossSpeakManager.Instance.Output("Menu des paramètres");
    };
    mainMenuGrid.Widgets.Add(settingsButton);

    var quitButton = new AccessibleButton("Quitter")
    {
      Id = "quitButton"
    };
    quitButton.Click += (s, a) =>
    {
      Exit();
    };
    mainMenuGrid.Widgets.Add(quitButton);

    _mainMenuPanel.Widgets.Add(mainMenuGrid);
    _desktop.FocusedKeyboardWidget = playButton;
  }

  private void CreateBrailleTableView(string tableName, CultureInfo culture)
  {
    _brailleTableViewPanel = new Panel();
    foreach (var voice in _speechSynthesizer.GetInstalledVoices())
    {
      if (voice.Enabled && voice.VoiceInfo.Culture.TwoLetterISOLanguageName == culture.TwoLetterISOLanguageName)
        _speechSynthesizer.SelectVoice(voice.VoiceInfo.Name);
    }
    var tableViewGrid = new VerticalStackPanel
    {
      Spacing = 10,
      HorizontalAlignment = HorizontalAlignment.Center,
      VerticalAlignment = VerticalAlignment.Center
    };
    var titleLabel = new Label
    {
      Text = tableName,
      HorizontalAlignment = HorizontalAlignment.Center
    };
    tableViewGrid.Widgets.Add(titleLabel);

    // Space
    tableViewGrid.Widgets.Add(new Label { Text = "" });
    var entries = _brailleParser.ParseFile(tableName + ".utb");
    entries.Sort((BrailleEntry e1, BrailleEntry e2) => String.Compare(e1.Characters, e2.Characters, culture, CompareOptions.IgnoreSymbols));
    foreach (var entry in entries)
    {
      var label = new AccessibleLabel(entry.ToString());
      label.KeyboardFocusChanged += (s, a) => { _speechSynthesizer.Speak(entry.Characters); };
      tableViewGrid.Widgets.Add(label);
    }
    _brailleTableViewPanel.Widgets.Add(tableViewGrid);
    _desktop.FocusedKeyboardWidget = titleLabel;
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
    var pauseButton = new AccessibleButton("Pause", 100, HorizontalAlignment.Right)
    {
      Id = "pauseButton",
      VerticalAlignment = VerticalAlignment.Top,
      AcceptsKeyboardFocus = false
    };

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
        var resumeButton = new AccessibleButton("Reprendre")
        {
          Id = "resumeButton"
        };
        resumeButton.Click += (sender, args) =>
        {
          _gameState.IsPaused = false;
          ((Label)pauseButton.Content).Text = "Pause";
          gameGrid.Widgets.Remove(gameGrid.FindChildById("pauseMenu"));
        };
        pauseMenu.Widgets.Add(resumeButton);

        // Bouton Menu Principal
        var returnToMenuButton = new AccessibleButton("Menu Principal")
        {
          Id = "returnButton"
        };
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
        var pauseMenu = gameGrid.FindChildById("pauseMenu");
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

    var volumeSlider = new AccessibleSlider(0.8f)
    {
      Id = "volumeSlider",
      AcceptsKeyboardFocus = true
    };

    // Annonce vocale du niveau de volume
    volumeSlider.ValueChanged += (s, a) =>
    {
      CrossSpeakManager.Instance.Output($"Volume: {(int)(volumeSlider.Value * 100)} pourcent");
    };

    volumeSlider.KeyboardFocusChanged += (s, a) =>
    {
      if (volumeSlider.IsKeyboardFocused)
        CrossSpeakManager.Instance.Output($"Slider de volume: {(int)(volumeSlider.Value * 100)} pourcent");
    };

    volumePanel.Widgets.Add(volumeSlider);
    settingsGrid.Widgets.Add(volumePanel);

    // Espace
    settingsGrid.Widgets.Add(new Label { Text = "" });

    // Bouton Retour au menu
    var backButton = new AccessibleButton("Retour au menu")
    {
      Id = "backButton"
    };
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
        var playButton = _mainMenuPanel.FindChildById("playButton");
        playButton?.SetKeyboardFocus();
        MediaPlayer.Play(_titleScreenSong);
        break;
      case GameScreen.BrailleTableView:
        CreateBrailleTableView("ja-jp-comp6", new CultureInfo("ja-Jp"));
        _desktop.Root = _brailleTableViewPanel;
        UpdateUIState();
        MediaPlayer.Play(_brailleTableViewSong);
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
        var volumeSlider = _settingsPanel.FindChildById("volumeSlider");
        volumeSlider?.SetKeyboardFocus();
        UpdateUIState();
        break;
    }
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
    var currentKeyboardState = new KeyboardState(allPressedKeys.ToArray());
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
          var pauseButton = _gamePanel.FindChildById("pauseButton") as Button;
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
            var resumeButton = new AccessibleButton("Reprendre")
            {
              Id = "resumeButton"
            };
            resumeButton.Click += (sender, args) =>
            {
              _gameState.IsPaused = false;
              ((Label)pauseButton.Content).Text = "Pause";
              var grid = _gamePanel.Widgets[0] as Grid;
              grid?.Widgets.Remove(grid.FindChildById("pauseMenu"));
            };
            pauseMenu.Widgets.Add(resumeButton);

            // Bouton Menu Principal
            var returnToMenuButton = new AccessibleButton("Menu Principal")
            {
              Id = "returnButton"
            };
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
      var scoreLabel = _gamePanel.FindChildById("scoreLabel") as Label;
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