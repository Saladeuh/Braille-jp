using System;
using System.Collections.Generic;
using System.Globalization;

namespace BrailleJP.Save;

public class SaveParameters
{

  public SaveParameters()
  {
    Volume = 0.5f;
    Language = CultureInfo.InstalledUICulture.TwoLetterISOLanguageName;
    LastPlayed = default;
    Flags = new Flags();
  }
  public float Volume { get; set; }
  public float MusicVolume { get; set; }
  public string? Language { get; set; }
  public DateTime LastPlayed { get; set; }
  public Flags Flags { get; set; }
}