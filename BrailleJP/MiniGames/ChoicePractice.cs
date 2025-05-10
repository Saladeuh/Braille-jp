using CrossSpeak;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;
using SharpLouis;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace BrailleJP.MiniGames;

public class ChoicePractice : IMiniGame
{
  public string Tips { get; } = @"Cet entraînement est le plus simple.
Vous allez entendre un caractère, puis 4 signes Braille seront affichés.
Vous devrez choisir lequel correspond en appuyant sur les chiffres de 1-4.
Pour réécouter le signe, appuyez sur 0.";
  public int Score { get => _goodGuesses; }
  private readonly CultureInfo Culture;
  public List<BrailleEntry> Entries { get; private set; }
  public List<BrailleEntry> LetterEntries { get; private set; }

  private BrailleEntry _choice1;
  private BrailleEntry _choice2;
  private BrailleEntry _choice3;
  private BrailleEntry _choice4;
  private BrailleEntry _guess;

  public Wrapper BrailleTranslator { get; private set; }
  public bool IsRunning { get; set; }

  private readonly SoundEffectInstance _goodSound;
  private SoundEffectInstance _victorySound;
  private readonly SoundEffectInstance _failSound;
  private bool _isPlayingGoodSound = false;
  private int _goodGuesses;
  private int _fails;
  private int _failsOnThisEntry;
  private bool _isPlayingFailSound;
  private bool _isReadingTips;
  private bool _firstFrameEnterHandled;

  public ChoicePractice(CultureInfo culture, bool firstPlay)
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
    PeakRandomChoices();
    if (firstPlay)
    {
      CrossSpeakManager.Instance.Output(Tips);
      _isReadingTips = true;
    }
    else
    {
      ShowChoices();
    }
  }

  private void PeakRandomChoices()
  {
    _choice1 = PeakRandomLetter();
    _choice2 = PeakRandomLetterThatsNot(_choice1);
    _choice3 = PeakRandomLetterThatsNot(_choice1, _choice2);
    _choice4 = PeakRandomLetterThatsNot(_choice1, _choice2, _choice3);
    _guess = new[] { _choice1, _choice2, _choice3, _choice4 }[Game1.Instance.Random.Next(4)];
  }

  private BrailleEntry PeakRandomLetter()
    => LetterEntries[Game1.Instance.Random.Next(LetterEntries.Count)];
  private BrailleEntry PeakRandomLetterThatsNot(params BrailleEntry[] excluded)
  {
    BrailleEntry randomLetter;
    do
    {
      randomLetter = PeakRandomLetter();
    } while (excluded.Contains(randomLetter));
    return randomLetter;
  }
  public void Update(GameTime gameTime, KeyboardState currentKeyboardState)
  {
    if (_goodSound.State == SoundState.Playing) return;
    if (!_firstFrameEnterHandled && _isReadingTips && Game1.Instance.IsKeyPressed(currentKeyboardState, Keys.Enter)) {
      _firstFrameEnterHandled = true;
    } else if (_isReadingTips && Game1.Instance.IsKeyPressed(currentKeyboardState, Keys.Enter, Keys.Space))
    {
      _isReadingTips = false;
      ShowChoices();
    }
    else if (_isReadingTips) return;
    if ((_isPlayingGoodSound && _goodSound.State == SoundState.Stopped)
      || (_failsOnThisEntry >= 3 && (_isPlayingFailSound && _failSound.State == SoundState.Stopped)))
    {
      _isPlayingGoodSound = false;
      PeakRandomChoices();
      ShowChoices();
      _failsOnThisEntry = 0;
    }
    if (_goodGuesses + _fails > 10 && _goodSound.State != SoundState.Playing)
    {
      Win();
    }
    BrailleEntry userGuess;
    if (Game1.Instance.IsKeyPressed(currentKeyboardState, Keys.D1, Keys.NumPad1))
    {
      userGuess = _choice1;
    }
    else if (Game1.Instance.IsKeyPressed(currentKeyboardState, Keys.D2, Keys.NumPad2))
    {
      userGuess = _choice2;
    }
    else
    {
      userGuess = Game1.Instance.IsKeyPressed(currentKeyboardState, Keys.D3, Keys.NumPad3)
        ? _choice3
        : Game1.Instance.IsKeyPressed(currentKeyboardState, Keys.D4, Keys.NumPad4) ? _choice4 : null;
    }
    if (Game1.Instance.IsKeyPressed(currentKeyboardState, Keys.D0, Keys.NumPad0))
    {
      ShowChoices();
    }
    if (userGuess != null && userGuess == _guess)
    {
      _goodSound.Play();
      _isPlayingGoodSound = true;
      _goodGuesses++;
    }
    else if (userGuess != null && userGuess != _guess)
    {
      _fails++;
      _failsOnThisEntry++;
      _failSound.Play();
      _isPlayingFailSound = true;
    }
    else if (userGuess != null)
    {
      ShowChoices();
    }
  }
  private void ShowChoices()
  {
    _guess.Voice.Play();
    CrossSpeakManager.Instance.Braille($"{_choice1.BrailleString} {_choice2.BrailleString} {_choice3.BrailleString} {_choice4.BrailleString}");
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
  }
}