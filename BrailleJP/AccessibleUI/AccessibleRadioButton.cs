using System;
using CrossSpeak;
using Myra.Graphics2D.UI;

namespace BrailleJP.AccessibleUI;

public class AccessibleRadioButton : RadioButton
{
  public AccessibleRadioButton(string text, bool isChecked = false)
  {
    Text = text;
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

    string announcement = string.Format(resourceKey, Text);
    CrossSpeakManager.Instance.Speak(announcement);
  }
}
