using CrossSpeak;
using Microsoft.Xna.Framework.Input;
using SharpHook;
using System.Speech.Synthesis;
using System.Threading.Tasks;

namespace BrailleJP;

public partial class Game1
{
  protected override void Initialize()
  {
    base.Initialize();
    this.Exiting += onExit;
    CrossSpeakManager.Instance.Initialize();
    _speechSynthesizer = new SpeechSynthesizer();
    // create hook to get keyboard and simulated keyboard (e.g. screen readers inputs) 
    TaskPoolGlobalHook hook = new();
    hook.KeyPressed += OnKeyPressed;
    hook.KeyReleased += OnKeyReleased;
    Task.Run(() => hook.Run());
    _previousKeyboardState = Keyboard.GetState();
    _previousMouseState = Mouse.GetState();
    _brailleParser = new BrailleTableParser(@"LibLouis\tables");
  }

}
