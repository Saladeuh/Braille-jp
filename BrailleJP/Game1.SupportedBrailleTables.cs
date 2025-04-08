using System;
using System.Collections.Generic;
using System.Globalization;

namespace BrailleJP;

public partial class Game1
{
  private static readonly Dictionary<CultureInfo, string> SUPPORTEDBRAILLETABLES = new Dictionary<CultureInfo, string>
  {
    {new CultureInfo("ja-Jp"), "ja-jp-comp6" }
  };
}
