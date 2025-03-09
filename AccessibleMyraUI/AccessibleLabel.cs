using System;
using CrossSpeak;
using Microsoft.Xna.Framework;
using Myra.Graphics2D.UI;

namespace AccessibleMyraUI;

public class AccessibleLabel : Label
{
  public AccessibleLabel(string text, Color? textColor = null)
  {
    Text = text;
    if (textColor.HasValue)
      TextColor = textColor.Value;
    AcceptsKeyboardFocus = true;

    KeyboardFocusChanged += OnAccessibleKeyboardFocusChanged;
  }

  private void OnAccessibleKeyboardFocusChanged(object sender, EventArgs e)
  {
    if (IsKeyboardFocused)
    {
      string announcement = string.Format(AccessibilityResources.Label_Focus, Text);
      CrossSpeakManager.Instance.Speak(announcement);
    }
  }
}
