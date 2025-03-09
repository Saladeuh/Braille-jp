using System;
using CrossSpeak;
using Myra.Graphics2D.UI;

namespace BrailleJP.AccessibleUI;

public class AccessibleCheckBox : CheckBox
{
  public AccessibleCheckBox(string text, bool isChecked = false)
  {
    Text = text;
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

    string announcement = string.Format(resourceKey, Text);
    CrossSpeakManager.Instance.Speak(announcement);
  }
}
