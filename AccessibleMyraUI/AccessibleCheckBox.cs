using CrossSpeak;
using Myra.Graphics2D.UI;
using System;

namespace AccessibleMyraUI;

public class AccessibleCheckBox : CheckButton
{
  public AccessibleCheckBox(string text, bool isChecked = false)
  {
    Content = new Label { Text = text };
    IsChecked = isChecked;
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
    AnnounceState();
  }

  private void AnnounceState()
  {
    string resourceKey = IsChecked ?
        AccessibilityResources.CheckBox_Checked :
        AccessibilityResources.CheckBox_Unchecked;
    if (Content is Label label)
    {
      var announcement = string.Format(resourceKey, label.Text);
      CrossSpeakManager.Instance.Output(announcement);
    }
  }
}
