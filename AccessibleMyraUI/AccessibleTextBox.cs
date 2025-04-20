using CrossSpeak;
using Myra.Graphics2D.UI;
using System;

namespace AccessibleMyraUI;

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
    string announcement = string.IsNullOrEmpty(Text) ? AccessibilityResources.TextBox_Empty : string.Format(AccessibilityResources.TextBox_Focus, Text);
    CrossSpeakManager.Instance.Output(announcement);
  }
}
