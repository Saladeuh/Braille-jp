using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Myra;
using Myra.Graphics2D.UI;

namespace BrailleJP;

public partial class Game1
{
  public SoundEffect UIConfirmSound { get => _UiConfirmSound; private set => _UiConfirmSound = value; }
  public SoundEffect UIViewScrollSound { get; private set; }
  public SoundEffect UIBackSound { get; private set; }
  private SoundEffect _UiConfirmSound;
  private Song _titleScreenSong;
  private Song _brailleTableViewSong;
  private Song _basicPracticeSong;

  protected override void LoadContent()
  {
    _spriteBatch = new SpriteBatch(GraphicsDevice);

    _titleScreenSong = Content.Load<Song>("music/GoodbyeGeno");
    _brailleTableViewSong = Content.Load<Song>("music/PinnaPark");
    _basicPracticeSong = Content.Load<Song>("music/Super Paper Mario： Gloam Valley Arrangement");
    UIConfirmSound = Content.Load<SoundEffect>("ui/confirmation_001");
    UIViewScrollSound = Content.Load<SoundEffect>("ui/view/PM_FSSF2_USER_INTERFACE_SIMPLE_56");
    UIBackSound = Content.Load<SoundEffect>("ui/minimize_008");
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
    _brailleTableViewPanels = new();
    _basicPracticePanels = new();
    foreach (var culture in SUPPORTEDBRAILLETABLES.Keys)
    {
      _brailleTableViewPanels.Add(culture, null);
      _basicPracticePanels.Add(culture, null);
    }
    CreateMainMenu();
    CreateGameUI();
    CreateSettingsUI();
    SwitchToScreen(GameScreen.MainMenu);
  }
}