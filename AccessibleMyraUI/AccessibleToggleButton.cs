using CrossSpeak;
using Myra.Graphics2D.UI;
using System;

namespace AccessibleMyraUI;

public class AccessibleToggleButton : Button
{
  private readonly bool _isToggled;
  public event EventHandler ToggledChanged;

  public AccessibleToggleButton(string text, bool isToggled = false, int width = 200)
  {
    Content = new Label { Text = text };
    _isToggled = isToggled;
    Width = width;
    AcceptsKeyboardFocus = true;

    TouchDown += OnAccessibleTouchDown;
    KeyboardFocusChanged += OnAccessibleKeyboardFocusChanged;

  }

  private void OnAccessibleTouchDown(object sender, EventArgs e)
  {
    Enabled = !Enabled;
    AnnounceState();
  }

  private void OnAccessibleKeyboardFocusChanged(object sender, EventArgs e)
  {
    if (IsKeyboardFocused)
      AnnounceState();
  }

  private void AnnounceState()
  {
    if (Content is Label label)
    {
      string resourceKey = Enabled ?
          AccessibilityResources.ToggleButton_On :
          AccessibilityResources.ToggleButton_Off;

      string announcement = string.Format(resourceKey, label.Text);
      CrossSpeakManager.Instance.Output(announcement);
    }
  }
}