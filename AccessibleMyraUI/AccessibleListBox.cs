using CrossSpeak;
using Myra.Graphics2D.UI;
using System;

namespace AccessibleMyraUI;
public class AccessibleListBox : ListView
{
  public AccessibleListBox(int width = 200, int height = 200)
  {
    Width = width;
    Height = height;
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
      ? string.Format(AccessibilityResources.ListBox_Selected, SelectedItem)
      : AccessibilityResources.ListBox_NoSelection;
    CrossSpeakManager.Instance.Output(announcement);
  }
}
