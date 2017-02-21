using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using Discord;
using Discord.Audio;
using Discord.API;
using Discord.Commands;
using Discord.ETF;
using Discord.Modules;
using Discord.Net;
using System.Timers;
using Discord.API.Converters;
using NAudio;
using NAudio.Wave;
using NAudio.CoreAudioApi;
using NAudio.FileFormats;
using System.Diagnostics;

namespace DiscordBot3._0
{
    // the Dj/Audio Part for
    partial class DiscordBotWorker
    {
#if DEBUG
        SoundCloudClient SCClient;
#endif

        /// <summary>
        /// Our Speech engien
        /// </summary>
        SpeechStreamerHelper SpeechHelper = new SpeechStreamerHelper();

        /// <summary>
        /// A call back to tell us we need to update the list
        /// </summary>
        public VoidVoid SoundBoardListUpDate { get; private set; }

        /// <summary>
        /// our sound clients
        /// </summary>
        Dictionary<ulong, IAudioClient> Radios = new Dictionary<ulong, IAudioClient>();

        /// <summary>
        /// the nasty over complicated bad practice Look up table
        /// </summary>
        public Dictionary<string, DictionaryDescriptorHelper<Dictionary<string, List<string>>>> SoundBoardBinding;

        /// <summary>
        /// the music Tree for DJ
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private bool MusicTree(MessageEventArgs e)
        {
            string[] Args = ArgMaker(e.Message.Text.Remove(0, 2));

            if (Args.Count() < 1)
                return false;

            if (Args[0].ToLower() == "help")
                e.Channel.SendMessage("Thanks for asking!:kissing_heart:\nFor DJ Commands.:minidisc:\n!@texttovoice (or ttv)-> Speaks the text aloud\n!@soundboard (or sb)-> plays sounds Note:there is extra help if you enter just =>!@soundboard.\n!@joinchannel -> she will simply join channel\nmore coming soon!");
            else if (Args.Count() == 2 && Args[0].ToLower() == "joinchannel")
                joinChannel(e.Server.Id, e.Server.FindChannels(Args[1], ChannelType.Voice, true).First());
            else if (Args[0].ToLower() == "joinchannel")
            {
                joinChannel(e.Server.Id, e.User.VoiceChannel);
            }
            else if (Args[0].ToLower() == "soundboard" || Args[0].ToLower() == "sb")
            {
                    SoundBoard(e.Server.Id, e.Channel, e.User.VoiceChannel, Args);
            }
            else if (Args.Count() > 1 && (Args[0].ToLower() == "texttovoice" || Args[0].ToLower() == "ttv"))
            {
                if (e.User?.VoiceChannel != null)
                {
                    SendAudioFromString(e.Server.Id, e.User?.VoiceChannel, e.Message.Text.Remove(0, 6));
                }
            }
            else if (Args[0].ToLower() == "testsound")
            {
                Console.WriteLine("InIt");
                Console.WriteLine(e.User?.VoiceChannel?.Name);
                if (e.User?.VoiceChannel != null)
                {
                    TestSendAudio(e.Server.Id, e.User.VoiceChannel);
                }
            }
#if DEBUG
            else if (Args.Count() > 1 && (Args[0].ToLower() == "ssc" || Args[0].ToLower() == "ssc"))
            {
                GetSCTracks(e, Args, 2);
            }
            else if (Args.Count() > 1 && (Args[0].ToLower() == "sscp" || Args[0].ToLower() == "sscp"))
            {
                Console.WriteLine("InIt");
                Console.WriteLine(e.User?.VoiceChannel?.Name);
                if (e.User?.VoiceChannel != null)
                {
                    SoundCloudStream(e, Args, e.Server.Id, e.User.VoiceChannel);
                }
            }
#endif
            else return false;
            return true;
        }


        ///////////////////////////////////////////////////util///////////////////

        /// <summary>
        /// call back for geting the sound board back
        /// </summary>
        void _SoundBoardListUpDate()
        {
            lock (SoundBoardBinding)
                SoundBoardBinding = _ControlPannel.SoundBoardBinding;
        }

        /// <summary>
        /// Play a set of sounds
        /// </summary>
        /// <param name="Join"></param>
        /// <param name="Args"></param>
        void SoundBoard(ulong ServerLight, Channel Chat, Channel Join, string[] Args)
        {
            if (Args.Count() < 2 || Args.Count() == 2 && Args[1].ToLower() == "list")
            {
                string NList = "";

                foreach (KeyValuePair<string, DictionaryDescriptorHelper<Dictionary<string, List<string>>>> s in SoundBoardBinding)
                {
                    NList += "\n" + s.Key + " -> " + s.Value.Descriptor;
                }

                Chat.SendMessage("Let me help you with a list!:kissing_heart:\nThere are " + SoundBoardBinding.Keys.Count + " tree(s) to pick from." + NList + "\n!@soundboard list [the tree] -> for more");
            }
            else if (Args[1].ToLower() == "list" && SoundBoardBinding.ContainsKey(Args[2].ToLower()))
            {
                string NList = "";

                foreach (KeyValuePair<string, List<string>> s in SoundBoardBinding[Args[2]].Value)
                {
                    NList += "\n" + s.Key;
                }

                Chat.SendMessage("Let me help you with a list!:kissing_heart:\nHere is the list for " + SoundBoardBinding[Args[2].ToLower()].Descriptor + "." + NList + "\n!@soundboard [the tree] [the key] -> to play");
            }
            else if (Args.Count() == 3)
            {
                if (Join != null)
                {
                    if (SoundBoardBinding.ContainsKey(Args[1].ToLower()) && SoundBoardBinding[Args[1].ToLower()].Value.ContainsKey(Args[2].ToLower()))
                        SendAudioFile(ServerLight, Join, PathGetter.GetSoundBoardPath(SoundBoardBinding[Args[1].ToLower()].Value[Args[2].ToLower()][Rand.Next(SoundBoardBinding[Args[1].ToLower()].Value[Args[2].ToLower()].Count)]));
                    else
                        Chat.SendMessage("Sorry but you may have to double check your tree or sound keys");
                }
                else
                    Chat.SendMessage("Sorry but you are not in a voice chat");
             }
        }


        

        /// <summary>
        /// joins a channel as async
        /// </summary>
        /// <param name="testout"></param>
        async void joinChannel(ulong ServerLight, Channel testout)
        {
            await TJoinChannel(ServerLight, testout);
        }

        /// <summary>
        /// the under call;
        /// </summary>
        /// <param name="ServerLight"></param>
        /// <param name="testout"></param>
        /// <returns></returns>
        async Task TJoinChannel(ulong ServerLight, Channel testout)
        {
            //grab an audio client and store
            IAudioClient Temp = await _DiscordClient.GetService<AudioService>().Join(testout);
            if (Radios.ContainsKey(ServerLight))
            {
                await Radios[ServerLight].Disconnect();
                Radios[ServerLight] = Temp;
            }
            else
                Radios.Add(ServerLight, Temp);
        }

        async void DisconectChannel(ulong ServerLight)
        {
            await Radios[ServerLight].Disconnect();
            Radios[ServerLight] = null;
        }

        async void TestSendAudio(ulong ServerLight, Channel testout)
        {
           await SendAudioFileInternal(ServerLight, testout, PathGetter.GetSoundBoardPath("Recording.m4a"));
        }

        async void SendAudioFromString(ulong ServerLight, Channel testout, string text)
        {

            MemoryStream TheStream = SpeechHelper.BeginRead(text);
            await SendAudioBuffer(ServerLight, testout, TheStream);


            TheStream?.Dispose();

        }

        async void SendAudioFile(ulong ServerLight, Channel testout, string Path)
        {
            await SendAudioFileInternal(ServerLight, testout, Path);
        }

        async Task SendAudioFileInternal(ulong ServerLight, Channel testout, string Path)
        {
            var channelCount = _DiscordClient.GetService<AudioService>().Config.Channels; // Get the number of AudioChannels our AudioService has been configured to use.
            //added a test to see if we need to

            //grab an audio client and store
            IAudioClient Temp = await _DiscordClient.GetService<AudioService>().Join(testout);
            if (Radios.ContainsKey(ServerLight))
               Radios[ServerLight] = Temp;
            else
               Radios.Add(ServerLight, Temp);

            try
            {
                var OutFormat = new WaveFormat(48000, 16, channelCount); // Create a new Output Format, using the spec that Discord will accept, and with the number of channels that our client supports.

                using (var MP3Reader = new AudioFileReader(Path)) // Create a new Disposable MP3FileReader, to read audio from the filePath parameter
                using (var resampler = new MediaFoundationResampler(MP3Reader, OutFormat)) // Create a Disposable Resampler, which will convert the read MP3 data to PCM, using our Output Format
                {
                    resampler.ResamplerQuality = 60; // Set the quality of the resampler to 60, the highest quality
                    int blockSize = OutFormat.AverageBytesPerSecond / 50; // Establish the size of our AudioBuffer
                    byte[] buffer = new byte[blockSize];
                    int byteCount;

                    while ((byteCount = resampler.Read(buffer, 0, blockSize)) > 0) // Read audio into our buffer, and keep a loop open while data is present
                    {
                        if (byteCount < blockSize)
                        {
                            // Incomplete Frame
                            for (int i = byteCount; i < blockSize; i++)
                                buffer[i] = 0;
                        }

                        Radios[ServerLight].Send(buffer, 0, blockSize); // Send the buffer to Discord
                    }
                }
            }
            catch
            { }
        }

        async Task SendAudioBuffer(ulong ServerLight, Channel testout, Stream Buffer, int rate = 48000)
        {
            try
            {
                var channelCount = _DiscordClient.GetService<AudioService>().Config.Channels; // Get the number of AudioChannels our AudioService has been configured to use.

            //grab an audio client and store
            IAudioClient Temp = await _DiscordClient.GetService<AudioService>().Join(testout);
            if (Radios.ContainsKey(ServerLight))
                  Radios[ServerLight] = Temp;
            else
                  Radios.Add(ServerLight, Temp);
            

            var OutFormat = new WaveFormat(rate, 16, channelCount); // Create a new Output Format, using the spec that Discord will accept, and with the number of channels that our client supports.
            using (var resampler = new RawSourceWaveStream(Buffer, OutFormat)) // Create a Disposable Resampler, which will convert the read MP3 data to PCM, using our Output Format
            {
                resampler.Position = 0;
                int blockSize = OutFormat.AverageBytesPerSecond; // Establish the size of our AudioBuffer
                byte[] buffer = new byte[blockSize];
                int byteCount;

                while ((byteCount = resampler.Read(buffer, 0, blockSize)) > 0) // Read audio into our buffer, and keep a loop open while data is present
                {
                    if (byteCount < blockSize)
                    {
                        // Incomplete Frame
                        for (int i = byteCount; i < blockSize; i++)
                            buffer[i] = 0;
                    }

                    Radios[ServerLight].Send(buffer, 0, blockSize); // Send the buffer to Discord

                    
                }
            }
            }
            catch
            { }
            
        }

        ////will use for streaming later
        //async void SendAudio(ulong ServerLight, Channel testout, string pathOrUrl, int rate = 48000)
        //{
        //    var channelCount = _DiscordClient.GetService<AudioService>().Config.Channels; // Get the number of AudioChannels our AudioService has been configured to use.

        //    //grab an audio client and store
        //    IAudioClient Temp = await _DiscordClient.GetService<AudioService>().Join(testout);
        //    if (Radios.ContainsKey(ServerLight))
        //        Radios[ServerLight] = Temp;
        //    else
        //        Radios.Add(ServerLight, Temp);

        //    var process = Process.Start(new ProcessStartInfo
        //    { // FFmpeg requires us to spawn a process and hook into its stdout, so we will create a Process
        //        FileName = "ffmpeg",
        //        Arguments = "-i "+ pathOrUrl + " " + // Here we provide a list of arguments to feed into FFmpeg. -i means the location of the file/URL it will read from
        //                    "-f s16le -ar "+ rate + " -ac 2 pipe:1", // Next, we tell it to output 16-bit 48000Hz PCM, over 2 channels, to stdout.
        //        UseShellExecute = false,
        //        RedirectStandardOutput = true // Capture the stdout of the process
        //    });
        //    Thread.Sleep(2000); // Sleep for a few seconds to FFmpeg can start processing data.

        //    int blockSize = 3840; // The size of bytes to read per frame; 1920 for mono
        //    byte[] buffer = new byte[blockSize];
        //    int byteCount;

        //    while (true) // Loop forever, so data will always be read
        //    {
        //        byteCount = process.StandardOutput.BaseStream // Access the underlying MemoryStream from the stdout of FFmpeg
        //                .Read(buffer, 0, blockSize); // Read stdout into the buffer

        //        if (byteCount == 0) // FFmpeg did not output anything
        //            break; // Break out of the while(true) loop, since there was nothing to read.

        //        Radios[ServerLight].Send(buffer, 0, byteCount); // Send our data to Discord
        //    }
        //    Radios[ServerLight].Wait(); // Wait for the Voice Client to finish sending data, as ffMPEG may have already finished buffering out a song, and it is unsafe to return now.
        //}


#if DEBUG
        async void GetSCTracks(MessageEventArgs e, string[] Args, int Number)
        {
            await _GetSCTracks(e, Args, Number);
        }


        async Task _GetSCTracks(MessageEventArgs e, string[] Args, int Number)
        {
            string temp = Args[1];
            for (int i = 3; i < Args.Count(); i++)
            {
                temp += " " + Args[i];
            }
            trackResults SCTracks = await SCClient.SearchTrack(temp);
            if (SCTracks.Count == 0)
                await e.Channel.SendMessage("Sorry, but no Matches were found!");
            for (int i = 0; i < Number && i < SCTracks.Count; i++)
            {
                await e.Channel.SendMessage((i + 1) + "." + SCTracks[i]);
                Thread.Sleep(20);
            }
        }

        async void SoundCloudStream(MessageEventArgs e, string[] Args, ulong ServerLight, Channel testout)
        {
            await _SoundCloudStream(e, Args, ServerLight, testout);
        }

        async Task _SoundCloudStream(MessageEventArgs e, string[] Args, ulong ServerLight, Channel testout)
        {
            string temp = Args[1];
            for (int i = 3; i < Args.Count(); i++)
            {
                temp += " " + Args[i];
            }
            trackResults SCTracks = await SCClient.SearchTrack(temp);
            int j = 0;
            while (j < SCTracks.Count && SCTracks[j]?.streamable == false)
            {
                j++;
            }
            if (j < SCTracks.Count)
            {
                Stream OS = new MemoryStream();
                Stream S = await SCClient.GetStream(SCTracks[j].id, _DiscordClient.GetService<AudioService>().Config.Channels);
                using (WaveStream waveStream = WaveFormatConversionStream.CreatePcmStream(new Mp3FileReader(S)))
                using (WaveFileWriter waveFileWriter = new WaveFileWriter(OS, waveStream.WaveFormat))
                {
                    byte[] bytes = new byte[waveStream.Length];
                    waveStream.Position = 0;
                    waveStream.Read(bytes, 0, (int)OS.Length);
                    waveFileWriter.Write(bytes, 0, bytes.Length);
                    waveFileWriter.Flush();
                }
                await SendAudioBuffer(ServerLight, testout, OS);
                S.Dispose();
                OS.Dispose();
            }
        }
#endif


    }
}
