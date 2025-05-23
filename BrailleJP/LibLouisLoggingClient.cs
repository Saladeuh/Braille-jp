using CrossSpeak;

namespace LinguaBraille;

public class LibLouisLoggingClient : SharpLouis.IClient
{
  public void OnLibLouisLog(string message)
  {
#if DEBUG
    CrossSpeakManager.Instance.Output(message);
#endif
  }

  public void OnWrapperLog(string message)
  {
#if DEBUG
    //CrossSpeakManager.Instance.Output(message);
#endif
  }
}