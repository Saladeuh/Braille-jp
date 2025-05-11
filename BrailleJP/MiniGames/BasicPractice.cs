using CrossSpeak;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;
using Myra.Events;
using SharpLouis;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace BrailleJP.MiniGames;

public class BasicPractice : IMiniGame
{
  public int Score { get => _goodAnswers; }
  public string Tips { get; } = @"Vous allez entendre un caractère, puis vous devrez taper le symbole correspondant sur votre plage Braille.
Attention ! Vous n’avez qu’un seul essai !";
  private readonly CultureInfo Culture;

  public List<BrailleEntry> Entries { get; private set; }
  public List<BrailleEntry> LetterEntries { get; private set; }
  public BrailleEntry CurrentEntry { get; private set; }
  public Wrapper BrailleTranslator { get; private set; }
  public bool IsRunning { get; set; }

  private readonly SoundEffectInstance _goodSound;
  private readonly SoundEffectInstance _victorySound;
  private readonly SoundEffectInstance _failSound;
  private bool _isPlayingGoodSound = false;
  private int _goodAnswers;
  private int _fails;
  private bool _isReadingTips;
  private bool _firstFrameEnterHandled;

  public BasicPractice(CultureInfo culture, bool firstPlay)
  {
    IsRunning = true;
    this.Culture = culture;
    string tablePath = Game1.SUPPORTEDBRAILLETABLES[culture];
    BrailleTranslator = SharpLouis.Wrapper.Create(tablePath, Game1.LibLouisLoggingClient);
    _goodSound = Game1.Instance.UIGoodSound.CreateInstance();
    _goodSound.Volume = 0.5f;
    _victorySound = Game1.Instance.UIVictorySound.CreateInstance();
    _victorySound.Volume = 0.5f;
    _failSound = Game1.Instance.UIFailSound.CreateInstance();
    _failSound.Volume = 0.5f;
    Entries = Game1.Instance.BrailleTables[tablePath];
    LetterEntries = Entries.Where(entry => entry.IsLowercaseLetter()).ToList();
    Game1.Instance.PracticeBrailleInput.TextChanged += onBrailleInput;
    if (firstPlay)
    {
      CrossSpeakManager.Instance.Output(Tips);
      _isReadingTips = true;
    }
    else
    {
      PeakRandomLetter();
    }
  }

  private void PeakRandomLetter()
  {
    CurrentEntry = LetterEntries[Game1.Instance.Random.Next(LetterEntries.Count)];
    CurrentEntry.Voice.Play();
#if DEBUG
    CrossSpeakManager.Instance.Braille(CurrentEntry.BrailleString);
#endif
  }

  public void Update(GameTime gameTime, KeyboardState currentKeyboardState)
  {
    if (!IsRunning) return;
    if (!_firstFrameEnterHandled && _isReadingTips && Game1.Instance.IsKeyPressed(currentKeyboardState, Keys.Enter))
    {
      _firstFrameEnterHandled = true;
    }
    else if (_isReadingTips && Game1.Instance.IsKeyPressed(currentKeyboardState, Keys.Enter, Keys.Space))
    {
      _isReadingTips = false;
      PeakRandomLetter();
    }
    if (_goodAnswers + _fails >= 10 && _goodSound.State != SoundState.Playing)
    {
      Win();
    }
    if (_isPlayingGoodSound && _goodSound.State == SoundState.Stopped)
    {
      _isPlayingGoodSound = false;
      PeakRandomLetter();
    }
  }
  private void onBrailleInput(object sender, ValueChangedEventArgs<string> e)
  {
    if (e.NewValue == string.Empty) return;
    if (_goodSound.State == SoundState.Playing)
    {
      Game1.Instance.PracticeBrailleInput.Text = e.OldValue;
      return;
    }
    var wantedBrailleChars = CurrentEntry.BrailleString;
    if (Game1.Instance.InputBrailleTranslator.TranslateString(e.NewValue.ToLower(), out var inputBraille))
    {
      if (wantedBrailleChars == inputBraille)
      {
        _goodSound.Play();
        _isPlayingGoodSound = true;
        _goodAnswers++;
      }
      else
      {
        if (e.NewValue.Length >= wantedBrailleChars.Length)
        {
          _failSound.Play();
          _fails++;
        }
      }
    }
    if (e.NewValue.Length >= wantedBrailleChars.Length)
    {
      Game1.Instance.PracticeBrailleInput.Text = String.Empty;
    }
  }

  public void Win()
  {
    _victorySound.Play();
    CrossSpeakManager.Instance.Output("Gagné !");
    Stop();
  }
  public void Stop()
  {
    IsRunning = false;
    BrailleTranslator.Free();
    _goodSound.Dispose();
    _failSound.Dispose();
    Game1.Instance.PracticeBrailleInput.TextChanged -= onBrailleInput;
  }
}
