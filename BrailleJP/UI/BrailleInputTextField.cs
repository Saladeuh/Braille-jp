using BrailleJP;
using CrossSpeak;
using Myra.Events;
using Myra.Graphics2D.UI;
using System;

namespace BrailleJP.UI;

public class BrailleInputTextField : TextBox
{
  public BrailleInputTextField(string text = "", int width = 200)
  {
    Text = text;
    Width = width;
    AcceptsKeyboardFocus = true;

    KeyboardFocusChanged += OnAccessibleKeyboardFocusChanged;
    TextChanged += OnAccessibleTextChanged;
  }

  private void OnAccessibleKeyboardFocusChanged(object sender, EventArgs e)
  {
    if (IsKeyboardFocused)
      AnnounceText();
  }

  private void OnAccessibleTextChanged(object sender, ValueChangedEventArgs<string> e)
  {
    if (e.NewValue == string.Empty) return;
    if (Game1.Instance.KeyboardSDFJKL)
    {
      Text = e.OldValue;
      return;
    }
    if (IsKeyboardFocused && !Game1.Instance.KeyboardSDFJKL)
      AnnounceText();
  }

  private void AnnounceText()
  {
    string announcement;
    if (!string.IsNullOrEmpty(Text))
    {
      announcement = Text;
      CrossSpeakManager.Instance.Braille(announcement);
    }
  }
}
