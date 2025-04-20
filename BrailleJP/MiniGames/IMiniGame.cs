using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace BrailleJP.MiniGames;

public interface IMiniGame
{
  void Update(GameTime gameTime, KeyboardState currentKeyboardState);
}
