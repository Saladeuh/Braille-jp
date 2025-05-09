using BrailleJP.UI;
using CrossSpeak;
using Myra.Graphics2D.UI;

namespace BrailleJP;

public partial class Game1
{
  private static readonly string TIPS = @"Ce jeu à pour but de vous faire découvrir et pratiquer le Braille japonais. 
Pour cela, vous aurez besoin d’utiliser une plage Braille, ainsi qu’un lecteur d’écran la supportant. Si vous arriver à lire ce texte en Braille, tout doit être bon. 
Voici un aperçu des commandes qui vous seront utiles :
Les flèches vous serve à naviguer dans les menus, entrée à sélectionner, échappe à quitter.

Vous pouvez relire ces informations à n’importe quel moment en appuyant sur F1 ou dans le menu principal.";
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

    ConfirmButton tableViewButton = new("Découvrir la table Braille")
    {
      Id = "playButton"
    };
    tableViewButton.Click += (s, a) =>
    {
      SwitchToScreen(GameScreen.BrailleTableView);
    };
    mainMenuGrid.Widgets.Add(tableViewButton);

    ConfirmButton choicePracticeButton = new("Entraînement : choix multiple")
    {
      Id = "choicePracticeButton"
    };
    choicePracticeButton.Click += (s, a) =>
    {
      SwitchToScreen(GameScreen.ChoicePraticce);
    };
    mainMenuGrid.Widgets.Add(choicePracticeButton);

    ConfirmButton basicPracticeButton = new("Entraînement : Frapp-tappe")
    {
      Id = "basicPracticeButton"
    };
    basicPracticeButton.Click += (s, a) =>
    {
      SwitchToScreen(GameScreen.BasicPraticce);
    };
    mainMenuGrid.Widgets.Add(basicPracticeButton);
#if DEBUG
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
#endif
    ConfirmButton tipsButton = new("Relire les explications")
    {
      Id = "tipsButton"
    };
    tipsButton.Click += (s, a) =>
    {
      CrossSpeakManager.Instance.Output(TIPS);
    };

    mainMenuGrid.Widgets.Add(tipsButton);

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
    _desktop.FocusedKeyboardWidget = tableViewButton;
  }
}
