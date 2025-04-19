using AccessibleMyraUI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrailleJP.UI;

internal class TableViewEntry : AccessibleLabel
{
  private SoundEffect _scrollSound;
  private BrailleEntry _entry;

  public TableViewEntry(BrailleEntry brailleEntry, Color? textColor = null) : base(brailleEntry.ToString(), textColor)
  {
    _entry = brailleEntry;
    KeyboardFocusChanged += OnScroll;
  }


  private void OnScroll(object sender, EventArgs e)
  {
    if (IsKeyboardFocused)
    {
      //if (Game1.Instance.SpeechSynthesizer.State != System.Speech.Synthesis.SynthesizerState.Speaking)
      _entry.Voice.Play();
      Game1.Instance.UIViewScrollSound.Play();
    }
  }
}
