using BrailleJP.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Myra.Graphics2D.UI;
using System.Globalization;
using System.Linq;

namespace BrailleJP;

public partial class Game1
{
  private GraphicsDeviceManager _graphics;
  private SpriteBatch _spriteBatch;
  private Desktop _desktop;
  private Panel _mainMenuPanel;
  private Panel _gamePanel;
  private Panel _settingsPanel;
  private void SwitchToScreen(GameScreen screen)
  {
    speechSynthesizer.SpeakAsyncCancelAll();
    _gameState.CurrentScreen = screen;
    CultureInfo culture = SUPPORTEDBRAILLETABLES.Keys.First();
    switch (screen)
    {
      case GameScreen.MainMenu:
        _desktop.Root = _mainMenuPanel;
        Widget playButton = _mainMenuPanel.FindChildById("playButton");
        playButton?.SetKeyboardFocus();
        if (MediaPlayer.Queue.ActiveSong != _titleScreenSong)
          MediaPlayer.Play(_titleScreenSong);
        break;
      case GameScreen.BrailleTableView:
        CreateBrailleTableView(culture);
        _desktop.Root = _brailleTableViewPanels[culture];
        UpdateUIState();
        MediaPlayer.Play(_brailleTableViewSong);
        break;
      case GameScreen.BasicPraticce:
        CreateBasicPractice(culture);
        _desktop.Root = _basicPracticePanels[culture];
        UpdateUIState();
        //MediaPlayer.Play(_brailleTableViewSong);
        _desktop.Root = _basicPracticePanels[culture];
        // Réinitialiser l'état du jeu ici si nécessaire
        _gameState.IsPaused = false;
        _gameState.Score = 0;
        UpdateUIState();
        break;
      case GameScreen.Settings:
        _desktop.Root = _settingsPanel;
        Widget volumeSlider = _settingsPanel.FindChildById("volumeSlider");
        volumeSlider?.SetKeyboardFocus();
        UpdateUIState();
        break;
    }
  }

  private void UpdateUIState()
  {
    if (_gameState.CurrentScreen == GameScreen.BasicPraticce)
    {
      Label scoreLabel = _gamePanel.FindChildById("scoreLabel") as Label;
      if (scoreLabel != null)
      {
        scoreLabel.Text = $"Score: {_gameState.Score}";
      }
    }
  }
  private void CreateGameUI()
  {
    _gamePanel = new Panel();

    Grid gameGrid = new()
    {
      RowSpacing = 8,
      ColumnSpacing = 8
    };

    Label scoreLabel = new()
    {
      Id = "scoreLabel",
      Text = "Score: 0",
      TextColor = Color.White,
      HorizontalAlignment = HorizontalAlignment.Left,
      VerticalAlignment = VerticalAlignment.Top
    };
    gameGrid.Widgets.Add(scoreLabel);

    // Bouton de pause
    ConfirmButton pauseButton = new("Pause", 100, HorizontalAlignment.Right)
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
        ConfirmButton resumeButton = new("Reprendre")
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
        ConfirmButton returnToMenuButton = new("Menu Principal")
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
        Widget pauseMenu = gameGrid.FindChildById("pauseMenu");
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

  protected override void Draw(GameTime gameTime)
  {
    GraphicsDevice.Clear(Color.Black);

    if (_gameState.CurrentScreen == GameScreen.BasicPraticce && !_gameState.IsPaused)
    {
      _spriteBatch.Begin();
      _spriteBatch.End();
    }

    // Make myra interface
    _desktop.Render();

    base.Draw(gameTime);
  }
}
