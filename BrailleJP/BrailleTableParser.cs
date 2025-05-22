using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace BrailleJP;

public partial class BrailleTableParser
{
  private readonly string _baseDirectory;
  private readonly HashSet<string> _processedFiles;
  private readonly Dictionary<string, string> _escapeSequences;
  private static readonly string[] Separator = ["\r\n", "\r", "\n"];

  public BrailleTableParser(string baseDirectory = "")
  {
    this._baseDirectory = baseDirectory;
    _processedFiles = [];
    _escapeSequences = InitializeEscapeSequences();
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

  private BrailleEntry ParseLine(string line, string sourceFile)
  {
    // Handle include statements
    if (line.TrimStart().StartsWith("include "))
    {
      return new BrailleEntry("include",
        line[(line.IndexOf("include ") + 8)..].Trim(),
        sourceFile
      );
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

    BrailleEntry entry = new(parts[0].ToLower(),
      ProcessEscapeSequences(parts[1]),
           sourceFile,
 parts.Length > 2 ? parts[2] : "",
      comment
    );

    return IsValidOpcode(entry.Opcode) ? entry : null;
  }
  public List<BrailleEntry> ParseFile(string filePath, FileEncoding encoding = FileEncoding.UTF8)
  {
    if (!_processedFiles.Add(filePath))
      return [];

    // Detect and read file with appropriate encoding
    filePath = Path.Combine(_baseDirectory, filePath);
    string fileContent = ReadFileWithEncoding(filePath, encoding);
    List<BrailleEntry> entries = new();
    string[] lines = fileContent.Split(Separator, StringSplitOptions.None);

    foreach (var t in lines)
    {
      string line = t.Trim();

      // Skip blank lines and comments
      if (string.IsNullOrWhiteSpace(line) ||
          line.StartsWith('#') ||
          line.StartsWith('<'))
        continue;

      BrailleEntry entry = ParseLine(line, filePath);
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
    foreach (KeyValuePair<string, string> sequence in _escapeSequences)
    {
      result = result.Replace(sequence.Key, sequence.Value);
    }

    return result;
  }

  private static bool IsValidOpcode(string opcode)
  {
    HashSet<string> validOpcodes =
    [
      "space", "digit", "letter", "lowercase", "uppercase",
      "punctuation", "sign", "math", "litdigit", "include"
    ];

    return validOpcodes.Contains(opcode);
  }

  [GeneratedRegex(@"\\x([0-9A-Fa-f]{4})")]
  private static partial Regex HexSeqRegex();
}
