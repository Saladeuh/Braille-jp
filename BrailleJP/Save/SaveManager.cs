using Newtonsoft.Json;
using System;
using System.IO;

namespace LinguaBraille.Save;

internal class SaveManager
{
  private static string DataPath
  {
    get
    {
      return Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "BrailleJpSave.dat");
    }
  }

  private static readonly JsonSerializerSettings Settings = new()
  {
    ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
    NullValueHandling = NullValueHandling.Ignore,
    DefaultValueHandling = DefaultValueHandling.Populate,
    TypeNameHandling = TypeNameHandling.All
  };

  public static SaveParameters LoadSave()
  {
    SaveParameters parameters = LoadJson() ?? new SaveParameters();
    return parameters;
  }

  private static SaveParameters LoadJson()
  {
    if (File.Exists(DataPath))
    {
      using StreamReader r = new(DataPath);
      string json = r.ReadToEnd();
      try
      {
        json = StringCipher.Decrypt(json, Secrets.SAVEKEY);
      }
      catch (FormatException)
      {
      }

      SaveParameters parameters = JsonConvert.DeserializeObject<SaveParameters>(json, Settings);
      return parameters;
    }
#if DEBUG
    //ScreenReader.Output("Nouvelle save");
#endif
    return null;
  }


  public static void WriteSave(SaveParameters parameters)
  {
    string json = JsonConvert.SerializeObject(parameters, Settings);
#if !DEBUG
    json = StringCipher.Encrypt(json, Secrets.SAVEKEY);
#endif
    File.WriteAllText(DataPath, json);
  }
}