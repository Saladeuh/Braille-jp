using AccessibleMyraUI;
using BrailleJP.UI;
using CrossSpeak;
using Microsoft.Xna.Framework;
using Myra.Graphics2D.UI;

namespace BrailleJP;

public partial class Game1
{
  private void CreateSettingsUI()
  {
    _settingsPanel = new Panel();

    VerticalStackPanel settingsGrid = new()
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

    HorizontalStackPanel volumePanel = new() { Spacing = 5 };
    volumePanel.Widgets.Add(new AccessibleLabel("Volume:"));

    AccessibleSlider volumeSlider = new(0.8f)
    {
      Id = "volumeSlider",
    };
    // Annonce vocale du niveau de volume
    volumeSlider.ValueChanged += (_, _) =>
    {
      CrossSpeakManager.Instance.Output($"Volume: {(int)(volumeSlider.Value * 100)} pourcent");
    };

    volumePanel.Widgets.Add(volumeSlider);
    settingsGrid.Widgets.Add(volumePanel);

    // Espace
    settingsGrid.Widgets.Add(new Label { Text = "" });

    // Bouton Retour au menu
    BackButton backButton = new("Retour au menu")
    {
      Id = "backButton"
    };
    backButton.Click += (_, _) =>
    {
      // TODO save settings
      SwitchToScreen(GameScreen.MainMenu);
    };
    settingsGrid.Widgets.Add(backButton);

    _settingsPanel.Widgets.Add(settingsGrid);
  }
}
