using AccessibleMyraUI;
using Myra.Graphics2D.UI;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace BrailleJP;

public partial class Game1
{
  private Dictionary<CultureInfo, Panel?> _brailleTableViewPanels;
  private void CreateBrailleTableView(CultureInfo culture)
  {
    if (_brailleTableViewPanels[culture] != null) return;
    _brailleTableViewPanels[culture] = new Panel();
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
      Text = culture.Name,
      HorizontalAlignment = HorizontalAlignment.Center
    };
    tableViewGrid.Widgets.Add(titleLabel);

    // Space
    tableViewGrid.Widgets.Add(new Label { Text = "" });
    List<BrailleEntry> entries = _brailleParser.ParseFile(SUPPORTEDBRAILLETABLES[culture] + ".utb");
    
    entries.Sort((BrailleEntry e1, BrailleEntry e2) => String.Compare(e1.Characters, e2.Characters, culture, CompareOptions.None));
    if(culture.IetfLanguageTag=="ja-JP")
      entries.SortByGojuon(e => e.Characters);
    foreach (BrailleEntry entry in entries)
    {
      if (entry.Opcode == "letter")
      {
        TableViewEntry label = new(entry);
        tableViewGrid.Widgets.Add(label);
      }
    }
    _brailleTableViewPanels[culture].Widgets.Add(tableViewGrid);
    _desktop.FocusedKeyboardWidget = titleLabel;
  }
}
