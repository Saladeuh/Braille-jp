using BrailleJP;
using LinguaBraille.Content;
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

namespace LinguaBraille.MiniGames;

public class WordPractice : IMiniGame
{
  public string Tips { get; } = GameText.Main_menu_word_practice;
  public int Score => _goodAnswers;
  private readonly Dictionary<string, SoundEffect> _words;
  private string CurrentWord { get; set; }
  private Wrapper BrailleTranslator { get; set; }
  public bool IsRunning { get; set; }

  private readonly SoundEffectInstance _goodSound;
  private readonly SoundEffectInstance _victorySound;
  private readonly SoundEffectInstance _failSound;
  private bool _isPlayingGoodSound = false;
  private int _goodAnswers;
  private int _fails;
  private bool _isReadingTips;
  private bool _firstFrameEnterHandled;

  public WordPractice(CultureInfo culture, bool firstPlay)
  {
    IsRunning = true;
    _words = new();
    string tablePath = Game1.SUPPORTEDBRAILLETABLES[culture];
    BrailleTranslator = Wrapper.Create(tablePath, Game1.LibLouisLoggingClient);
    _goodSound = Game1.Instance.UIGoodSound.CreateInstance();
    _goodSound.Volume = 0.5f;
    _victorySound = Game1.Instance.UIVictorySound.CreateInstance();
    _victorySound.Volume = 0.5f;
    _failSound = Game1.Instance.UIFailSound.CreateInstance();
    _failSound.Volume = 0.5f;
    var wordFiles = Directory.GetFiles(Path.Combine(Game1.Instance.Content.RootDirectory, "speech", Path.GetFileNameWithoutExtension(tablePath), "words"));
    do{
      var wordFilePath = wordFiles[Game1.Instance.Random.Next(wordFiles.Length)];
      var word = Path.GetFileNameWithoutExtension(wordFilePath);
      if (!word.Contains("_slow"))
      {
        _words.TryAdd(word, Game1.Instance.Content.Load<SoundEffect>($"speech/{Path.GetFileNameWithoutExtension(tablePath)}/words/{word}"));
      }
    } while(_words.Count<=10);
    Game1.Instance.PracticeBrailleInput.TextChanged += onBrailleInput;
    if (firstPlay)
    {
      CrossSpeakManager.Instance.Output(Tips);
      _isReadingTips = true;
    }
    else
    {
      PeakRandomWord();
    }
  }

  private void PeakRandomWord()
  {
    CurrentWord = _words.ElementAt(Game1.Instance.Random.Next(_words.Count)).Key;
    _words[CurrentWord].Play();
#if DEBUG
    BrailleTranslator.TranslateString(CurrentWord, out var brailleChars);
    CrossSpeakManager.Instance.Braille(brailleChars);
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
      PeakRandomWord();
    }
    if (_goodAnswers + _fails >= 10 && _goodSound.State != SoundState.Playing)
    {
      Win();
    }
    if (_isPlayingGoodSound && _goodSound.State == SoundState.Stopped)
    {
      _isPlayingGoodSound = false;
      PeakRandomWord();
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
    BrailleTranslator.TranslateString(CurrentWord, out var wantedBrailleChars);
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
      Game1.Instance.PracticeBrailleInput.Text = string.Empty;
    }
  }

  public void Win()
  {
    _victorySound.Play();
    Stop();
  }

  private void Stop()
  {
    IsRunning = false;
    BrailleTranslator.Free();
    _goodSound.Dispose();
    _failSound.Dispose();
    Game1.Instance.PracticeBrailleInput.TextChanged -= onBrailleInput;
  }
}
