using System.Collections.Generic;

namespace BrailleJP;

public class BrailleAnalyzer
{
  private const int BRAILLE_UNICODE_OFFSET = 0x2800;
  private const int DOT_COUNT = 8;

  private static int GetPattern(char brailleChar)
  {
    return brailleChar - BRAILLE_UNICODE_OFFSET;
  }

  public static char PatternToChar(int pattern)
  {
    return (char)(pattern + BRAILLE_UNICODE_OFFSET);
  }

  public static string PatternToChar(string pattern)
  {
    string result = "";
    var patterns = pattern.Split('-');
    foreach (var patternChar in patterns)
    {
      result += pattern + BRAILLE_UNICODE_OFFSET;
    }
    return result;
  }

  private static int[] PatternToDots(int pattern)
  {
    List<int> dots = new();
    for (int i = 0; i < DOT_COUNT; i++)
    {
      if ((pattern & (1 << i)) != 0)
      {
        dots.Add(i + 1);
      }
    }
    return [.. dots];
  }

  public static int[] GetRaisedDots(char brailleChar)
  {
    return PatternToDots(GetPattern(brailleChar));
  }

  public static int[] GetCommonRaisedDots(char brailleChar1, char brailleChar2)
  {
    int commonPattern = GetPattern(brailleChar1) & GetPattern(brailleChar2);
    return PatternToDots(commonPattern);
  }

  public static char GetCommonBrailleChar(char brailleChar1, char brailleChar2)
  {
    int commonPattern = GetPattern(brailleChar1) & GetPattern(brailleChar2);
    return PatternToChar(commonPattern);
  }

  /*
  public static void PrintBrailleInfo(char brailleChar)
  {
    int[] dots = GetRaisedDots(brailleChar);
    Console.WriteLine($"Braille character: {brailleChar}");
    Console.WriteLine($"Unicode: U+{((int)brailleChar):X4}");
    Console.WriteLine($"Raised dots: {string.Join(", ", dots)}");
  }
  */

  /*
  public static void CompareAndPrintBrailleChars(char braille1, char braille2)
  {
    int[] dots1 = GetRaisedDots(braille1);
    int[] dots2 = GetRaisedDots(braille2);
    int[] commonDots = GetCommonRaisedDots(braille1, braille2);
    char commonChar = GetCommonBrailleChar(braille1, braille2);

    Console.WriteLine($"First character: {braille1} (U+{((int)braille1):X4})");
    Console.WriteLine($"Raised dots: {string.Join(", ", dots1)}");
    Console.WriteLine($"Second character: {braille2} (U+{((int)braille2):X4})");
    Console.WriteLine($"Raised dots: {string.Join(", ", dots2)}");
    Console.WriteLine($"Common dots: {string.Join(", ", commonDots)}");
    Console.WriteLine($"Common braille character: {commonChar} (U+{((int)commonChar):X4})");
  }
  */
}