using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CrossSpeak;
using Myra.Graphics2D.UI;
using static System.Net.Mime.MediaTypeNames;

namespace AccessibleMyraUI;
public class AccessibleListBox : ListBox
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
    string announcement;
    if (SelectedItem != null)
      announcement = string.Format(AccessibilityResources.ListBox_Selected, SelectedItem);
    else
      announcement = AccessibilityResources.ListBox_NoSelection;

    CrossSpeakManager.Instance.Speak(announcement);
  }
}
