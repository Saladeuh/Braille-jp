using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Myra;
using Myra.Graphics2D.UI;

namespace BrailleJP;

public partial class Game1
{
  public SoundEffect UiConfirmSound { get => _UiConfirmSound; private set => _UiConfirmSound = value; }
  private SoundEffect _UiConfirmSound;
  private Song _titleScreenSong;
  private Song _brailleTableViewSong;

  protected override void LoadContent()
  {
    _spriteBatch = new SpriteBatch(GraphicsDevice);
    _titleScreenSong = Content.Load<Song>("music/GoodbyeGeno");
    _brailleTableViewSong = Content.Load<Song>("music/PinnaPark");
    UiConfirmSound = Content.Load<SoundEffect>("ui/confirmation_001");
    MediaPlayer.IsRepeating = true;
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