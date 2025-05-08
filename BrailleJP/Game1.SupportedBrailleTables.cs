using System.Collections.Generic;
using System.Globalization;

namespace BrailleJP;

public partial class Game1
{
  public static readonly Dictionary<CultureInfo, string> SUPPORTEDBRAILLETABLES = new()
  {
    {new CultureInfo("ja-Jp"), "ja-jp-comp6.utb" },
    //{new CultureInfo("fr-FR"), "fr-bfu-comp8.utb" }
  };
}
