using System;
using CrossSpeak;
using Myra.Graphics2D.UI;

namespace BrailleJP.AccessibleUI;

public class AccessibleTabControl : TabControl
{
  public AccessibleTabControl()
  {
    AcceptsKeyboardFocus = true;

    SelectedIndexChanged += OnAccessibleSelectedIndexChanged;
  }

  private void OnAccessibleSelectedIndexChanged(object sender, EventArgs e)
  {
    if (SelectedIndex >= 0 && SelectedIndex < Items.Count && Items[SelectedIndex??0] is TabItem tabItem)
    {
      string tabText = tabItem.Text ?? AccessibilityResources.Tab_Unnamed;
      string announcement = string.Format(AccessibilityResources.Tab_Selected, tabText);
      CrossSpeakManager.Instance.Speak(announcement);
    }
  }
}
