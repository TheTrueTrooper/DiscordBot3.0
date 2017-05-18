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
using RestSharp.Extensions;
using static DiscordBot3._0.Form1;

namespace DiscordBot3._0
{
    class ServerSettings
    {
        public ServerSettings(Server s)
        {
            _Server = s;
            SayHello = false;
            AnnounceChannel = Server.TextChannels.First().Name;
            LogChannel = null;
        }

        

        public Server Server { get { return _Server; } }

        Server _Server;

        public ulong ServerID { get { return Server.Id; } }

        public bool SayHello { get; set; }

        public string AnnounceChannel { get; set; }

        public string LogChannel { get; set; }
    }


    // the main orign file for
    // has the constructor tick and other basic basic utilities
    partial class DiscordBotWorker
    {
        /// <summary>
        /// a list of servers for per server
        /// </summary>
        SortedDictionary<ulong, ServerSettings> _ServerSettings = new SortedDictionary<ulong, ServerSettings>();
        /// <summary>
        /// a store of old commands so we can reque them if needed
        /// </summary>
        SortedDictionary<ulong, MessageEventArgs> UsersLast = new SortedDictionary<ulong, MessageEventArgs>();
        /// <summary>
        /// a list of game we can play
        /// </summary>
        public static string[] GamesToPlay { get; set; }
        /// <summary>
        /// our ticker object
        /// </summary>
        static public System.Timers.Timer Ticker { private set; get; }

        /// <summary>
        /// Our Random Number generator one static across bots should be good
        /// </summary>
        static Random Rand = new Random();

        /// <summary>
        /// the access wraper used for getting at our friend wrapper
        /// </summary>
        public DiscordClient _DiscordClient { private set; get; }

        int TickOnNewGames = 0;
        int MaxTickOnNewGames;

        User CoolDown;
        byte CoolDownTick = 0;

        bool OnlineSayHi = false;

        Form1 _ControlPannel;

        public delegate void ControlPannelCommand(string[] DoIT);
        public delegate void VoidVoid();

        public ControlPannelCommand MasterAndCommander { private set; get; }

        ~DiscordBotWorker()
        {
            _DiscordClient.Disconnect();
        }

        static DiscordBotWorker()
        {
            Ticker = new System.Timers.Timer(20000);
            Ticker.AutoReset = true;
        }

        public DiscordBotWorker(Form1 ControlPannel, string[] _GamesToPlay,
            SoundBoardList _SoundBoardBinding)
        {
            _ControlPannel = ControlPannel;
#if DEBUG
            ConsoleWrite("is Debug build\n");
#else
            ConsoleWrite("is Chidori build\n");
#endif

            ConsoleWrite("Building Client......");
            _DiscordClient = new DiscordClient()
            .UsingAudio(x =>
            {
                x.Mode = AudioMode.Both;
                x.EnableEncryption = true;
                x.Bitrate = AudioServiceConfig.MaxBitrate;
                x.BufferLength = 10000;
            });
            //configsfor things
#if DEBUG
            //the sound cloud client config 
            SCClient = new SoundCloudClient("9789f855276cf2c76bf4c6b62a09155e");
            track.ToStringOverRide = x =>
            {
                return x != null ? "```Title: " + x.title +
                "\nID : " + x.id +
                "\nstreamable : " + x.streamable +
                "\nstream_url:" + x.stream_url + "```"
                : null;
            };
#endif
            //the anime client config
            _MyAnimeListClient = new MyAnimeListClient("The_True_Trooper", "Jim456852,.,");
            Manga.ToStringOverRide = x =>
            {
                return x != null ?
                    "```Title: " + x.title +
                    "\nEnglish: " + x.english +
                    "\nSynonyms: " + x.synonyms +
                    "\nChapters: " + x.chapters +
                    "\nVolumes: " + x.volumes +
                    "\nScore: " + x.score +
                    "\nType: " + x.type +
                    "\nStatus: " + x.status +
                    "\nStart Date: " + x.start_date +
                    "\nEnd Date: " + x.end_date +
                    "\nSynopsis: " + x.synopsis.Replace("<br />", "").Replace("[i]", "").Replace("[/i]", "").HtmlDecode() + "```" +
                    "\nhttp://myanimelist.net/manga/" + x.id
                    : null;
            };
            Anime.ToStringOverRide = x =>
            {
                return x != null ?
                    "```Title: " + x.title +
                    "\nEnglish: " + x.english +
                    "\nSynonyms: " + x.synonyms +
                    "\nEpisodes: " + x.episodes +
                    "\nScore: " + x.score +
                    "\nType: " + x.type +
                    "\nStatus: " + x.status +
                    "\nStart Date: " + x.start_date +
                    "\nEnd Date: " + x.end_date +
                    "\nSynopsis: " + x.synopsis.Replace("<br />", "").Replace("[i]", "").Replace("[/i]", "").HtmlDecode() + "```" +
                    "\nhttp://myanimelist.net/anime/" + x.id
                    : null;
            };

            //bot callbacks and event bindings
            MasterAndCommander = new ControlPannelCommand(_MasterAndCommander);
            SoundBoardListUpDate = new VoidVoid(_SoundBoardListUpDate);

            _DiscordClient.Ready += Ready;
            _DiscordClient.MessageReceived += MessRec;
            //_DiscordClient.MessageUpdated += MessRec;
            _DiscordClient.UserJoined += JoinHello;
            _DiscordClient.JoinedServer += JoinServer;
            _DiscordClient.UserUpdated += UserUpHello;
            _DiscordClient.RoleCreated += RoleCreate;
            _DiscordClient.RoleDeleted += RoleDelete;
            _DiscordClient.RoleUpdated += RoleUpdate;
            _DiscordClient.UserBanned += UserBanned;
            _DiscordClient.UserUnbanned += UserUnbanned;
            _DiscordClient.ChannelCreated += ChannelCreated;
            _DiscordClient.ChannelDestroyed += ChannelDeleted;
            _DiscordClient.ChannelUpdated += ChannelUpdated;
            

            ConsoleWrite("Building support classes......");
            //ticks info
            Ticker.Elapsed += Tick;
            MaxTickOnNewGames = Rand.Next(30);

            //list populating
            ConsoleWrite("Populating lists......");
            GamesToPlay = null;
            lock (GamesToPlay)
                GamesToPlay = GamesToPlay.Concat(_GamesToPlay).ToArray();
            SoundBoardBinding = null;
            lock (SoundBoardBinding)
                SoundBoardBinding = _SoundBoardBinding;

            Connect();

        }


        private async void Connect()
        {
            int trys = 0;

            ConsoleWrite("Connecting client to server......");

            try
            {
#if DEBUG
                await _DiscordClient.Connect("MTkyMzU1Mjk4MTI2NTk0MDU4.CkHoVA.WGJ3UqihFndW0w-Ch01Sih04L6o", TokenType.Bot);
#else
                await _DiscordClient.Connect(_ControlPannel.UseToken, TokenType.Bot);
#endif
                ConsoleWrite("Connected starting up full......");
            }
            catch
            {
                if (trys < 10)
                {
                    trys++;
                    ConsoleWrite("Failed to connect. Retring attempt " + trys + " of 10 in 2000.");
                    Thread.Sleep(2000);
#if DEBUG
                    await _DiscordClient.Connect("MTkyMzU1Mjk4MTI2NTk0MDU4.CkHoVA.WGJ3UqihFndW0w-Ch01Sih04L6o", TokenType.Bot);
#else
                    await _DiscordClient.Connect(_ControlPannel.UseToken, TokenType.Bot);
#endif
                }

                ConsoleWrite("Failed to connect. Plz Retry......");
            }
        }

        private void Ready(object sender, EventArgs e)
        {
            Thread.Sleep(40);
            
            UpdateConsole();

            Ticker.Start();

            ConsoleWrite("Full start Completed");
        }

        private void JoinServer(object sender, ServerEventArgs e)
        {
#if DEBUG
            foreach (Channel c in e.Server.TextChannels)
                c.SendMessage(":wave: Hello there! :wave: \nI am Kaname Chidori Debug Version:kissing_heart:\nThat mean im not done. So prob am full of bugs and mis-prints:poop:\nI was written in C# using discord.Net\nBy The_Trooper(steam) or DefinitelyNotMarkiplier(Discord) aka Angelo :100:\nThanks for the invite. I hope I can be usefull and we can have lots of fun!:dancer:");
#else
            foreach (Channel c in e.Server.TextChannels)
                c.SendMessage(":wave: Hello there! :wave: \nI am Kaname Chidori:kissing_heart:\nI was written in C# using discord.Net\nBy The_Trooper(Steam) or DefinitelyNotMarkiplier(Discord) aka Angelo :100:\nThanks for the invite. I hope I can be usefull and we can have lots of fun!:dancer:");
#endif
        }

        private void Tick(object sender, ElapsedEventArgs e)
        {
            
            if (CoolDownTick > 2)
            {
                CoolDownTick = 0;
                CoolDown = null;
            }
            else if (CoolDown != null)
                CoolDownTick++;

            TickOnNewGames++;
            if (TickOnNewGames > MaxTickOnNewGames)
            {
                TickOnNewGames = 0;
                MaxTickOnNewGames = Rand.Next(10, 30);
                lock(GamesToPlay)
                    _DiscordClient.SetGame(GamesToPlay[Rand.Next(0, GamesToPlay.Count() -1)]);
            }

            foreach (Server s in _DiscordClient.Servers)
            {
                if(Radios.ContainsKey(s.Id))
                    Radios[s.Id].VoiceSocket?.SendHeartbeat();
            }

            _DiscordClient.Servers.Where(x => !_ServerSettings.ContainsKey(x.Id)).ToList().ForEach(x => _ServerSettings.Add(x.Id, new ServerSettings(x)));
            //_ServerSettings.Keys.Except(_DiscordClient.Servers.)

            UpdateConsole();
        }

        private void JoinHello(object sender, UserEventArgs e)
        {
            if (e.User.IsBot) return;
            e.User.Channels.First().SendMessage(":wave: Hello there! " + e.User.Mention + "! :wave:\n Welcome to the server\n Admins are in green and are as follows: Only DefinitelyNotMarkiplier matters as he mad me");

        }

        private void UserUpHello(object sender, UserUpdatedEventArgs e)
        {
            if (e.Before.IsBot) return;
            if(OnlineSayHi && CoolDown?.Name != e.After.Name && e.Before.Status == UserStatus.Offline && e.After.Status == UserStatus.Online)
            {
                CoolDown = e.After;
                if (DateTime.Now.Hour < 12)
                    e.After.SendMessage("Good morning " + e.After.Mention + ":wave: hope you are having a great day");
                else
                    e.After.SendMessage("Good afternoon " + e.After.Mention + ":wave: hope you are having a great day");
            }
        }

        private void MessRec(object sender, MessageEventArgs e)
        {
            bool BDoIKnow = false;
           
            if (e.Message.IsAuthor)
                  return;

            if (e.Message.Text.First() == '!' && e.Message.Text.Count() > 1)
            {
#if !DEBUG
                if (!e.Message.Text.ToLower().StartsWith("!~chat"))
                    e.Message.Delete();
#endif

                if (e.Message.Text.ToLower() == "!help")
                {
                    e.Channel.SendMessage("Thanks for asking!:kissing_heart:\nThere are 4 Catagories.:ok_hand:\n!~help for chat commands\n!!help for utilitie Commands\n!@help for DJ Commands\n!#help for admin Commands\nNote some commands need proper athority\nThere is also a !again command to repeat last.");
                    BDoIKnow = true;
                }
                else if (e.Message.Text.ToLower() == "!update")
                    UpdateConsole();
                else if (e.Message.Text.ToLower() == "!again")
                {
                    BDoIKnow = true;
                    if (UsersLast.ContainsKey(e.Message.User.Id) && UsersLast[e.Message.User.Id].Message.Text != "!again")
                    {
                            MessRec(sender, UsersLast[e.Message.User.Id]);
                    }
                    else
                        e.Channel.SendMessage("Sorry;\nBut I dont have a last action for you yet:dancer:");
                }
                else if (e.Message.Text[1] == '!')
                    BDoIKnow = UtiliTree(e);
                else if (e.Message.Text[1] == '@')
                    BDoIKnow = MusicTree(e);
                else if (e.Message.Text[1] == '#')
                    BDoIKnow = AdminTree(e);
                else if (e.Message.Text[1] == '~')
                    BDoIKnow = ChatTree(e);
                if (!BDoIKnow)
                    e.Channel.SendMessage("Sorry;\nI dont follow,\nbut its not a problem:dancer:");
                else if (e.Message.User != null && e.Message.Text.ToLower() != "!again")
                {
                    if (!UsersLast.ContainsKey(e.Message.User.Id))
                        UsersLast.Add(e.Message.User.Id, e);
                    else
                        UsersLast[e.User.Id] = e;
                }
            }
        }

        

        //admin logging

        void RoleCreate(object Sender, RoleEventArgs e)
        {
            if (_ServerSettings[e.Server.Id].LogChannel != null)
            {
                if (e.Server.FindChannels(_ServerSettings[e.Server.Id].LogChannel, ChannelType.Text, true).ToList().Count == 0)
                    e.Server.CreateChannel(_ServerSettings[e.Server.Id].LogChannel, ChannelType.Text);
                string Admins = "\nAdmins on:";
                e.Server.Users.Where(x => { return x.Roles.Any(y => y.Permissions.BanMembers || y.Permissions.Administrator) && x.Status.Value == UserStatus.Online; }).ToList().ForEach(x => { Admins += "\n" + x.Name; });
                e.Server.FindChannels(_ServerSettings[e.Server.Id].LogChannel, ChannelType.Text).First().SendMessage("```" + e.Role + " was created at " + DateTime.UtcNow + " utc" + Admins + "```");
            }
        }

        void RoleDelete(object Sender, RoleEventArgs e)
        {
            if (_ServerSettings[e.Server.Id].LogChannel != null)
            {
                if (e.Server.FindChannels(_ServerSettings[e.Server.Id].LogChannel, ChannelType.Text, true).ToList().Count == 0)
                    e.Server.CreateChannel(_ServerSettings[e.Server.Id].LogChannel, ChannelType.Text);
                string Admins = "\nAdmins on:";
                e.Server.Users.Where(x => { return x.Roles.Any(y => y.Permissions.BanMembers || y.Permissions.Administrator) && x.Status.Value == UserStatus.Online; }).ToList().ForEach(x => { Admins += "\n" + x.Name; });
                e.Server.FindChannels(_ServerSettings[e.Server.Id].LogChannel, ChannelType.Text).First().SendMessage("```" + e.Role + " was Deleted at " + DateTime.UtcNow + " utc" + Admins + "```");
            }
        }

        void RoleUpdate(object Sender, RoleUpdatedEventArgs e)
        {
            if (_ServerSettings[e.Server.Id].LogChannel != null)
            {
                if (_ServerSettings[e.Server.Id].LogChannel != null)
                {
                    if (e.Server.FindChannels(_ServerSettings[e.Server.Id].LogChannel, ChannelType.Text, true).ToList().Count == 0)
                        e.Server.CreateChannel(_ServerSettings[e.Server.Id].LogChannel, ChannelType.Text);
                    string Admins = "\nAdmins on:";
                    e.Server.Users.Where(x => { return x.Roles.Any(y => y.Permissions.BanMembers || y.Permissions.Administrator) && x.Status.Value == UserStatus.Online; }).ToList().ForEach(x => { Admins += "\n" + x.Name; });
                    e.Server.FindChannels(_ServerSettings[e.Server.Id].LogChannel, ChannelType.Text).First().SendMessage("```" + e.Before + " was Updated to " + e.After + " at " + DateTime.UtcNow + " utc" + Admins + "```");
                }
            }
        }

        void UserBanned(object Sender, UserEventArgs e)
        {
            if (_ServerSettings[e.Server.Id].LogChannel != null)
            {
                if (e.Server.FindChannels(_ServerSettings[e.Server.Id].LogChannel, ChannelType.Text, true).ToList().Count == 0)
                    e.Server.CreateChannel(_ServerSettings[e.Server.Id].LogChannel, ChannelType.Text);
                string Admins = "\nAdmins on:";
                e.Server.Users.Where(x => { return x.Roles.Any(y => y.Permissions.BanMembers || y.Permissions.Administrator) && x.Status.Value == UserStatus.Online; }).ToList().ForEach(x => { Admins += "\n" + x.Name; });
                e.Server.FindChannels(_ServerSettings[e.Server.Id].LogChannel, ChannelType.Text).First().SendMessage("```" + e.User.Name + " was Banned at " + DateTime.UtcNow + Admins + " utc" + ":hammer:```");
            }
        }

        void UserUnbanned(object Sender, UserEventArgs e)
        {
            if (_ServerSettings[e.Server.Id].LogChannel != null)
            {
                if (e.Server.FindChannels(_ServerSettings[e.Server.Id].LogChannel, ChannelType.Text, true).ToList().Count == 0)
                    e.Server.CreateChannel(_ServerSettings[e.Server.Id].LogChannel, ChannelType.Text);
                string Admins = "\nAdmins on:";
                e.Server.Users.Where(x => { return x.Roles.Any(y => y.Permissions.BanMembers || y.Permissions.Administrator) && x.Status.Value == UserStatus.Online; }).ToList().ForEach(x => { Admins += "\n" + x.Name; });
                e.Server.FindChannels(_ServerSettings[e.Server.Id].LogChannel, ChannelType.Text).First().SendMessage("```" + e.User.Name + " was Unbannedd at " + DateTime.UtcNow + " utc" + Admins + ":hammer:```");
            }
        }

        void ChannelCreated(object Sender, ChannelEventArgs e)
        {
            if (_ServerSettings[e.Server.Id].LogChannel != null)
            {
                if (e.Server.FindChannels(_ServerSettings[e.Server.Id].LogChannel, ChannelType.Text, true).ToList().Count == 0)
                    e.Server.CreateChannel(_ServerSettings[e.Server.Id].LogChannel, ChannelType.Text);
                string Admins = "\nAdmins on:";
                e.Server.Users.Where(x => { return x.Roles.Any(y => y.Permissions.ManageChannels || y.Permissions.Administrator) && x.Status.Value == UserStatus.Online; }).ToList().ForEach(x => { Admins += "\n" + x.Name; });
                e.Server.FindChannels(_ServerSettings[e.Server.Id].LogChannel, ChannelType.Text).First().SendMessage("```" + e.Channel.Name + " was Created at " + DateTime.UtcNow + " utc" + Admins + ":hammer:```");
            }
        }

        void ChannelDeleted(object Sender, ChannelEventArgs e)
        {
            if (_ServerSettings[e.Server.Id].LogChannel != null)
            {
                if (e.Server.FindChannels(_ServerSettings[e.Server.Id].LogChannel, ChannelType.Text, true).ToList().Count == 0)
                    e.Server.CreateChannel(_ServerSettings[e.Server.Id].LogChannel, ChannelType.Text);
                string Admins = "\nAdmins on:";
                e.Server.Users.Where(x => { return x.Roles.Any(y => y.Permissions.ManageChannels || y.Permissions.Administrator) && x.Status.Value == UserStatus.Online; }).ToList().ForEach(x => { Admins += "\n" + x.Name; });
                e.Server.FindChannels(_ServerSettings[e.Server.Id].LogChannel, ChannelType.Text).First().SendMessage("```" + e.Channel.Name + " was Deleted at " + DateTime.UtcNow + " utc" + Admins + ":hammer:```");
            }
        }

        void ChannelUpdated(object Sender, ChannelUpdatedEventArgs e)
        {
            if (_ServerSettings[e.Server.Id].LogChannel != null)
            {
                if (e.Server.FindChannels(_ServerSettings[e.Server.Id].LogChannel, ChannelType.Text, true).ToList().Count == 0)
                    e.Server.CreateChannel(_ServerSettings[e.Server.Id].LogChannel, ChannelType.Text);
                string Admins = "\nAdmins on:";
                e.Server.Users.Where(x => { return x.Roles.Any(y => y.Permissions.ManageChannels || y.Permissions.Administrator) && x.Status.Value == UserStatus.Online; }).ToList().ForEach(x => { Admins += "\n" + x.Name; });
                e.Server.FindChannels(_ServerSettings[e.Server.Id].LogChannel, ChannelType.Text).First().SendMessage("```" + e.Before.Name + " was updated to " + e.After.Name + " at " + DateTime.UtcNow + " utc" + Admins + ":hammer:```");
            }
        }

        /////////////////////////////////////////////////////////////////////////////utility//////////////////////////////////////////////////////////

        /// <summary>
        /// writes to a console
        /// </summary>
        /// <param name="_output">the string to put out</param>
        /// <param name="_Type">the console to actually write to</param>
        void ConsoleWrite(string _output = "", ConsoleWriteType _Type = ConsoleWriteType.Basic)
        {
            _ControlPannel?.ConsoleWrite?.Invoke(_output, _Type);
        }

        /// <summary>
        /// Clears a console
        /// </summary>
        /// <param name="_type">the console to clear</param>
        void ClearConsole(ConsoleWriteType _type = ConsoleWriteType.Basic)
        {
            _ControlPannel?.ClearConsole?.Invoke(_type);
        }

        /// <summary>
        /// Rips apart the string to make command lines Basic first write
        /// </summary>
        /// <param name="test"> the string to build from</param>
        /// <returns></returns>
        public static string[] ArgMaker(string test)
        {
            string[] Args = test.Trim(' ').Split('\"');
            string[] Args2 = Args;
            List<string> FinArgs = new List<string>();
            for (int i = 0; i < Args.Length; i++)
            {
                if (i % 2 == 0 && Args[i].Length > 0)
                {
                    Args2 = Args[i].Split(' ');
                    foreach (string s in Args2)
                    {
                        if (s.Length > 0)
                            FinArgs.Add(s);
                    }
                }
                else if (i % 2 != 0 && Args[i].Length > 0)
                    FinArgs.Add(Args[i]);
            }
            return FinArgs.ToArray();
        }
        
        /// <summary>
        /// will rip apart the string to make commands that more acuratly include refs of things  needs work <--------------------------
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        //need work
        private static string[] ArgMaker(MessageEventArgs e)
        {
            List<string> FinArgs = new List<string>();
            string test = e.Message.Text.Remove(0, 2);
            foreach (User u in e.Message.MentionedUsers)
            {
                FinArgs.Add(u.Mention);
                test.IndexOf(u.Mention);
                test.Replace(u.Mention, "@");
            }
            
            string[] Args = test.Trim(' ').Split('\"');
            string[] Args2 = Args;

            for (int i = 0; i < Args.Length; i++)
            {
                if (i % 2 == 0 && Args[i].Length > 0)
                {
                    Args2 = Args[i].Split(' ');
                    foreach (string s in Args2)
                    {
                        if (s.Length > 0)
                            FinArgs.Add(s);
                    }
                }
                else if (i % 2 != 0 && Args[i].Length > 0)
                    FinArgs.Add(Args[i]);
            }
            return FinArgs.ToArray();
        }

        /// <summary>
        /// a full console update for use with tick
        /// </summary>
        void UpdateConsole()
        {
            ClearConsole(ConsoleWriteType.BotStatus);
            ClearConsole(ConsoleWriteType.ServerInfo);

            ConsoleWrite("Internet:" + _DiscordClient?.Status?.Value, ConsoleWriteType.BotStatus);
            ConsoleWrite("API Token:" + _DiscordClient?.StatusAPI?.Token, ConsoleWriteType.BotStatus);
            ConsoleWrite("Client Token:" + _DiscordClient?.ClientAPI?.Token ?? "Negative" , ConsoleWriteType.BotStatus);
            ConsoleWrite("SessionID:" + _DiscordClient?.SessionId, ConsoleWriteType.BotStatus);
            ConsoleWrite("", ConsoleWriteType.BotStatus);


            ConsoleWrite("BotsUserName:" + _DiscordClient?.CurrentUser?.Name, ConsoleWriteType.BotStatus);
            ConsoleWrite("BotsServerStatus:" + _DiscordClient?.CurrentUser?.Status, ConsoleWriteType.BotStatus);
            ConsoleWrite("BotsAvatarID:" + _DiscordClient?.CurrentUser?.AvatarId, ConsoleWriteType.BotStatus);

            ConsoleWrite("", ConsoleWriteType.BotStatus);

            ConsoleWrite("ServerCount:" + _DiscordClient?.Servers?.Count(), ConsoleWriteType.BotStatus);
            foreach (Server s in _DiscordClient?.Servers)
            {
                ConsoleWrite("", ConsoleWriteType.ServerInfo);
                ConsoleWrite("ServerName:" + s?.Name, ConsoleWriteType.ServerInfo);
                ConsoleWrite("ServerOwner:" + s?.Owner, ConsoleWriteType.ServerInfo);
                ConsoleWrite("ServerID:" + s?.Id, ConsoleWriteType.ServerInfo);
                ConsoleWrite("ServerRolls:", ConsoleWriteType.ServerInfo);
                foreach (Discord.Role R in s?.CurrentUser?.Roles)
                {
                    ConsoleWrite(R?.Name, ConsoleWriteType.ServerInfo);
                    ConsoleWrite(R?.Id.ToString(), ConsoleWriteType.ServerInfo);
                }
            }
            ConsoleWrite("Updating info (ticked)");
        }

        /// <summary>
        /// For Commands from console may do it (good movie) needs work <------------------------------------
        /// </summary>
        /// <param name="DoIT">the command</param>
        void _MasterAndCommander(string[] DoIT)
        {

            if(DoIT[0].ToLower() == "sim")
            {
                string Message = "sim";
                for (int i = 3; i < DoIT.Count(); i++)
                    Message += " " + DoIT[i];
                _DiscordClient?.FindServers(DoIT[1])?.First()?.FindChannels(DoIT[2])?.First()?.SendMessage(Message);
            }
            
        }

        
    }
}

//protected Task PrebufferTask() => Task.Run(() =>
//{
//    int red = 0;
//    byte[] buffit = new byte[3840];
//    while (!BufferCanceler.Token.IsCancellationRequested && insideFFMPEG != null && !insideFFMPEG.StandardOutput.EndOfStream)
//    {
//        //red += insideFFMPEG.StandardOutput.BaseStream.Read(buffit, 0, 3840);
//        red = insideFFMPEG.StandardOutput.BaseStream.ReadByte();
//        if (bufferQueue.Count >= bufferBytes)
//            while (bufferQueue.Count >= bufferBytes)
//                Thread.Sleep(100);
//        if (red < 0)
//            Thread.Sleep(100);
//        else
//        {
//            //foreach (byte bite in buffit) { bufferQueue.Enqueue(bite); };
//            bufferQueue.Enqueue((byte)red);
//            bytesahead--;
//            red = 0;
//        }
//    }
//});


