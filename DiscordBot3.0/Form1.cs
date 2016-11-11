using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using NAudio.Wave;
using NAudio.Utils;
using NAudio;
using NAudio.Wave.SampleProviders;
using NAudio.Wave.Compression;

namespace DiscordBot3._0
{
    /// <summary>
    /// the posible ways to write to the console
    /// At the moment the consoles to write to
    /// </summary>
    public enum ConsoleWriteType
    {
        Basic,
        BotStatus,
        ServerInfo
    };


    public partial class Form1 : Form
    {
        /// <summary>
        /// SoundBoard Catagory bindings saved in the object to be light wieght and kep it stright
        /// </summary>
        class SBCat
        {
            /// <summary>
            /// Value you will use to call it
            /// </summary>
            public string Key { private set; get; }
            /// <summary>
            /// Description for the thing
            /// </summary>
            public string Dec { private set; get; }

            /// <summary>
            /// Creates a catagory
            /// </summary>
            /// <param name="_Key">The key to call it</param>
            /// <param name="_Dec">The description</param>
            public SBCat(string _Key, string _Dec)
            {
                Key = _Key;
                Dec = _Dec;
            }

            /// <summary>
            /// Gets the string to get it from
            /// </summary>
            /// <returns>The string to display</returns>
            public override string ToString()
            {
                return "Key:" + Key + "-:-Dec:" + Dec;
            }

            /// <summary>
            /// gets if the key ar copped to prevent redundences key checking is more important here as multi tress can not exist under one tree but mult sounds can be random from one sound key
            /// </summary>
            /// <param name="obj">the object ot test</param>
            /// <returns>if the key alreay exists</returns>
            public override bool Equals(object obj)
            {
                if (!(obj is SBCat)) return false;
                return this.Key.Equals(((SBCat)obj).Key);
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }
        }

        /// <summary>
        /// SoundBoard bindings saved in the object to be light wieght and kep it stright is linked to the Cats va refs
        /// </summary>
        class SBSound
        {
            /// <summary>
            /// The catagory the thing is called from
            /// </summary>
            public SBCat Cat { private set; get; }
            /// <summary>
            /// Value you will use to call it
            /// </summary>
            public string Key { private set; get; }
            /// <summary>
            /// File name not including The full path Will programaticly generally
            /// </summary>
            public string File { private set; get; }

            /// <summary>
            /// Creates a sound binding
            /// </summary>
            /// <param name="_Key">The key to call it</param>
            /// <param name="_Dec">The description</param>
            public SBSound(SBCat _Cat, string _Key, string _File)
            {
                Cat = _Cat;
                Key = _Key;
                File = _File;
            }

            /// <summary>
            /// Gets the string to get it from
            /// </summary>
            /// <returns>The string to display</returns>
            public override string ToString()
            {
                return "CatKey:" + Cat.Key + "-:-Key:" + Key + "-:-File:" + File;
            }

            /// <summary>
            /// gets if the paths ar copped to prevent redundences mult sounds can exist under one key but redundant paths must be remove.
            /// </summary>
            /// <param name="obj">the object ot test</param>
            /// <returns>if the path alreay exists</returns>
            public override bool Equals(object obj)
            {
                if (!(obj is SBSound)) return false;
                return this.File.Equals(((SBSound)obj).File);
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }

        }

        /// <summary>
        /// List of bound Memes
        /// </summary>
        public Dictionary<string, string> MemeList { private set; get; } = new Dictionary<string, string>();

        /// <summary>
        /// To Fetch our over complicated Monstrosity 
        /// ie Grabs the sound BoardList
        /// </summary>
        public Dictionary<string, DictionaryDescriptorHelper<Dictionary<string, List<string>>>> SoundBoardBinding
        { get {
                Dictionary<string, DictionaryDescriptorHelper<Dictionary<string, List<string>>>> Temp = new Dictionary<string, DictionaryDescriptorHelper<Dictionary<string, List<string>>>>();
                //temp builder
                // makes a catagory
                for (int i = 0; i < _LB_SoundBoardSoundsBinder.Items.Count; i++)
                {
                    SBSound temp = (SBSound)_LB_SoundBoardSoundsBinder.Items[i];
                    if (!Temp.ContainsKey(temp.Cat.Key))
                    {
                        DictionaryDescriptorHelper<Dictionary<string, List<string>>> t1 = new DictionaryDescriptorHelper<Dictionary<string, List<string>>>(new Dictionary<string, List<string>>(), temp.Cat.Dec);
                        Temp.Add(temp.Cat.Key, t1);
                    }

                    // makes a sound binding in cat
                    if (!Temp[temp.Cat.Key].Value.ContainsKey(temp.Key))
                    {
                        List<string> t2 = new List<string>(new List<string>());
                        Temp[temp.Cat.Key].Value.Add(temp.Key, t2);
                    }

                    Temp[temp.Cat.Key].Value[temp.Key].Add(temp.File);
                }

                return Temp;
            } }

        /// <summary>
        /// Value to set
        /// </summary>
        static bool _Mute = true;

        /// <summary>
        /// Should we auto connect
        /// </summary>
        bool _AutoConnect = false;

        /// <summary>
        /// Did the restart happen with a crash
        /// </summary>
        bool _Crashed = false;

        /// <summary>
        /// A Property to set Mute
        /// </summary>
        public static bool Mute { get { return _Mute; } set { _Mute = value; } }

        /// <summary>
        /// var to tell us to record from mics
        /// </summary>
        static bool _Record = false;

        /// <summary>
        /// A Property to set _Record
        /// </summary>
        public static bool Record { get { return _Record; } set { _Record = value; } }

        /// <summary>
        /// a delegate for use with sending strings to the consoles
        /// used by:
        /// Console write
        /// </summary>
        /// <param name="output"> the string to send</param>
        /// <param name="_Type">the Console in question</param>
        public delegate void voidStringConsoleWriteType(string output, ConsoleWriteType _Type = ConsoleWriteType.Basic);

        /// <summary>
        /// delagate for callbacks with the consoles 
        /// used by:
        /// Clear Console
        /// </summary>
        /// <param name="Colour">the Console in question</param>
        public delegate void voidConsoleWriteType(ConsoleWriteType Type = ConsoleWriteType.Basic);

        /// <summary>
        /// a call back for the consoles to be updated with added info
        /// </summary>
        public voidStringConsoleWriteType ConsoleWrite { private set; get; }

        /// <summary>
        /// a callback for clearing the console
        /// </summary>
        public voidConsoleWriteType ClearConsole { private set; get; }

        /// <summary>
        /// the token we would like the bot to use
        /// </summary>
        public string UseToken { get { return _TB_BotTokenSet.Text; } }

        /// <summary>
        /// our main client or con that handles discord commands and player interations
        /// </summary>
        DiscordBotWorker _Client;

        /// <summary>
        /// Grabs the music folder from the form
        /// </summary>
        public string MusicFolderPath { get { return _TB_MusicFolderSet.Text; } }


        public Form1()
        {
            InitializeComponent();
            //set the different delegates
            ConsoleWrite = new voidStringConsoleWriteType(_PrintBotStatusOut);
            ClearConsole = new voidConsoleWriteType(_ClearConsole);
        }

        /// <summary>
        /// a constructor for leveraging a auto restart
        /// </summary>
        /// <param name="Auto"></param>
        /// <param name="Crashed"></param>
        public Form1(bool Auto, bool Crashed = false) : this()
        {
            _AutoConnect = Auto;
            _Crashed = Crashed;
        }

        /// <summary>
        /// Event for the loaded form to use
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender, EventArgs e)
        {
            // we will try loading the list of games to pick from 
            try
            {
                //the file is a simple text file so grab a stream reader as it has a nice line by line streamer and a file end bool
                using (StreamReader Reader = new StreamReader(new FileStream(Environment.CurrentDirectory + "\\GameToPlay.text", FileMode.OpenOrCreate)))
                {
                    //as long as we have lines load a line in as a new game item
                    while (!Reader.EndOfStream)
                        _LB_GamesToPlay.Items.Add(Reader.ReadLine());
                    Reader.Close(); // close the stream for the next app and clean up
                    Reader.Dispose();
                }

            }
            catch
            {
                // if we didnt find the thing or had a problem just make a primitve array of sample games to use
                string[] GamesToPayPrimitive = { null, "Guns of Icarus Online", "Half Life 3", "Valkyria Chronicles™" ,
            "Gun Wings", "Ghost in the Shell Stand Alone Complex First Assault Online", "Agarest: Generations of War" , "ArcheAge", "Portal 2",
            "Fallout 4", "DARK SOULS™ II", "ArcheAge", "Sid Meier's Civilization V", "Age of Empires II: HD Edition", "Garry's Mod", "Zombie Grinder",
            "Team Fortress 2", "Life is Feudal: Your Own", "Alien Swarm", "Counter-Strike: Global Offensive", "STAR WARS™: Knights of the Old Republic™" };
                //microsoft wtf no to array of the items well do it the old fastioned way
                foreach (string s in GamesToPayPrimitive)
                {
                    _LB_GamesToPlay.Items.Add(s);
                }
            }

            try
            {
                //the file is a simple text file so grab a stream reader as it has a nice line by line streamer and a file end bool
                using (StreamReader Reader = new StreamReader(new FileStream(Environment.CurrentDirectory + "\\SoundBoard.text", FileMode.OpenOrCreate)))
                {
                    //as long as we have lines load a line in as a new game item
                    while (!Reader.EndOfStream)
                    {
                        // The lines alternate between keys and their description for catagories so we read alternating lines for them
                        SBCat Cat = new SBCat(Reader.ReadLine(), Reader.ReadLine());
                        // this uses the compare (value model) to to see if we have it in the list
                        // if it is already in the dictionary grab that key and use it to prevent us from using the wrong ref to bind sounds to the key
                        if (_LB_SoundBoardCatagoryBinder.Items.Contains(Cat))
                            Cat = (SBCat)_LB_SoundBoardCatagoryBinder.Items[_LB_SoundBoardCatagoryBinder.Items.IndexOf(Cat)];
                        else // if not add a key as a new uniqe key
                            _LB_SoundBoardCatagoryBinder.Items.Add(Cat);
                        // the next we bound a sound with the next two lines and our cat. cat, key, file
                        SBSound Sound = new SBSound(Cat, Reader.ReadLine(), Reader.ReadLine());
                        // use our sound to fully bind it
                        _LB_SoundBoardSoundsBinder.Items.Add(Sound);
                    }
                    Reader.Close(); // close the stream for the next app and clean up
                    Reader.Dispose();
                }

            }
            catch
            {
                //do Nothing if it fails as this jut means we must rebind on exit
            }

            try
            {
                //the file is a simple text file so grab a stream reader as it has a nice line by line streamer and a file end bool
                using (StreamReader Reader = new StreamReader(new FileStream(Environment.CurrentDirectory + "\\MemeList.text", FileMode.OpenOrCreate)))
                {
                    //as long as we have lines load a line in as a new game item
                    while (!Reader.EndOfStream)
                    {
                        KeyValuePair<string, string> Temp = new KeyValuePair<string, string>(Reader.ReadLine(), Reader.ReadLine());
                        MemeList.Add(Temp.Key, Temp.Value);
                        _LB_GiffList.Items.Add(Temp.Key + "-:-" + Temp.Value);
                    }
                    Reader.Close(); // close the stream for the next app and clean up
                    Reader.Dispose();
                }

            }
            catch
            {
                //do Nothing if it fails as this jut means we must rebind on exit
            }

            //foreach(System.Speech.Synthesis.InstalledVoice R in new System.Speech.Synthesis.SpeechSynthesizer().GetInstalledVoices())
            //_RTB_ConsoleOut.AppendText(R.VoiceInfo.Name);

            if (_Crashed)
                _RTB_ConsoleOut.AppendText("This IS a FULL System crash RESTART! CHECK THE LOGS DING BAT -> "+ Environment.CurrentDirectory + "\\KillLog.text");

            if (_AutoConnect)
                _Bu_Connect_Click(sender, e);

        }

        /// <summary>
        /// exit event button (for saving the later required info)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                // call a Disconnect to prevent hanging
                _Bu_Disconnect_Click(sender, e);

                //grab a file stream for th games list
                using (StreamWriter Saver = File.CreateText(Environment.CurrentDirectory + "\\GameToPlay.text"))
                {
                    // write a line for each game
                    foreach (string s in _LB_GamesToPlay.Items)
                    {
                        Saver.WriteLine(s);
                    }
                    Saver.Close(); // let the stream go and dispose
                    Saver.Dispose();
                }
                //grab a file stream for th bindings list
                using (StreamWriter Saver = File.CreateText(Environment.CurrentDirectory + "\\SoundBoard.text"))
                {
                    // write a line for each game
                    foreach (SBSound s in _LB_SoundBoardSoundsBinder.Items)
                    {
                        // for each sound write a line
                        Saver.WriteLine(s.Cat.Key);
                        Saver.WriteLine(s.Cat.Dec);
                        Saver.WriteLine(s.Key);
                        Saver.WriteLine(s.File);
                    }
                    Saver.Close(); // let the stream go and dispose
                    Saver.Dispose();
                }
                using (StreamWriter Saver = File.CreateText(Environment.CurrentDirectory + "\\MemeList.text"))
                {
                    // write a line for each game
                    foreach (KeyValuePair<string, string> p in MemeList)
                    {
                        // for each sound write a line
                        Saver.WriteLine(p.Key);
                        Saver.WriteLine(p.Value);
                    }
                    Saver.Close(); // let the stream go and dispose
                    Saver.Dispose();
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// a function for use with a delegate to clear a console
        /// </summary>
        /// <param name="_Type">the type of console to use</param>
        void _ClearConsole(ConsoleWriteType _Type = ConsoleWriteType.Basic)
        {
            // based on selected console(app has three)
            switch (_Type)
            {
                case ConsoleWriteType.BotStatus:
                    //invoke a second delegate from this thread to avoid a cross thread violation to clear
                    Invoke(new Action(() => { _RTB_BotStatusOut.Clear(); }));
                    break;
                case ConsoleWriteType.Basic:
                    Invoke(new Action(() => { _RTB_ConsoleOut.Clear(); }));
                    break;
                case ConsoleWriteType.ServerInfo:
                    Invoke(new Action(() => { _RTB_ServerInfoOut.Clear(); }));
                    break;
            }
        }

        /// <summary>
        /// a function for use with a delegate to print to console
        /// </summary>
        /// <param name="_output">the string to send out</param>
        /// <param name="_Type">the console to target</param>
        void _PrintBotStatusOut(string _output, ConsoleWriteType _Type = ConsoleWriteType.Basic)
        {
            // based on selected console(app has three)
            switch (_Type)
            {
                case ConsoleWriteType.BotStatus:
                    //invoke a second delegate from this thread to avoid a cross thread violation to Append on a line
                    Invoke(new Action(() => { _RTB_BotStatusOut.AppendText(_output + "\n"); }));
                    break;
                case ConsoleWriteType.Basic:
                    Invoke(new Action(() => { _RTB_ConsoleOut.AppendText(_output + "\n"); }));
                    break;
                case ConsoleWriteType.ServerInfo:
                    Invoke(new Action(() => { _RTB_ServerInfoOut.AppendText(_output + "\n"); }));
                    break;
            }
        }

        /// <summary>
        /// basic button connect event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _Bu_Connect_Click(object sender, EventArgs e)
        {
            // first check if we have a connection as it would be redundent to do if we have one
            // also to prevent hanging memory
            if (_Client != null) return; //if not null simply return
            
            //quickly build a array of values from the listbox
            string[] temp = new string[_LB_GamesToPlay.Items.Count + 1];
            temp[0] = null; // we dont aways play games so leave a null for if we dont
            int i = 1; // start one off as well the extra is null
            foreach (string s in _LB_GamesToPlay.Items)
            {
                temp[i] = s;
                i++;
            }

            // connect using this as the control and with the array of games
            _Client = new DiscordBotWorker(this, temp, SoundBoardBinding);
            Thread.Sleep(200);

        }

        /// <summary>
        /// simple disconect button event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _Bu_Disconnect_Click(object sender, EventArgs e)
        {
            // just disconnect if not null and house clean
            _Client?._DiscordClient?.Disconnect();
            _Client?._DiscordClient?.Dispose();
            _Client = null; // set the old client for clean up and neatly nulled

        }

        /// <summary>
        /// A simple add games button event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _Bu_AddToGamesToPlay_Click(object sender, EventArgs e)
        {
            // if it isnt in the list and it is not empty add it
            if (_TB_AddGameToPlayList.Text != "" && !_LB_GamesToPlay.Items.Contains(_TB_AddGameToPlayList.Text))
                _LB_GamesToPlay.Items.Add(_TB_AddGameToPlayList.Text);
            // if we are ready 
            if(_Client != null)// lock the var to set
            lock(DiscordBotWorker.GamesToPlay)
            {
                // quickly build a list from the list oie microsoft there is a fully comented example else were
                string[] temp = new string[_LB_GamesToPlay.Items.Count + 1];
                temp[0] = null;
                int i = 1;
                foreach (string s in _LB_GamesToPlay.Items)
                {
                    temp[i] = s;
                    i++;
                }
                //set it
                DiscordBotWorker.GamesToPlay = temp;
            }
        }

        /// <summary>
        /// removes a catagoty binding and all its children
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _Bu_RemoveFromGamesToPlay_Click(object sender, EventArgs e)
        {
            //if we have not selected a value return
            if (_LB_GamesToPlay.SelectedIndex == -1) return;
            // or else remove the index and set none selected for safety
            _LB_GamesToPlay.Items.RemoveAt(_LB_GamesToPlay.SelectedIndex);
            _LB_GamesToPlay.SelectedIndex = -1;
            if (_Client != null)// repeat of the the second half above
            lock (DiscordBotWorker.GamesToPlay)
            {
                string[] temp = new string[_LB_GamesToPlay.Items.Count + 1];
                temp[0] = null;
                int i = 1;
                foreach (string s in _LB_GamesToPlay.Items)
                {
                    temp[i] = s;
                    i++;
                }
                DiscordBotWorker.GamesToPlay = temp;
            }
        }

        /// <summary>
        /// button event that Grabs your systems usable outputs to capture
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _Bu_AudioRefreshSources_Click(object sender, EventArgs e)
        {
            // begin an update
            _LV_AudioDevices.BeginUpdate();

            //build a list of wave devices on the computer
            List<WaveInCapabilities> sources = new List<WaveInCapabilities>();
            List<WaveOutCapabilities> outputs = new List<WaveOutCapabilities>();

            for (int i = 0; i < WaveIn.DeviceCount; i++)
            {
                sources.Add(WaveIn.GetCapabilities(i));
            }
            for (int i = 0; i < WaveOut.DeviceCount; i++)
            {
                outputs.Add(WaveOut.GetCapabilities(i));
            }

            // clear and print out these devices to see
            _LV_AudioDevices.Items.Clear();

            foreach (WaveInCapabilities s in sources)
            {
                ListViewItem Insert = new ListViewItem(s.ProductName);
                Insert.SubItems.Add(new ListViewItem.ListViewSubItem(Insert, s.Channels.ToString()));
                Insert.SubItems.Add(new ListViewItem.ListViewSubItem(Insert, "In"));
                _LV_AudioDevices.Items.Add(Insert);
            }
            foreach (WaveOutCapabilities s in outputs)
            {
                ListViewItem Insert = new ListViewItem(s.ProductName);
                Insert.SubItems.Add(new ListViewItem.ListViewSubItem(Insert, s.Channels.ToString()));
                Insert.SubItems.Add(new ListViewItem.ListViewSubItem(Insert, "Out"));
                _LV_AudioDevices.Items.Add(Insert);
            }

            


            _LV_AudioDevices.EndUpdate();
        }


        private void _Bu_AudioStreamSelectOut_Click(object sender, EventArgs e)
        {
            if (_LV_AudioDevices.SelectedItems.Count == 0) return;

        }

        public WaveOut Recorder { get; private set; }
        public DirectSoundOut Streamer { get; private set; }

        private void _Bu_AudioStreamSelectIn_Click(object sender, EventArgs e)
        {
            if (_LV_AudioDevices.SelectedItems.Count == 0)
            {
                MessageBox.Show("Please Select a device that is a In from the box");
                return;
            }
            int index = _LV_AudioDevices.SelectedItems[0].Index;
            if (index > WaveIn.DeviceCount)
            {
                MessageBox.Show("Please Select a device that is a In from the box");
                return;
            }
            

        }

        private void _Bu_AudioStreamMute_Click(object sender, EventArgs e)
        {
            Mute = !Mute;
            if (Mute)
                _Bu_AudioStreamMute.Text = "Unmute";
            else
                _Bu_AudioStreamMute.Text = "Mute";
        }

        private void _Bu_AudioStreamRecord_Click(object sender, EventArgs e)
        {
            Record = !Record;
            if (Record)
                _Bu_AudioStreamRecord.Text = "Stop Rec";
            else
                _Bu_AudioStreamRecord.Text = "Record";
        }

        /// <summary>
        /// Cleans out text if console gets too long
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _RTB_ConsoleOut_TextChanged(object sender, EventArgs e)
        {
            if (_RTB_ConsoleOut.TextLength > 2000000000)
            {
                int NumRemove = _RTB_ConsoleOut.Text.IndexOf('\n');
                _RTB_ConsoleOut.Text.Remove(0, NumRemove+ 1);
            }
            _RTB_ConsoleOut.Select(_RTB_ConsoleOut.TextLength - 1, 0);
            _RTB_ConsoleOut.ScrollToCaret();
        }

        /// <summary>
        /// executes command
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _Bu_Excute_Click(object sender, EventArgs e)
        {
            _RTB_ConsoleOut.AppendText("User : " + _TB_CommandPrompt.Text + "\n");

            string[] args = DiscordBotWorker.ArgMaker(_TB_CommandPrompt.Text);
            if (args[0].ToLower() == "sim" && _Client != null)
                _Client.MasterAndCommander.Invoke(args);
            else if (args[0].ToLower() == "help")
                _RTB_ConsoleOut.AppendText("There are these commands\nsim [server] [channel] <order strings>-> sends a sim message to discord though the bot");
            
        }

        /// <summary>
        /// executes command on shift
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _TB_CommandPrompt_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                _Bu_Excute_Click(sender, e as EventArgs);
        }

        /// <summary>
        /// Shows the text box chang to let the person know he has selected stuff
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _LB_SoundBoardCatagoryBinder_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_LB_SoundBoardCatagoryBinder.SelectedIndex != -1)
                _TB_SoundBoardSouCat.Text = _LB_SoundBoardCatagoryBinder.Text;
        }

        /// <summary>
        /// Adds a catagory
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _Bu_SoundBoardCatagoryAdd_Click(object sender, EventArgs e)
        {
            if (_TB_SoundBoardCatKey.Text != "" && _TB_SoundBoardCatDes.Text != "")
            {
                SBCat Temp = new SBCat(_TB_SoundBoardCatKey.Text, _TB_SoundBoardCatDes.Text);
                if (!_LB_SoundBoardCatagoryBinder.Items.Contains(Temp))
                    _LB_SoundBoardCatagoryBinder.Items.Add(Temp);
                else
                    MessageBox.Show("Can not Duplicate Key");
            }
        }

        /// <summary>
        /// removes a cat and its branches
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _Bu_SoundBoardCatagoryRemove_Click(object sender, EventArgs e)
        {
            if (_LB_SoundBoardCatagoryBinder.SelectedIndex == -1) return;
            
            for (int i = 0; i < _LB_SoundBoardSoundsBinder.Items.Count; i++)
            {
                SBSound temp = (SBSound)_LB_SoundBoardSoundsBinder.Items[i];
                if (temp.Cat.Equals(_LB_SoundBoardCatagoryBinder.Items[_LB_SoundBoardCatagoryBinder.SelectedIndex]))
                    _LB_SoundBoardSoundsBinder.Items.RemoveAt(i);
            }

            _LB_SoundBoardCatagoryBinder.Items.RemoveAt(_LB_SoundBoardCatagoryBinder.SelectedIndex);
            _Client?.SoundBoardListUpDate.Invoke();
        }

        /// <summary>
        /// adds a file to the sound board
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _Bu_SoundBoardSoundAdd_Click(object sender, EventArgs e)
        {
            if (_LB_SoundBoardCatagoryBinder.SelectedIndex != -1 && _TB_SoundBoardSouCat.Text != "" && _TB_SoundBoardSouFil.Text != "")
            {
                SBSound Temp = new SBSound((SBCat)_LB_SoundBoardCatagoryBinder.Items[_LB_SoundBoardCatagoryBinder.SelectedIndex], _TB_SoundBoardSouKey.Text, _TB_SoundBoardSouFil.Text);
                if (!_LB_SoundBoardSoundsBinder.Items.Contains(Temp) && File.Exists(PathGetter.GetSoundBoardPath(Temp.File)))
                    _LB_SoundBoardSoundsBinder.Items.Add(Temp);
                else
                    MessageBox.Show("Can not Duplicate File or File does not exist in _SoundBoard_ folder");
            }
            _Client?.SoundBoardListUpDate.Invoke();
        }

        /// <summary>
        /// removes a file from sound board
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _Bu_SoundBoardSoundRemove_Click(object sender, EventArgs e)
        {
            if (_LB_SoundBoardSoundsBinder.SelectedIndex == -1) return;
            _LB_SoundBoardSoundsBinder.Items.RemoveAt(_LB_SoundBoardSoundsBinder.SelectedIndex);
            _Client?.SoundBoardListUpDate.Invoke();
        }

        private void _Bu_AddGiff_Click(object sender, EventArgs e)
        {
            if (_TB_GiffKeys.Text != "" && _TB_GiffPath.Text != "")
            {
                if (!MemeList.ContainsKey(_TB_GiffKeys.Text) && !MemeList.ContainsValue(_TB_GiffPath.Text))
                {
                    if (File.Exists(PathGetter.GetMemePath(_TB_GiffPath.Text)))
                    {
                        MemeList.Add(_TB_GiffKeys.Text.ToLower(), _TB_GiffPath.Text);
                        _LB_GiffList.Items.Add(_TB_GiffKeys.Text.ToLower() + "-:-" + _TB_GiffPath.Text);
                    }
                    else
                        MessageBox.Show("that file does not exist");
                }
                else
                    MessageBox.Show("You have ether the file or the key already included");
            }
            else
                MessageBox.Show("You have a blank field");

        }

        private void _Bu_RemoveGiff_Click(object sender, EventArgs e)
        {
            if (_LB_GiffList.SelectedIndex == -1) return;
            MemeList.Remove((((string)(_LB_GiffList.SelectedItem)).Substring(0, ((string)(_LB_GiffList.SelectedItem)).IndexOf("-:-"))));
            _LB_GiffList.Items.RemoveAt(_LB_GiffList.SelectedIndex);
            
        }
    }
}
