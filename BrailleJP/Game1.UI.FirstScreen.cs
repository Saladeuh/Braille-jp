using AccessibleMyraUI;
using BrailleJP.Content;
using BrailleJP.UI;
using CrossSpeak;
using Myra.Graphics2D.UI;

namespace BrailleJP;

public partial class Game1
{
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
      Text = GameText.First_screen_title,
      HorizontalAlignment = HorizontalAlignment.Center
    };
    firstScreenGrid.Widgets.Add(titleLabel);

    // Space
    firstScreenGrid.Widgets.Add(new Label { Text = "" });
    var tipsLabel = new AccessibleLabel(GameText.Tips) { Id = "tipsLabel" };
    firstScreenGrid.Widgets.Add(tipsLabel);

    ConfirmButton startButton = new(GameText.First_screen_start)
    {
      Id = "startButton"
    };
    startButton.Click += (s, a) =>
    {
      Save.Flags.EmptySave = false;
      SwitchToScreen(GameScreen.MainMenu);
    };
    firstScreenGrid.Widgets.Add(startButton);

    _firstScreenPanel.Widgets.Add(firstScreenGrid);
    _desktop.FocusedKeyboardWidget = tipsLabel;
  }
}
