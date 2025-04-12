﻿using AccessibleMyraUI;
using Microsoft.Xna.Framework.Audio;
using Myra.Graphics2D.UI;
namespace BrailleJP.UI;

public class ConfirmButton : AccessibleButton
{
    public ConfirmButton(string text, int width = 0, HorizontalAlignment horizontalAlignment = HorizontalAlignment.Left)
        : base(text, width, horizontalAlignment)
    {
        Click += OnButtonClick;
    }

    private void OnButtonClick(object sender, System.EventArgs e)
    {
        Game1.Instance.UIConfirmSound.Play();
    }
}