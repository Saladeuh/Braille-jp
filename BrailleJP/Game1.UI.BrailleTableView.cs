using LinguaBraille.Content;
using LinguaBraille;
using LinguaBraille.UI;
using Myra.Graphics2D.UI;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace BrailleJP;

public partial class Game1
{
  private Dictionary<CultureInfo, Panel?> _brailleTableViewPanels;
  private void CreateBrailleTableView(CultureInfo culture)
  {
    if (_brailleTableViewPanels[culture] != null) return;
    _brailleTableViewPanels[culture] = new Panel();
    VerticalStackPanel tableViewGrid = new()
    {
      Spacing = 10,
      HorizontalAlignment = HorizontalAlignment.Center,
      VerticalAlignment = VerticalAlignment.Center
    };
    Label titleLabel = new()
    {
      Text = String.Format(GameText.Braille_tableview_title, culture.DisplayName),
      HorizontalAlignment = HorizontalAlignment.Center
    };
    tableViewGrid.Widgets.Add(titleLabel);

    // Space
    tableViewGrid.Widgets.Add(new Label { Text = "" });


    var entries = BrailleTables[SUPPORTEDBRAILLETABLES[culture]];
    entries.Sort((e1, e2) => String.Compare(e1.Characters, e2.Characters, culture, CompareOptions.None));
    if (culture.IetfLanguageTag == "ja-JP")
      entries.SortByGojuon(e => e.Characters);
    foreach (BrailleEntry entry in entries)
    {
      if (entry.IsLowercaseLetter())
      {
        TableViewEntry label = new(entry);
        tableViewGrid.Widgets.Add(label);
      }
    }
    _brailleTableViewPanels[culture].Widgets.Add(tableViewGrid);
    _desktop.FocusedKeyboardWidget = titleLabel;
  }
}
