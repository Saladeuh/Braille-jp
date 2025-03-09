using System;
using CrossSpeak;
using Myra.Graphics2D.UI;

namespace BrailleJP.AccessibleUI;

public class AccessibleTextBox : TextBox
{
  public AccessibleTextBox(string text = "", int width = 200)
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
    if (string.IsNullOrEmpty(Text))
      announcement = AccessibilityResources.TextBox_Empty;
    else
      announcement = string.Format(AccessibilityResources.TextBox_Focus, Text);

    CrossSpeakManager.Instance.Speak(announcement);
  }
}
