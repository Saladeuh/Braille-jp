using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrailleJP.MiniGames;

public interface IMiniGame
{
  public void HandleKeyboard(KeyboardState currentKeyboardState);

}
