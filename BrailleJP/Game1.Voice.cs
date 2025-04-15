using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Speech.Synthesis;
using System.Text;
using System.Threading.Tasks;

namespace BrailleJP;

public partial class Game1
{
  private SpeechSynthesizer speechSynthesizer = new();
  public SpeechSynthesizer SpeechSynthesizer { get => speechSynthesizer; private set => speechSynthesizer = value; }

  private void SetVoiceLanguage(CultureInfo culture)
  {
    foreach (System.Speech.Synthesis.InstalledVoice voice in SpeechSynthesizer.GetInstalledVoices())
    {
      if (voice.Enabled && voice.VoiceInfo.Culture.TwoLetterISOLanguageName == culture.TwoLetterISOLanguageName)
        SpeechSynthesizer.SelectVoice(voice.VoiceInfo.Name);
    }
  }
}
