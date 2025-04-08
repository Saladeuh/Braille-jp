using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Myra;
using Myra.Graphics2D.UI;

namespace BrailleJP;

public partial class Game1
{
  public SoundEffect UiConfirmSound { get => _UiConfirmSound; private set => _UiConfirmSound = value; }
  public SoundEffect UIViewScrollSound { get; private set; }

  private SoundEffect _UiConfirmSound;
  private Song _titleScreenSong;
  private Song _brailleTableViewSong;

  protected override void LoadContent()
  {
    _spriteBatch = new SpriteBatch(GraphicsDevice);
    _titleScreenSong = Content.Load<Song>("music/GoodbyeGeno");
    _brailleTableViewSong = Content.Load<Song>("music/PinnaPark");

    UiConfirmSound = Content.Load<SoundEffect>("ui/confirmation_001");
    UIViewScrollSound = Content.Load<SoundEffect>("ui/view/PM_FSSF2_USER_INTERFACE_SIMPLE_56");
    MediaPlayer.IsRepeating = true;
    MediaPlayer.Volume = 0.5f;
    MyraEnvironment.Game = this;
    _desktop = new Desktop
    {
      HasExternalTextInput = true
    };
    Window.TextInput += (s, a) =>
    {
      _desktop.OnChar(a.Character);
    };

    CreateMainMenu();
    CreateGameUI();
    CreateSettingsUI();
    SwitchToScreen(GameScreen.MainMenu);
  }
}