using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
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
    if (IsKeyPressed(currentKeyboardState, Keys.Escape))
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
    if ((_gameState.CurrentScreen == GameScreen.BasicPraticce || _gameState.CurrentScreen == GameScreen.ChoicePraticce)
      && !_gameState.IsPaused)
    {
      if (CurrentPlayingMiniGame.IsRunning)
      {
        CurrentPlayingMiniGame.Update(gameTime, currentKeyboardState);
      }
      else
      {
        CurrentPlayingMiniGame = null;
        SwitchToScreen(GameScreen.MainMenu);
      }
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
