﻿using AccessibleMyraUI;
using BrailleJP;
using Myra.Graphics2D.UI;

namespace LinguaBraille.UI;

public class BackButton : AccessibleButton
{
  public BackButton(string text, int width = 0, HorizontalAlignment horizontalAlignment = HorizontalAlignment.Left)
      : base(text, width, horizontalAlignment)
  {
    Click += OnButtonClick;
  }

  private void OnButtonClick(object sender, System.EventArgs e)
  {
    Game1.Instance.UIBackSound.Play();
  }
}