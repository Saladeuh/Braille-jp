using AccessibleMyraUI;
using BrailleJP.Content;
using BrailleJP.UI;
using Myra.Graphics2D.UI;
using System.Collections.Generic;
using System.Globalization;

namespace BrailleJP;

public partial class Game1
{
  private Dictionary<CultureInfo, Panel?> _basicPracticePanels;

  public BrailleInputTextField PracticeBrailleInput { get; set; }

  private void CreateBasicPracticeUI(CultureInfo culture)
  {
    if (_basicPracticePanels[culture] != null) return;
    _basicPracticePanels[culture] = new Panel();

    VerticalStackPanel basicPracticeGrid = new()
    {
      Spacing = 20,
      HorizontalAlignment = HorizontalAlignment.Center,
      VerticalAlignment = VerticalAlignment.Center
    };
    Label titleLabel = new()
    {
      Text = string.Format(GameText.Basic_practice_title, culture.DisplayName),
      HorizontalAlignment = HorizontalAlignment.Center
    };
    basicPracticeGrid.Widgets.Add(titleLabel);

    // Space
    basicPracticeGrid.Widgets.Add(new Label { Text = "" });
    PracticeBrailleInput = new BrailleInputTextField();
    basicPracticeGrid.Widgets.Add(PracticeBrailleInput);

    _basicPracticePanels[culture].Widgets.Add(basicPracticeGrid);
    _desktop.FocusedKeyboardWidget = PracticeBrailleInput;
  }
}
