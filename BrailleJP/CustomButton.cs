using AccessibleMyraUI;
using Microsoft.Xna.Framework.Audio;
using Myra.Graphics2D.UI;

namespace BrailleJP;

public class CustomButton : AccessibleButton
{
  private SoundEffect _confirmSound;

  public CustomButton(string text, int width = 0, HorizontalAlignment horizontalAlignment = HorizontalAlignment.Left)
      : base(text, width, horizontalAlignment)
  {
    Click += OnButtonClick;
  }

  private void OnButtonClick(object sender, System.EventArgs e)
  {
    Game1.Instance.UiConfirmSound.Play();
  }
}