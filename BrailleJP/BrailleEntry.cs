using System.IO;

namespace BrailleJP;

public class BrailleEntry
{
  public string Opcode { get; set; }
  public string Characters { get; set; }
  public string DotPattern { get; set; }
  public string Comment { get; set; }
  public string SourceFile { get; set; }
  public int LineNumber { get; set; }

  // Helper property to get the category/type of the entry
  public string Category => Opcode switch
  {
    "space" => "Whitespace",
    "digit" => "Numeric",
    "letter" => "Alphabetic",
    "lowercase" => "Lowercase",
    "uppercase" => "Uppercase",
    "punctuation" => "Punctuation",
    "sign" => "Sign",
    "math" => "Mathematical",
    "litdigit" => "Literary Digit",
    "include" => "Include",
    _ => "Unknown"
  };

  public override string ToString()
  {
    if (Opcode == "include")
      return $"Include file: {Characters}";
    string brailleDotChar = BrailleString;
    string result = $"{brailleDotChar} {Characters} {DotPattern}";
    if (!string.IsNullOrEmpty(Comment))
      result += $" # {Comment}";
    return result;
  }

  public string BrailleString
  {
    get
    {
      var brailleTranslator = SharpLouis.Wrapper.Create(Path.GetFileName(this.SourceFile), Game1.LibLouisLoggingClient);
      brailleTranslator.TranslateString(this.Characters, out var brailleDotChar);
      return brailleDotChar;
    }
  }
}