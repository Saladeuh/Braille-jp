using System;
using CrossSpeak;
using Myra.Graphics2D.UI;

namespace BrailleJP.AccessibleUI;

public class AccessibleComboBox : ComboBox
{
  public AccessibleComboBox(int width = 200)
  {
    Width = width;
    AcceptsKeyboardFocus = true;

    KeyboardFocusChanged += OnAccessibleKeyboardFocusChanged;
    SelectedIndexChanged += OnAccessibleSelectedIndexChanged;
  }

  private void OnAccessibleKeyboardFocusChanged(object sender, EventArgs e)
  {
    if (IsKeyboardFocused)
      AnnounceSelection();
  }

  private void OnAccessibleSelectedIndexChanged(object sender, EventArgs e)
  {
    AnnounceSelection();
  }

  private void AnnounceSelection()
  {
    string announcement;
    if (SelectedItem != null)
      announcement = string.Format(AccessibilityResources.ComboBox_Selected, SelectedItem);
    else
      announcement = AccessibilityResources.ComboBox_Focus;

    CrossSpeakManager.Instance.Speak(announcement);
  }
}
