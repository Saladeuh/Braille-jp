using Microsoft.Xna.Framework.Input;
using Myra.Graphics2D.UI;
using SharpHook;
using System.Collections.Generic;
using BrailleJP.UI;
using CrossSpeak;
using Microsoft.Xna.Framework.Media;

namespace BrailleJP;

public partial class Game1
{
  private KeyboardState _previousKeyboardState;
  private MouseState _previousMouseState;
  private readonly object _keyLock = new();
  private readonly HashSet<Keys> _hookPressedKeys = new();
  private readonly HashSet<Keys> _keysToProcess = new(); // Nouvelles touches à traiter
  private bool _updateProcessed = false;
  public bool KeyboardSDFJKL = false;

  private void HandleKeyboardNavigation(KeyboardState currentKeyboardState)
  {
    Widget focused = _desktop.FocusedKeyboardWidget;

    // navigation keys (arrows, tab, entrance)
    if (IsKeyPressed(currentKeyboardState, Keys.Down) || IsKeyPressed(currentKeyboardState, Keys.Right))
    {
      _desktop.FocusNext();
    }

    if (IsKeyPressed(currentKeyboardState, Keys.Up) || IsKeyPressed(currentKeyboardState, Keys.Left))
    {
      _desktop.FocusPrevious();
    }
    if (IsKeyPressed(currentKeyboardState, Keys.Enter))
    {
      if (focused is Button button)
      {
        button.DoClick();
      }
    }
    if (KeyboardSDFJKL && focused is BrailleInputTextField brailleInputTextField)
    {
      var dots = new List<int>();
      if (IsKeyPressed(currentKeyboardState, Keys.S))
      {
        dots.Add(3);
      }
      if (IsKeyPressed(currentKeyboardState, Keys.D))
      {
        dots.Add(2);
      }
      if (IsKeyPressed(currentKeyboardState, Keys.F))
      {
        dots.Add(1);
      }
      if (IsKeyPressed(currentKeyboardState, Keys.J))
      {
        dots.Add(4);
      }
      if (IsKeyPressed(currentKeyboardState, Keys.K))
      {
        dots.Add(5);
      }
      if (IsKeyPressed(currentKeyboardState, Keys.M))
      {
        dots.Add(6);
      }
      if (dots.Count > 0)
      {
        var brailleChar = BrailleAnalyzer.PatternToChar(BrailleAnalyzer.DotsToPattern(dots.ToArray()));
      }
    }

    if (IsKeyPressed(currentKeyboardState, Keys.F5))
    {
      KeyboardSDFJKL = !KeyboardSDFJKL;
      if (KeyboardSDFJKL)
      {
        CrossSpeakManager.Instance.Output("Passage en mode saisie SDJJKL.");
      }
      else
      {
        CrossSpeakManager.Instance.Output("Passage en mode saisie plage Braille.");
      }
    }
    if (IsKeyPressed(currentKeyboardState, Keys.F2))
    {
      MediaPlayer.Volume -= 0.1f;
    }
    if (IsKeyPressed(currentKeyboardState, Keys.F3))
    {
      MediaPlayer.Volume += 0.1f;
    }
  }

  public bool IsKeyPressed(KeyboardState currentKeyboardState, params Keys[] keys)
  {
    foreach (Keys key in keys)
    {
      if (currentKeyboardState.IsKeyDown(key) && !_previousKeyboardState.IsKeyDown(key))
      {
        return true;
      }
    }
    return false;
  }

  private void OnKeyPressed(object sender, KeyboardHookEventArgs e)
  {
    if (!IsActive) return;
    Keys monogameKey = Utils.ConvertKeyCodeToMonogameKey(e.Data.KeyCode);

    if (monogameKey != Keys.None)
    {
      lock (_keyLock)
      {
        _hookPressedKeys.Add(monogameKey);
        _keysToProcess.Add(monogameKey); // Ajouter aux touches à traiter
        _updateProcessed = false; // Réinitialiser le flag
      }
    }
  }

  private void OnKeyReleased(object sender, KeyboardHookEventArgs e)
  {
    if (!IsActive) return;
    Keys monogameKey = Utils.ConvertKeyCodeToMonogameKey(e.Data.KeyCode);

    if (monogameKey != Keys.None)
    {
      lock (_keyLock)
      {
        _hookPressedKeys.Remove(monogameKey);
        // Do not withdraw from _KeStoprocess - These keys must be treated at least once      }
      }
    }
  }

}
