using CrossSpeak;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;
using Myra.Events;
using SharpLouis;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BrailleJP.MiniGames;

public class BasicPractice : IMiniGame
{
  private CultureInfo Culture;

  public List<BrailleEntry> Entries { get; private set; }
  public List<BrailleEntry> LetterEntries { get; private set; }
  public BrailleEntry CurrentEntry { get; private set; }
  public Wrapper BrailleTranslator { get; private set; }
  private SoundEffectInstance _victorySound;
  private SoundEffectInstance _failSound;
  private bool _isPlayingVictorySound = false;

  public BasicPractice(CultureInfo culture)
  {
    this.Culture = culture;
    string tablePath = Game1.SUPPORTEDBRAILLETABLES[culture] + ".utb";
    BrailleTranslator = SharpLouis.Wrapper.Create(tablePath, Game1.LibLouisLoggingClient);
    _victorySound = Game1.Instance.UILittleVictorySound.CreateInstance();
    _failSound = Game1.Instance.UIFailSound.CreateInstance();
    Entries = Game1.Instance.BrailleParser.ParseFile(tablePath);
    LetterEntries = Entries.Where(entry => entry.IsLowercaseLetter()).ToList();
    PeakRandomLetter();
    Game1.Instance.PracticeBrailleInput.TextChanged += onBrailleInput;
  }

  private void PeakRandomLetter()
  {
    CurrentEntry = LetterEntries[Game1.Instance.Random.Next(LetterEntries.Count)];
    CurrentEntry.Voice.Play();
    CrossSpeakManager.Instance.Braille(CurrentEntry.BrailleString);
  }

  public void Update(GameTime gameTime, KeyboardState currentKeyboardState)
  {
    if (_isPlayingVictorySound && _victorySound.State == SoundState.Stopped)
    {
      _isPlayingVictorySound = false;
      PeakRandomLetter();
    }
  }
  private void onBrailleInput(object sender, ValueChangedEventArgs<string> e)
  {
    if (e.NewValue == string.Empty) return;
    if (_victorySound.State == SoundState.Playing)
    {
      Game1.Instance.PracticeBrailleInput.Text = e.OldValue as string;
      return;
    }
    var wantedBrailleChars = CurrentEntry.BrailleString;
    if (Game1.Instance.InputBrailleTranslator.TranslateString(e.NewValue.ToLower(), out var inputBraille))
    {
      if (wantedBrailleChars == inputBraille)
      {
        _victorySound.Play();
        _isPlayingVictorySound = true;
      }
      else
      {
        if (e.NewValue.Length >= wantedBrailleChars.Length)
        {
          _failSound.Play();
        }
      }
    }
    if (e.NewValue.Length >= wantedBrailleChars.Length)
    {
      Game1.Instance.PracticeBrailleInput.Text = String.Empty;
    }
  }
}
