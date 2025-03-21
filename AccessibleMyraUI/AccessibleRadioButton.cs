using System;
using CrossSpeak;
using Myra.Graphics2D.UI;

namespace AccessibleMyraUI;

public class AccessibleRadioButton : RadioButton
{
  public AccessibleRadioButton(string text, bool isChecked = false)
  {
    Content = new Label { Text = text };
    Enabled = isChecked;
    AcceptsKeyboardFocus = true;

    KeyboardFocusChanged += OnAccessibleKeyboardFocusChanged;
    EnabledChanged += OnAccessibleCheckChanged;
  }

  private void OnAccessibleKeyboardFocusChanged(object sender, EventArgs e)
  {
    if (IsKeyboardFocused)
      AnnounceState();
  }

  private void OnAccessibleCheckChanged(object sender, EventArgs e)
  {
    if (Enabled)
      AnnounceState();
  }

  private void AnnounceState()
  {
    string resourceKey = Enabled ?
        AccessibilityResources.RadioButton_Selected :
        AccessibilityResources.RadioButton_Unselected;
    if (Content is Label label)
    {
      string announcement = string.Format(resourceKey, label.Text);
      CrossSpeakManager.Instance.Output(announcement);
    }
  }
}
