using CrossSpeak;
using Myra.Graphics2D.UI;
using System;

namespace AccessibleMyraUI;

public class AccessibleComboBox : ComboView
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
    string announcement = SelectedItem != null
      ? string.Format(AccessibilityResources.ComboBox_Selected, SelectedItem)
      : AccessibilityResources.ComboBox_Focus;
    CrossSpeakManager.Instance.Output(announcement);
  }
}
