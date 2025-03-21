using CrossSpeak;
using Myra.Graphics2D.UI;
using System;

namespace AccessibleMyraUI;

public class AccessibleButton : Button
{
  public AccessibleButton(string text, int width = 200, HorizontalAlignment horizontalAlignment = HorizontalAlignment.Center)
  {
    Content = new Label { Text = text };
    Width = width;
    HorizontalAlignment = horizontalAlignment;
    AcceptsKeyboardFocus = true;

    TouchDown += OnAccessibleTouchDown;
    KeyboardFocusChanged += OnAccessibleKeyboardFocusChanged;
  }

  private void OnAccessibleTouchDown(object sender, EventArgs e)
  {
    AnnounceText();
  }

  private void OnAccessibleKeyboardFocusChanged(object sender, EventArgs e)
  {
    if (IsKeyboardFocused)
      AnnounceText();
  }

  private void AnnounceText()
  {
    if (Content is Label label)
    {
      string announcement = string.Format(AccessibilityResources.Button_Focus, label.Text);
      CrossSpeakManager.Instance.Output(announcement);
    }
  }
}