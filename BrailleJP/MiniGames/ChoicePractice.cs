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
  private readonly CultureInfo Culture;
  public List<BrailleEntry> Entries { get; private set; }
  public List<BrailleEntry> LetterEntries { get; private set; }

  private BrailleEntry _choice1;
  private BrailleEntry _choice2;
  private BrailleEntry _choice3;
  private BrailleEntry _choice4;
  private BrailleEntry _guess;

  public Wrapper BrailleTranslator { get; private set; }
  private readonly SoundEffectInstance _victorySound;
  private readonly SoundEffectInstance _failSound;
  private bool _isPlayingVictorySound = false;

  public ChoicePractice(CultureInfo culture)
  {
    this.Culture = culture;
    string tablePath = Game1.SUPPORTEDBRAILLETABLES[culture] + ".utb";
    BrailleTranslator = SharpLouis.Wrapper.Create(tablePath, Game1.LibLouisLoggingClient);
    _victorySound = Game1.Instance.UILittleVictorySound.CreateInstance();
    _failSound = Game1.Instance.UIFailSound.CreateInstance();
    Entries = Game1.Instance.BrailleParser.ParseFile(tablePath);
    LetterEntries = Entries.Where(entry => entry.IsLowercaseLetter()).ToList();
    PeakRandomChoices();
    ShowChoices();
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
    if (_victorySound.State == SoundState.Playing) return;
    if (_isPlayingVictorySound && _victorySound.State == SoundState.Stopped)
    {
      _isPlayingVictorySound = false;
      PeakRandomChoices();
      ShowChoices();
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

    if (userGuess != null && userGuess == _guess)
    {
      _victorySound.Play();
      _isPlayingVictorySound = true;
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
}