using AccessibleMyraUI;
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
    volumePanel.Widgets.Add(new Label { Text = "Volume:" });

    AccessibleSlider volumeSlider = new(0.8f)
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
    CustomButton backButton = new("Retour au menu")
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
}
