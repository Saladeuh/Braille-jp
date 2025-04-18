﻿using BrailleJP.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Myra.Graphics2D.UI;
using System.Collections.Generic;
using System.Linq;

namespace BrailleJP;

public partial class Game1
{
  protected override void Update(GameTime gameTime)
  {
    KeyboardState nativeKeyboardState = Keyboard.GetState();
    List<Keys> allPressedKeys = nativeKeyboardState.GetPressedKeys().ToList();
    lock (_keyLock)
    {
      allPressedKeys.AddRange(_hookPressedKeys);
      if (!_updateProcessed)
      {
        allPressedKeys.AddRange(_keysToProcess);
        allPressedKeys = allPressedKeys.Distinct().ToList();
      }
    }
    KeyboardState currentKeyboardState = new(allPressedKeys.ToArray());
    MouseState currentMouseState = Mouse.GetState();

    _desktop.UpdateInput();
    HandleKeyboardNavigation(currentKeyboardState);
    // quit on escape key
    if (IsKeyPressed(Keys.Escape, currentKeyboardState))
    {
      if (_gameState.CurrentScreen != GameScreen.MainMenu)
      {
        SwitchToScreen(GameScreen.MainMenu);
      }
      else
      {
        Exit();
      }
    }
    if (_gameState.CurrentScreen == GameScreen.BasicPraticce && !_gameState.IsPaused)
    {
      CurrentPlayingMiniGame.Update(gameTime, currentKeyboardState);
    }

    UpdateUIState();

    _previousKeyboardState = currentKeyboardState;
    _previousMouseState = currentMouseState;
    lock (_keyLock)
    {
      _updateProcessed = true;
      _keysToProcess.Clear(); // Vider les touches à traiter puisqu'elles ont été traitées
    }
    base.Update(gameTime);
  }
}
