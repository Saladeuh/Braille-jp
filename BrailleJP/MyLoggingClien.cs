namespace BrailleJP;

public class MyLoggingClien : SharpLouis.IClient
{
  public void OnLibLouisLog(string message)
  {
    //Console.WriteLine(message);
  }

  public void OnWrapperLog(string message)
  {
    //Console.WriteLine(message);
  }
}