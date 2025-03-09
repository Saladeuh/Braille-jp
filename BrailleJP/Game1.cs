using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Myra;
using Myra.Graphics2D.UI;
using CrossSpeak;
using AssetManagementBase;

namespace BrailleJP;

public class Game1 : Game
{
  private GraphicsDeviceManager _graphics;
  private SpriteBatch _spriteBatch;
  private Desktop _desktop;
  private GameState _gameState;

  // Interfaces utilisateur pour chaque écran
  private Panel _mainMenuPanel;
  private Panel _gamePanel;
  private Panel _settingsPanel;

  // Pour la gestion des entrées clavier
  private KeyboardState _previousKeyboardState;
  private MouseState _previousMouseState;

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

    // Initialiser les états d'entrée
    _previousKeyboardState = Keyboard.GetState();
    _previousMouseState = Mouse.GetState();
  }

  protected override void LoadContent()
  {
    _spriteBatch = new SpriteBatch(GraphicsDevice);
    MyraEnvironment.Game = this;

    // Initialiser l'environnement Myra
    _desktop = new Desktop();

    // Configurer la gestion des entrées texte
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
    // Titre du jeu
    var titleLabel = new Label
    {
      Text = "BRAILLE JP",
      HorizontalAlignment = HorizontalAlignment.Center
    };
    mainMenuGrid.Widgets.Add(titleLabel);

    // Espacement
    mainMenuGrid.Widgets.Add(new Label { Text = "" });

    // Bouton Jouer
    var playButton = new Button
    {
      Id = "playButton",
      Content = new Label { Text = "Jouer" },
      Width = 200,
      HorizontalAlignment = HorizontalAlignment.Center
    };

    // Configuration importante pour la navigation clavier
    playButton.AcceptsKeyboardFocus = true;

    playButton.Click += (s, a) =>
    {
      SwitchToScreen(GameScreen.Game);
      CrossSpeakManager.Instance.Speak("Jeu démarré");
    };

    // Annonce vocale quand le focus arrive sur ce bouton
    playButton.TouchDown += (s, a) => CrossSpeakManager.Instance.Speak("Bouton Jouer");
    playButton.KeyboardFocusChanged += (s, a) =>
    {
      if (playButton.IsKeyboardFocused)
        CrossSpeakManager.Instance.Speak("Bouton Jouer");
    };

    mainMenuGrid.Widgets.Add(playButton);

    // Bouton Paramètres
    var settingsButton = new Button
    {
      Id = "settingsButton",
      Content = new Label { Text = "Paramètres" },
      Width = 200,
      HorizontalAlignment = HorizontalAlignment.Center
    };
    settingsButton.AcceptsKeyboardFocus = true;

    settingsButton.Click += (s, a) =>
    {
      SwitchToScreen(GameScreen.Settings);
      CrossSpeakManager.Instance.Speak("Menu des paramètres");
    };

    // Annonce vocale quand le focus arrive sur ce bouton
    settingsButton.TouchDown += (s, a) => CrossSpeakManager.Instance.Speak("Bouton Paramètres");
    settingsButton.KeyboardFocusChanged += (s, a) =>
    {
      if (settingsButton.IsKeyboardFocused)
        CrossSpeakManager.Instance.Speak("Bouton Paramètres");
    };

    mainMenuGrid.Widgets.Add(settingsButton);

    // Bouton Quitter
    var quitButton = new Button
    {
      Id = "quitButton",
      Content = new Label { Text = "Quitter" },
      Width = 200,
      HorizontalAlignment = HorizontalAlignment.Center
    };
    quitButton.AcceptsKeyboardFocus = true;

    quitButton.Click += (s, a) =>
    {
      Exit();
    };

    // Annonce vocale quand le focus arrive sur ce bouton
    quitButton.TouchDown += (s, a) => CrossSpeakManager.Instance.Speak("Bouton Quitter");
    quitButton.KeyboardFocusChanged += (s, a) =>
    {
      if (quitButton.IsKeyboardFocused)
        CrossSpeakManager.Instance.Speak("Bouton Quitter");
    };

    mainMenuGrid.Widgets.Add(quitButton);

    // Ajouter le grid au panel du menu
    _mainMenuPanel.Widgets.Add(mainMenuGrid);

    // Définir le premier bouton comme ayant le focus par défaut
    _desktop.FocusedKeyboardWidget=playButton;
    //playButton.SetKeyboardFocus();
  }

  private void CreateGameUI()
  {
    _gamePanel = new Panel();

    // Interface du jeu
    var gameGrid = new Grid
    {
      RowSpacing = 8,
      ColumnSpacing = 8
    };

    // Ajouter les éléments d'interface du jeu
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
    var pauseButton = new Button
    {
      Id = "pauseButton",
      Content = new Label { Text = "Pause" },
      Width = 100,
      HorizontalAlignment = HorizontalAlignment.Right,
      VerticalAlignment = VerticalAlignment.Top
    };
    pauseButton.AcceptsKeyboardFocus = true;

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

        var resumeButton = new Button
        {
          Id = "resumeButton",
          Content = new Label { Text = "Reprendre" }
        };
        resumeButton.AcceptsKeyboardFocus = true;

        resumeButton.Click += (sender, args) =>
        {
          _gameState.IsPaused = false;
          ((Label)pauseButton.Content).Text = "Pause";
          gameGrid.Widgets.Remove(gameGrid.FindWidgetById("pauseMenu"));
        };

        // Annonce vocale
        resumeButton.KeyboardFocusChanged += (sender, args) =>
        {
          if (resumeButton.IsKeyboardFocused)
            CrossSpeakManager.Instance.Speak("Bouton Reprendre");
        };

        pauseMenu.Widgets.Add(resumeButton);

        var returnToMenuButton = new Button
        {
          Id = "returnButton",
          Content = new Label { Text = "Menu Principal" }
        };
        returnToMenuButton.AcceptsKeyboardFocus = true;

        returnToMenuButton.Click += (sender, args) =>
        {
          SwitchToScreen(GameScreen.MainMenu);
        };

        // Annonce vocale
        returnToMenuButton.KeyboardFocusChanged += (sender, args) =>
        {
          if (returnToMenuButton.IsKeyboardFocused)
            CrossSpeakManager.Instance.Speak("Bouton Menu Principal");
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

    // Annonce vocale
    pauseButton.KeyboardFocusChanged += (s, a) =>
    {
      if (pauseButton.IsKeyboardFocused)
        CrossSpeakManager.Instance.Speak("Bouton Pause");
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

    // Titre
    settingsGrid.Widgets.Add(new Label
    {
      Text = "PARAMÈTRES",
      TextColor = Color.White,
      HorizontalAlignment = HorizontalAlignment.Center
    });

    // Paramètre de volume
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

    // Bouton Retour
    var backButton = new Button
    {
      Id = "backButton",
      Content = new Label { Text = "Retour au menu" },
      Width = 200,
      HorizontalAlignment = HorizontalAlignment.Center
    };
    backButton.AcceptsKeyboardFocus = true;

    backButton.Click += (s, a) =>
    {
      // Sauvegarder les paramètres ici si nécessaire
      SwitchToScreen(GameScreen.MainMenu);
    };

    backButton.KeyboardFocusChanged += (s, a) =>
    {
      if (backButton.IsKeyboardFocused)
        CrossSpeakManager.Instance.Speak("Bouton Retour au menu");
    };

    settingsGrid.Widgets.Add(backButton);

    _settingsPanel.Widgets.Add(settingsGrid);

    // Donner le focus au slider au démarrage
    //volumeSlider.SetKeyboardFocus();
  }

  private void SwitchToScreen(GameScreen screen)
  {
    _gameState.CurrentScreen = screen;

    // Définir le widget racine en fonction de l'écran actuel
    switch (screen)
    {
      case GameScreen.MainMenu:
        _desktop.Root = _mainMenuPanel;
        // Donner le focus au premier bouton du menu
        var playButton = _mainMenuPanel.FindWidgetById("playButton");
        playButton?.SetKeyboardFocus();
        break;
      case GameScreen.Game:
        _desktop.Root = _gamePanel;
        // Réinitialiser l'état du jeu ici si nécessaire
        _gameState.IsPaused = false;
        _gameState.Score = 0;
        UpdateUIState();
        // Donner le focus au bouton de pause
        var pauseButton = _gamePanel.FindWidgetById("pauseButton");
        pauseButton?.SetKeyboardFocus();
        break;
      case GameScreen.Settings:
        _desktop.Root = _settingsPanel;
        // Donner le focus au slider de volume
        var volumeSlider = _settingsPanel.FindWidgetById("volumeSlider");
        volumeSlider?.SetKeyboardFocus();
        break;
    }
  }

  protected override void Update(GameTime gameTime)
  {
    var currentKeyboardState = Keyboard.GetState();
    var currentMouseState = Mouse.GetState();

    // Mettre à jour l'état du desktop avec les entrées souris
    _desktop.UpdateInput();

    // Gestion de la navigation au clavier personnalisée
    HandleKeyboardNavigation(currentKeyboardState);

    // Vérifier la touche Échap pour le menu principal
    if (IsKeyPressed(Keys.Escape, currentKeyboardState))
    {
      if (_gameState.CurrentScreen != GameScreen.MainMenu)
      {
        if (_gameState.CurrentScreen == GameScreen.Game && !_gameState.IsPaused)
        {
          // Mettre en pause plutôt que de revenir directement au menu
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

            var resumeButton = new Button
            {
              Id = "resumeButton",
              Content = new Label { Text = "Reprendre" }
            };
            resumeButton.AcceptsKeyboardFocus = true;

            resumeButton.Click += (sender, args) =>
            {
              _gameState.IsPaused = false;
              ((Label)pauseButton.Content).Text = "Pause";
              var grid = _gamePanel.Widgets[0] as Grid;
              grid?.Widgets.Remove(grid.FindWidgetById("pauseMenu"));
            };

            pauseMenu.Widgets.Add(resumeButton);

            var returnToMenuButton = new Button
            {
              Id = "returnButton",
              Content = new Label { Text = "Menu Principal" }
            };
            returnToMenuButton.AcceptsKeyboardFocus = true;

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

    // Mettre à jour l'interface utilisateur
    UpdateUIState();

    // Mettre à jour les états d'entrée précédents
    _previousKeyboardState = currentKeyboardState;
    _previousMouseState = currentMouseState;

    base.Update(gameTime);
  }

  private void HandleKeyboardNavigation(KeyboardState currentKeyboardState)
  {
    // Gestion des touches de navigation (flèches, Tab, Entrée)
    if (IsKeyPressed(Keys.Down, currentKeyboardState) || IsKeyPressed(Keys.Right, currentKeyboardState))
    {
      _desktop.FocusNext();
    }

    if (IsKeyPressed(Keys.Up, currentKeyboardState) || IsKeyPressed(Keys.Left, currentKeyboardState))
    {
      _desktop.FocusPrevious();
    }
    // Simuler un clic avec la touche Entrée sur l'élément ayant le focus
    if (IsKeyPressed(Keys.Enter, currentKeyboardState))
    {
      Widget focused = _desktop.FocusedKeyboardWidget;
      if (focused is Button button)
      {
        // Simuler un clic sur le bouton
        button.DoClick();
      }
      else if (focused is HorizontalSlider slider)
      {
        // Pour les sliders, on pourrait ajuster la valeur
        // Mais généralement les flèches gauche/droite font déjà cela
      }
    }
  }

  // Fonction d'aide pour détecter si une touche vient d'être pressée
  private bool IsKeyPressed(Keys key, KeyboardState currentKeyboardState)
  {
    return currentKeyboardState.IsKeyDown(key) && !_previousKeyboardState.IsKeyDown(key);
  }

  private void UpdateGameLogic(GameTime gameTime)
  {
    // Exemple : incrémente le score avec le temps
    if (gameTime.TotalGameTime.Milliseconds % 1000 == 0)
    {
      _gameState.AddPoints(1);
    }

    // Autre logique de jeu ici
    // Déplacement des personnages, détection de collision, etc.
  }

  private void UpdateUIState()
  {
    // Mettre à jour l'interface en fonction de l'état du jeu
    if (_gameState.CurrentScreen == GameScreen.Game)
    {
      var scoreLabel = _gamePanel.FindWidgetById("scoreLabel") as Label;
      if (scoreLabel != null)
      {
        scoreLabel.Text = $"Score: {_gameState.Score}";
      }

      // Autres mises à jour UI liées au jeu
    }
  }

  protected override void Draw(GameTime gameTime)
  {
    GraphicsDevice.Clear(Color.Black);

    // Dessiner les éléments de jeu si nécessaire
    if (_gameState.CurrentScreen == GameScreen.Game && !_gameState.IsPaused)
    {
      _spriteBatch.Begin();
      // Dessiner les éléments de jeu ici
      _spriteBatch.End();
    }

    // Rendre l'interface Myra
    _desktop.Render();

    base.Draw(gameTime);
  }

  private void onExit(object sender, EventArgs e)
  {
    CrossSpeakManager.Instance.Close();
  }
}
