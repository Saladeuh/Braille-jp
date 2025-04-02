using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace BrailleJP;

public partial class BrailleTableParser
{
  private readonly string baseDirectory;
  private readonly HashSet<string> processedFiles;
  private readonly Dictionary<string, string> escapeSequences;
  private static readonly string[] separator = ["\r\n", "\r", "\n"];

  public BrailleTableParser(string baseDirectory = "")
  {
    this.baseDirectory = baseDirectory;
    processedFiles = [];
    escapeSequences = InitializeEscapeSequences();
  }

  private static Dictionary<string, string> InitializeEscapeSequences()
  {
    return new Dictionary<string, string>
          {
              { @"\\", @"\" },
              { @"\f", "\f" },
              { @"\n", "\n" },
              { @"\r", "\r" },
              { @"\s", " " },
              { @"\t", "\t" },
              { @"\v", "\v" },
              { @"\e", "\x1B" }
          };
  }

  private static string[] SplitLine(string line)
  {
    List<string> parts = new();
    StringBuilder currentPart = new();
    bool inEscape = false;

    for (int i = 0; i < line.Length; i++)
    {
      char c = line[i];

      // Gérer les commentaires
      if (c == '#' && !inEscape)
      {
        if (currentPart.Length > 0)
        {
          parts.Add(currentPart.ToString().Trim());
          currentPart.Clear();
        }
        // Ajouter le reste de la ligne comme commentaire
        parts.Add(line[i..].Trim());
        break;
      }

      // Gérer les séquences d'échappement
      if (c == '\\' && !inEscape)
      {
        inEscape = true;
        currentPart.Append(c);
        continue;
      }

      if (inEscape)
      {
        currentPart.Append(c);
        inEscape = false;
        continue;
      }

      // Gérer les espaces comme séparateurs
      if (char.IsWhiteSpace(c) && !inEscape)
      {
        if (currentPart.Length > 0)
        {
          parts.Add(currentPart.ToString().Trim());
          currentPart.Clear();
        }
      }
      else
      {
        currentPart.Append(c);
      }
    }

    // Ajouter la dernière partie si elle existe
    if (currentPart.Length > 0)
    {
      parts.Add(currentPart.ToString().Trim());
    }

    return parts.Where(p => !string.IsNullOrWhiteSpace(p)).ToArray();
  }

  private BrailleEntry ParseLine(string line, string sourceFile, int lineNumber)
  {
    // Handle include statements
    if (line.TrimStart().StartsWith("include "))
    {
      return new BrailleEntry
      {
        Opcode = "include",
        Characters = line[(line.IndexOf("include ") + 8)..].Trim(),
        SourceFile = sourceFile,
        LineNumber = lineNumber
      };
    }

    string[] parts = SplitLine(line);
    if (parts.Length < 2)
      return null;

    string comment = "";
    int commentIndex = -1;
    for (int i = 0; i < parts.Length; i++)
    {
      if (parts[i].StartsWith('#'))
      {
        commentIndex = i;
        break;
      }
    }

    if (commentIndex != -1)
    {
      comment = string.Join(" ", parts.Skip(commentIndex).Select(p => p.TrimStart('#').Trim()));
      parts = parts.Take(commentIndex).ToArray();
    }

    BrailleEntry entry = new()
    {
      Opcode = parts[0].ToLower(),
      Characters = ProcessEscapeSequences(parts[1]),
      DotPattern = parts.Length > 2 ? parts[2] : "",
      Comment = comment,
      SourceFile = sourceFile,
      LineNumber = lineNumber
    };

    return IsValidOpcode(entry.Opcode) ? entry : null;
  }
  public List<BrailleEntry> ParseFile(string filePath, FileEncoding encoding = FileEncoding.UTF8)
  {
    if (processedFiles.Contains(filePath))
      return [];

    processedFiles.Add(filePath);

    // Detect and read file with appropriate encoding
    filePath = Path.Combine(baseDirectory, filePath);
    string fileContent = ReadFileWithEncoding(filePath, encoding);
    List<BrailleEntry> entries = new();
    string[] lines = fileContent.Split(separator, StringSplitOptions.None);

    for (int lineNumber = 0; lineNumber < lines.Length; lineNumber++)
    {
      string line = lines[lineNumber].Trim();

      // Skip blank lines and comments
      if (string.IsNullOrWhiteSpace(line) ||
          line.StartsWith('#') ||
          line.StartsWith('<'))
        continue;

      BrailleEntry entry = ParseLine(line, filePath, lineNumber + 1);
      if (entry != null)
      {
        if (entry.Opcode == "include")
        {
          string includePath = entry.Characters;
          entries.AddRange(ParseFile(includePath, encoding));
        }
        else
        {
          entries.Add(entry);
        }
      }
    }

    return entries;
  }

  private static string ReadFileWithEncoding(string filePath, FileEncoding encoding)
  {
    byte[] bytes = File.ReadAllBytes(filePath);

    return encoding switch
    {
      FileEncoding.ASCII => Encoding.ASCII.GetString(bytes),
      FileEncoding.UTF8 => Encoding.UTF8.GetString(bytes),
      FileEncoding.UTF16BE => Encoding.BigEndianUnicode.GetString(bytes),
      FileEncoding.UTF16LE => Encoding.Unicode.GetString(bytes),
      _ => throw new ArgumentException("Unsupported encoding")
    };
  }


  private string ProcessEscapeSequences(string input)
  {
    string result = input;

    // Handle hex escape sequences
    result = HexSeqRegex().Replace(result, m =>
    {
      string hexValue = m.Groups[1].Value;
      return ((char)Convert.ToInt32(hexValue, 16)).ToString();
    });

    // Handle predefined escape sequences
    foreach (KeyValuePair<string, string> sequence in escapeSequences)
    {
      result = result.Replace(sequence.Key, sequence.Value);
    }

    return result;
  }

  private static bool IsValidOpcode(string opcode)
  {
    HashSet<string> validOpcodes = new()
    {
              "space", "digit", "letter", "lowercase", "uppercase",
              "punctuation", "sign", "math", "litdigit", "include"
          };

    return validOpcodes.Contains(opcode);
  }

  [GeneratedRegex(@"\\x([0-9A-Fa-f]{4})")]
  private static partial Regex HexSeqRegex();
}
public enum FileEncoding
{
  ASCII,
  UTF8,
  UTF16BE,
  UTF16LE
}