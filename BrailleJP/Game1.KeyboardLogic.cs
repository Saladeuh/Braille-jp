﻿using AccessibleMyraUI;
using Microsoft.Xna.Framework.Input;
using Myra.Graphics2D.UI;
using SharpHook;
using System.Collections.Generic;

namespace BrailleJP;

public partial class Game1
{
  private KeyboardState _previousKeyboardState;
  private MouseState _previousMouseState;
  private readonly object _keyLock = new();
  private readonly HashSet<Keys> _hookPressedKeys = new();
  private readonly HashSet<Keys> _keysToProcess = new(); // Nouvelles touches à traiter
  private bool _updateProcessed = false;

  private void HandleKeyboardNavigation(KeyboardState currentKeyboardState)
  {
    // navigation keys (arrows, tab, entrance)
    if (IsKeyPressed(Keys.Down, currentKeyboardState) || IsKeyPressed(Keys.Right, currentKeyboardState))
    {
      _desktop.FocusNext();
    }

    if (IsKeyPressed(Keys.Up, currentKeyboardState) || IsKeyPressed(Keys.Left, currentKeyboardState))
    {
      _desktop.FocusPrevious();
    }
    if (IsKeyPressed(Keys.Enter, currentKeyboardState))
    {
      Widget focused = _desktop.FocusedKeyboardWidget;
      if (focused is Button button)
      {
        button.DoClick();
      }
    }
  }

  public bool IsKeyPressed(Keys key, KeyboardState currentKeyboardState)
  {
    return currentKeyboardState.IsKeyDown(key) && !_previousKeyboardState.IsKeyDown(key);
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
