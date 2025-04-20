using CrossSpeak;
using Myra.Graphics2D.UI;
using System;

namespace AccessibleMyraUI;

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

  private void OnAccessibleTextChanged(object sender, EventArgs e)
  {
    if (IsKeyboardFocused)
      AnnounceText();
  }

  private void AnnounceText()
  {
    string announcement;
    if (!string.IsNullOrEmpty(Text))
    {
      announcement = Text;
      CrossSpeakManager.Instance.Output(announcement);
    }
  }
}
