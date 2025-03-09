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
    ValueChanged += OnAccessibleValueChanged;
  }

  private void OnAccessibleKeyboardFocusChanged(object sender, EventArgs e)
  {
    if (IsKeyboardFocused)
      AnnounceValue();
  }

  private void OnAccessibleValueChanged(object sender, EventArgs e)
  {
    if (IsKeyboardFocused)
      AnnounceValue();
  }

  private void AnnounceValue()
  {
    int percentage = (int)(Value * 100);
    string announcement = string.Format(AccessibilityResources.Slider_Value, percentage);
    CrossSpeakManager.Instance.Speak(announcement);
  }
}
