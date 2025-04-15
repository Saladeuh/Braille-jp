using CrossSpeak;
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

  public BasicPractice(CultureInfo culture)
  {
    this.Culture = culture;
    string tablePath = Game1.SUPPORTEDBRAILLETABLES[culture] + ".utb";
    BrailleTranslator = SharpLouis.Wrapper.Create(tablePath, Game1.LibLouisLoggingClient);
    Entries = Game1.Instance.BrailleParser.ParseFile(tablePath);
    LetterEntries = Entries.Where(entry => entry.Opcode == "letter").ToList();
    PeakRandomLetter();
    Game1.Instance.PracticeBrailleInput.TextChanged += onBrailleInput;
  }

  private void PeakRandomLetter()
  {
    CurrentEntry = LetterEntries[Game1.Instance.Random.Next(LetterEntries.Count)];
    Game1.Instance.SpeechSynthesizer.Speak(CurrentEntry.Characters);
    CrossSpeakManager.Instance.Braille(CurrentEntry.BrailleChar);
  }

  private void onBrailleInput(object sender, ValueChangedEventArgs<string> e)
  {
    string wantedBrailleChar = CurrentEntry.BrailleChar;
    if (Game1.Instance.InputBrailleTranslator.TranslateString(e.NewValue.ToLower(), out var inputBraille))
    {
      if (wantedBrailleChar == inputBraille)
      {
        CrossSpeakManager.Instance.Output("wééé");
        PeakRandomLetter();
      }
      else
      {
        Game1.Instance.SpeechSynthesizer.Speak(inputBraille);
      }
    }
    if(e.NewValue.Length >= wantedBrailleChar.Length)
    {
      Game1.Instance.PracticeBrailleInput.Text = String.Empty;
    }
  }

  public void HandleKeyboard(KeyboardState currentKeyboardState)
  {

  }
}
