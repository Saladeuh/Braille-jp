using AccessibleMyraUI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrailleJP;

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
    //Game1.Instance.SpeechSynthesizer.Speak(_entry.Characters); 
    Game1.Instance.UIViewScrollSound.Play();
  }
}
