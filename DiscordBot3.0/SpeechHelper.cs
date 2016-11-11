using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Speech;
using System.Speech.Synthesis;
using System.Speech.Recognition;
using System.Speech.AudioFormat;
using System.Speech.Recognition.SrgsGrammar;
using System.Speech.Synthesis.TtsEngine;
using System.IO;
using System.Threading;

namespace DiscordBot3._0
{
    class SpeechStreamerHelper
    {
        SpeechSynthesizer Reader = new SpeechSynthesizer();

        public Prompt GetCurrentlySpokenPrompt { get { return Reader.GetCurrentlySpokenPrompt(); } }

        public int Rate { get { return Reader.Rate; } }
        
        public SpeechStreamerHelper()
        {
            Reader.SelectVoiceByHints(VoiceGender.Female, VoiceAge.Teen);
            //Reader.Rate = -10;
            Reader.Rate = -1;
        }

        public MemoryStream BeginRead(string toRead)
        {
            MemoryStream Bufferers = new MemoryStream();
            Reader.SetOutputToAudioStream(Bufferers, new SpeechAudioFormatInfo(43500, AudioBitsPerSample.Sixteen, AudioChannel.Stereo));
            if (toRead != null && toRead.Trim(' ') != "")
            {   // await for a stream
                Reader.Speak(toRead);
            }

            return Bufferers;
        }

        public void Dispose()
        {
            Reader.SpeakAsyncCancelAll();
            Reader.Dispose();
        }
    }
}
