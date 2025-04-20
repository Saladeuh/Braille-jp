using AccessibleMyraUI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System;

namespace BrailleJP.UI;

internal class TableViewEntry : AccessibleLabel
{
  private readonly SoundEffect _scrollSound;
  private readonly BrailleEntry _entry;

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
