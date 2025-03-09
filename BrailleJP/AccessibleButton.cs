using System;
using Myra.Graphics2D.UI;
using CrossSpeak;

namespace BrailleJP;

public class AccessibleButton : Button
{
  public AccessibleButton(string text, int width = 200, HorizontalAlignment horizontalAlignment = HorizontalAlignment.Center)
  {
    Content = new Label { Text = text };
    Width = width;
    HorizontalAlignment = horizontalAlignment;
    AcceptsKeyboardFocus = true;

    // Ajouter les gestionnaires d'événements pour l'accessibilité
    TouchDown += OnAccessibleTouchDown;
    KeyboardFocusChanged += OnAccessibleKeyboardFocusChanged;
  }

  private void OnAccessibleTouchDown(object sender, EventArgs e)
  {
    AnnounceText();
  }

  private void OnAccessibleKeyboardFocusChanged(object sender, EventArgs e)
  {
    if (IsKeyboardFocused)
      AnnounceText();
  }

  private void AnnounceText()
  {
    if (Content is Label label)
    {
      CrossSpeakManager.Instance.Speak(label.Text);
    }
  }
}