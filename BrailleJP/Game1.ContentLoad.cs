﻿using LinguaBraille;
using LinguaBraille.Save;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Myra;
using Myra.Graphics2D.UI;

namespace BrailleJP;

public partial class Game1
{
  public SoundEffect UIConfirmSound { get => _UiConfirmSound; private set => _UiConfirmSound = value; }
  public SoundEffect UIGoodSound { get; private set; }
  public SoundEffect UIVictorySound { get; private set; }
  public SoundEffect UIFailSound { get; private set; }
  public SoundEffect UIViewScrollSound { get; private set; }
  public SoundEffect UIBackSound { get; private set; }
  private SoundEffect _UiConfirmSound;
  private Song _titleScreenSong;
  private Song _brailleTableViewSong;
  private Song _basicPracticeSong;
  protected override void LoadContent()
  {
    _spriteBatch = new SpriteBatch(GraphicsDevice);
    Save = SaveManager.LoadSave();
    _titleScreenSong = Content.Load<Song>("music/GoodbyeGeno");
    _brailleTableViewSong = Content.Load<Song>("music/PinnaPark");
    _basicPracticeSong = Content.Load<Song>("music/Super Paper Mario： Gloam Valley Arrangement");
    UIConfirmSound = Content.Load<SoundEffect>("ui/confirmation_001");
    UIBackSound = Content.Load<SoundEffect>("ui/minimize_008");
    UIGoodSound = Content.Load<SoundEffect>("ui/confirmation_002");
    UIVictorySound = Content.Load<SoundEffect>("ui/confirmation_004");
    UIFailSound = Content.Load<SoundEffect>("ui/Cartoon Toy Squeaky Toy Squeaks 01");
    UIViewScrollSound = Content.Load<SoundEffect>("ui/view/PM_FSSF2_USER_INTERFACE_SIMPLE_56");
    //string tablePath = Game1.SUPPORTEDBRAILLETABLES.First().Value;
    //var entries = BrailleParser.ParseFile(tablePath);
    MediaPlayer.IsRepeating = true;
    MediaPlayer.Volume = 0.3f;
    SoundEffect.MasterVolume = 1f;
    MyraEnvironment.Game = this;
    _desktop = new Desktop
    {
      HasExternalTextInput = true
    };
    Window.TextInput += (s, a) =>
    {
      _desktop.OnChar(a.Character);
    };
    _brailleTableViewPanels = [];
    _basicPracticePanels = [];
    foreach (var culture in SUPPORTEDBRAILLETABLES.Keys)
    {
      _brailleTableViewPanels.Add(culture, null);
      _basicPracticePanels.Add(culture, null);
    }
    foreach (var table in SUPPORTEDBRAILLETABLES.Values)
    {
      BrailleTables[table] = BrailleParser.ParseFile(table);
    }
    CreateMainMenu();
    CreateSettingsUI();
    CreateFirstScreen();
    SwitchToScreen(Save.Flags.EmptySave ? GameScreen.First : GameScreen.MainMenu);
  }
}