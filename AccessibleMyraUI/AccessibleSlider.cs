using System;
using CrossSpeak;
using Myra.Graphics2D.UI;

namespace AccessibleMyraUI;

public class AccessibleSlider : HorizontalSlider
{
  public AccessibleSlider(float initialValue = 0.5f, int width = 200)
  {
    Value = initialValue;
    Width = width;
    AcceptsKeyboardFocus = true;

    KeyboardFocusChanged += OnAccessibleKeyboardFocusChanged;
    ValueChanged += OnValueChanged;
  }

  private void OnAccessibleKeyboardFocusChanged(object sender, EventArgs e)
  {
    if (IsKeyboardFocused)
      Announce();
  }

  private void OnValueChanged(object sender, EventArgs e)
  {
    if (IsKeyboardFocused)
      Announce();
  }

  private void Announce()
  {
    int percentage = (int)(Value * 100);
    string announcement = string.Format(AccessibilityResources.Slider, Id, percentage);
    CrossSpeakManager.Instance.Output(announcement);
  }
}
