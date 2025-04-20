using BrailleJP.UI;
using CrossSpeak;
using Myra.Graphics2D.UI;

namespace BrailleJP;

public partial class Game1
{
  private void CreateMainMenu()
  {
    _mainMenuPanel = new Panel();

    VerticalStackPanel mainMenuGrid = new()
    {
      Spacing = 20,
      HorizontalAlignment = HorizontalAlignment.Center,
      VerticalAlignment = VerticalAlignment.Center
    };
    Label titleLabel = new()
    {
      Text = "BRAILLE JP",
      HorizontalAlignment = HorizontalAlignment.Center
    };
    mainMenuGrid.Widgets.Add(titleLabel);

    // Space
    mainMenuGrid.Widgets.Add(new Label { Text = "" });

    ConfirmButton playButton = new("Jouer")
    {
      Id = "playButton"
    };
    playButton.Click += (s, a) =>
    {
      SwitchToScreen(GameScreen.BrailleTableView);
      CrossSpeakManager.Instance.Output("Jeu démarré");
    };
    mainMenuGrid.Widgets.Add(playButton);
    ConfirmButton basicPracticeButton = new("Entraînement")
    {
      Id = "basicPracticeButton"
    };
    basicPracticeButton.Click += (s, a) =>
    {
      SwitchToScreen(GameScreen.BasicPraticce);
    };
    mainMenuGrid.Widgets.Add(basicPracticeButton);

    ConfirmButton choicePracticeButton = new("Choix")
    {
      Id = "choicePracticeButton"
    };
    choicePracticeButton.Click += (s, a) =>
    {
      SwitchToScreen(GameScreen.ChoicePraticce);
    };
    mainMenuGrid.Widgets.Add(choicePracticeButton);

    ConfirmButton settingsButton = new("Paramètres")
    {
      Id = "settingsButton"
    };
    settingsButton.Click += (s, a) =>
    {
      SwitchToScreen(GameScreen.Settings);
      CrossSpeakManager.Instance.Output("Menu des paramètres");
    };
    mainMenuGrid.Widgets.Add(settingsButton);

    BackButton quitButton = new("Quitter")
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
}
