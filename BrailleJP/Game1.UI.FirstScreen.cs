using AccessibleMyraUI;
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
F2 et F3 servent à ajuster le volume de la musique.
Vous pouvez relire ces informations à n’importe quel moment en appuyant sur F1 ou dans le menu principal.";
  private Panel _firstScreenPanel;

  private void CreateFirstScreen()
  {
    _firstScreenPanel = new Panel();

    VerticalStackPanel firstScreenGrid = new()
    {
      Spacing = 20,
      HorizontalAlignment = HorizontalAlignment.Center,
      VerticalAlignment = VerticalAlignment.Center
    };
    Label titleLabel = new()
    {
      Text = "Bienvenue",
      HorizontalAlignment = HorizontalAlignment.Center
    };
    firstScreenGrid.Widgets.Add(titleLabel);

    // Space
    firstScreenGrid.Widgets.Add(new Label { Text = "" });
    var tipsLabel = new AccessibleLabel(TIPS) { Id = "tipsLabel" };
    firstScreenGrid.Widgets.Add(tipsLabel);

    ConfirmButton startButton = new("Commencer")
    {
      Id = "startButton"
    };
    startButton.Click += (s, a) =>
    {
      SwitchToScreen(GameScreen.MainMenu);
    };
    firstScreenGrid.Widgets.Add(startButton);

    _firstScreenPanel.Widgets.Add(firstScreenGrid);
    _desktop.FocusedKeyboardWidget = tipsLabel;
  }
}
