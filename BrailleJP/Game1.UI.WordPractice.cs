using BrailleJP.Content;
using BrailleJP.UI;
using Myra.Graphics2D.UI;
using System.Collections.Generic;
using System.Globalization;

namespace BrailleJP;

public partial class Game1
{
  private readonly Dictionary<CultureInfo, Panel?> _wordPracticePanels;
  
  private void CreateWordPracticeUI(CultureInfo culture)
  {
    if (_wordPracticePanels[culture] != null) return;
    _wordPracticePanels[culture] = new Panel();

    VerticalStackPanel wordPracticeGrid = new()
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
    wordPracticeGrid.Widgets.Add(titleLabel);

    // Space
    wordPracticeGrid.Widgets.Add(new Label { Text = "" });
    PracticeBrailleInput = new BrailleInputTextField();
    wordPracticeGrid.Widgets.Add(PracticeBrailleInput);

    _wordPracticePanels[culture].Widgets.Add(wordPracticeGrid);
    _desktop.FocusedKeyboardWidget = PracticeBrailleInput;
  }
}
