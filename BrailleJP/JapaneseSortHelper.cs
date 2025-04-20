using System;
using System.Collections.Generic;

public static class JapaneseSortHelper
{
  // Correspondence table for the Gojūon order (traditional Japanese order)
  private static readonly Dictionary<char, int> GojuonOrder = new()
  {
        // Ordre des kanas: a, i, u, e, o, puis ka, ki, ku, ke, ko, etc.
        
        // Hiragana
        {'あ', 1}, {'い', 2}, {'う', 3}, {'え', 4}, {'お', 5},
        {'か', 6}, {'き', 7}, {'く', 8}, {'け', 9}, {'こ', 10},
        {'が', 11}, {'ぎ', 12}, {'ぐ', 13}, {'げ', 14}, {'ご', 15},
        {'さ', 16}, {'し', 17}, {'す', 18}, {'せ', 19}, {'そ', 20},
        {'ざ', 21}, {'じ', 22}, {'ず', 23}, {'ぜ', 24}, {'ぞ', 25},
        {'た', 26}, {'ち', 27}, {'つ', 28}, {'て', 29}, {'と', 30},
        {'だ', 31}, {'ぢ', 32}, {'づ', 33}, {'で', 34}, {'ど', 35},
        {'な', 36}, {'に', 37}, {'ぬ', 38}, {'ね', 39}, {'の', 40},
        {'は', 41}, {'ひ', 42}, {'ふ', 43}, {'へ', 44}, {'ほ', 45},
        {'ば', 46}, {'び', 47}, {'ぶ', 48}, {'べ', 49}, {'ぼ', 50},
        {'ぱ', 51}, {'ぴ', 52}, {'ぷ', 53}, {'ぺ', 54}, {'ぽ', 55},
        {'ま', 56}, {'み', 57}, {'む', 58}, {'め', 59}, {'も', 60},
        {'や', 61}, {'ゆ', 62}, {'よ', 63},
        {'ら', 64}, {'り', 65}, {'る', 66}, {'れ', 67}, {'ろ', 68},
        {'わ', 69}, {'を', 70}, {'ん', 71},
        
        // Katakana
        {'ア', 101}, {'イ', 102}, {'ウ', 103}, {'エ', 104}, {'オ', 105},
        {'カ', 106}, {'キ', 107}, {'ク', 108}, {'ケ', 109}, {'コ', 110},
        {'ガ', 111}, {'ギ', 112}, {'グ', 113}, {'ゲ', 114}, {'ゴ', 115},
        {'サ', 116}, {'シ', 117}, {'ス', 118}, {'セ', 119}, {'ソ', 120},
        {'ザ', 121}, {'ジ', 122}, {'ズ', 123}, {'ゼ', 124}, {'ゾ', 125},
        {'タ', 126}, {'チ', 127}, {'ツ', 128}, {'テ', 129}, {'ト', 130},
        {'ダ', 131}, {'ヂ', 132}, {'ヅ', 133}, {'デ', 134}, {'ド', 135},
        {'ナ', 136}, {'ニ', 137}, {'ヌ', 138}, {'ネ', 139}, {'ノ', 140},
        {'ハ', 141}, {'ヒ', 142}, {'フ', 143}, {'ヘ', 144}, {'ホ', 145},
        {'バ', 146}, {'ビ', 147}, {'ブ', 148}, {'ベ', 149}, {'ボ', 150},
        {'パ', 151}, {'ピ', 152}, {'プ', 153}, {'ペ', 154}, {'ポ', 155},
        {'マ', 156}, {'ミ', 157}, {'ム', 158}, {'メ', 159}, {'モ', 160},
        {'ヤ', 161}, {'ユ', 162}, {'ヨ', 163},
        {'ラ', 164}, {'リ', 165}, {'ル', 166}, {'レ', 167}, {'ロ', 168},
        {'ワ', 169}, {'ヲ', 170}, {'ン', 171},
        
        // Petits caractères et caractères spéciaux
        {'っ', 72}, {'ゃ', 73}, {'ゅ', 74}, {'ょ', 75},
        {'ッ', 172}, {'ャ', 173}, {'ュ', 174}, {'ョ', 175}
    };

  public static int CompareGojuon(string str1, string str2)
  {
    int minLength = Math.Min(str1.Length, str2.Length);

    for (int i = 0; i < minLength; i++)
    {
      int order1 = GetCharOrder(str1[i]);
      int order2 = GetCharOrder(str2[i]);

      if (order1 != order2)
      {
        return order1.CompareTo(order2);
      }
    }

    // If all the characters until the minimum length are equal, the shortest chain comes first
    return str1.Length.CompareTo(str2.Length);
  }

  private static int GetCharOrder(char c)
  {
    if (GojuonOrder.TryGetValue(c, out int order))
    {
      return order;
    }

    // For characters that are not in the table, use their unicode value
    return c;
  }

  // Extension method to sort a list of Brailleentry according to the gojūon order
  public static void SortByGojuon<T>(this List<T> entries, Func<T, string> selector)
  {
    entries.Sort((e1, e2) => CompareGojuon(selector(e1), selector(e2)));
  }
}