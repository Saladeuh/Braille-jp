using Microsoft.Xna.Framework.Audio;
using System;
using System.IO;

namespace BrailleJP;

public class BrailleEntry
{
  public string Opcode { get; set; }
  public string Characters { get; set; }
  public string DotPattern { get; set; }
  public string Comment { get; set; }
  public string SourceFile { get; set; }

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
      var brailleDotChar = "";
      if (brailleTranslator != null) brailleTranslator.TranslateString(this.Characters, out brailleDotChar);
      return brailleDotChar;
    }
  }
  public SoundEffectInstance Voice { get; set; }
  public BrailleEntry(string opcode, string characters, string sourceFile, string dotPattern = "", string comment = "")
  {
    Opcode = opcode;
    Characters = characters;
    DotPattern = dotPattern;
    Comment = comment;
    SourceFile = sourceFile;
    var fileNameWithoutExt = Path.GetFileNameWithoutExtension(this.SourceFile);
    var soundPath = $"speech/{fileNameWithoutExt}/{DotPattern}";
    try
    {
      Voice = Game1.Instance.Content.Load<SoundEffect>(soundPath).CreateInstance();
      Voice.Volume = 1;
    }
    catch (Exception)
    {
      Voice = null;
    }
  }

  public bool IsLowercaseLetter() => Opcode == "lowercase" || Opcode == "letter";
}