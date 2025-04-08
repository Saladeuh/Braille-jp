using AccessibleMyraUI;
using Myra.Graphics2D.UI;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace BrailleJP;

public partial class Game1
{
  private void CreateBrailleTableView(string tableName, CultureInfo culture)
  {
    _brailleTableViewPanel = new Panel();
    foreach (System.Speech.Synthesis.InstalledVoice voice in SpeechSynthesizer.GetInstalledVoices())
    {
      if (voice.Enabled && voice.VoiceInfo.Culture.TwoLetterISOLanguageName == culture.TwoLetterISOLanguageName)
        SpeechSynthesizer.SelectVoice(voice.VoiceInfo.Name);
    }
    VerticalStackPanel tableViewGrid = new()
    {
      Spacing = 10,
      HorizontalAlignment = HorizontalAlignment.Center,
      VerticalAlignment = VerticalAlignment.Center
    };
    Label titleLabel = new()
    {
      Text = tableName,
      HorizontalAlignment = HorizontalAlignment.Center
    };
    tableViewGrid.Widgets.Add(titleLabel);

    // Space
    tableViewGrid.Widgets.Add(new Label { Text = "" });
    List<BrailleEntry> entries = _brailleParser.ParseFile(tableName + ".utb");
    entries.Sort((BrailleEntry e1, BrailleEntry e2) => String.Compare(e1.Characters, e2.Characters, culture, CompareOptions.IgnoreCase));
    foreach (BrailleEntry entry in entries)
    {
      if (entry.Opcode == "letter")
      {
        TableViewEntry label = new(entry);
        tableViewGrid.Widgets.Add(label);
      }
    }
    _brailleTableViewPanel.Widgets.Add(tableViewGrid);
    _desktop.FocusedKeyboardWidget = titleLabel;
  }
}
