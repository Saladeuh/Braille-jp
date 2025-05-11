using BrailleJP.Content;
using Myra.Graphics2D.UI;
using System.Collections.Generic;
using System.Globalization;

namespace BrailleJP;

public partial class Game1
{
  private readonly Dictionary<CultureInfo, Panel?> _choicePracticePanels;
  private void CreateChoicePracticeUI(CultureInfo culture)
  {
    if (_choicePracticePanels[culture] != null) return;
    _choicePracticePanels[culture] = new Panel();

    VerticalStackPanel choicePracticeGrid = new()
    {
      Spacing = 20,
      HorizontalAlignment = HorizontalAlignment.Center,
      VerticalAlignment = VerticalAlignment.Center
    };
    Label titleLabel = new()
    {
      Text = string.Format(GameText.Choice_practice_title, culture.DisplayName),
      HorizontalAlignment = HorizontalAlignment.Center
    };
    choicePracticeGrid.Widgets.Add(titleLabel);

    // Space
    choicePracticeGrid.Widgets.Add(new Label { Text = "" });
    _choicePracticePanels[culture].Widgets.Add(choicePracticeGrid);
    _desktop.FocusedKeyboardWidget = choicePracticeGrid;
  }
}
