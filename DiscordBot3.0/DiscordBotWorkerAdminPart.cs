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


namespace DiscordBot3._0
{
    //Part for admin
    partial class DiscordBotWorker
    {

        /// <summary>
        /// useful admin tools
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private bool AdminTree(MessageEventArgs e)
        {
            string[] Args = ArgMaker(e.Message.Text.Remove(0, 2));
            if (Args[0].ToLower() == "help")
                e.Channel.SendMessage("Thanks for asking!:kissing_heart:\nFor Admin Commands.:cocktail:\n!#inviteme -> Generates a link to invite her to your server\n!#clean -> Cleans messages (tell it how many)\n!#broadcast -> Broadcast to all channels.\n!#bbroadcast -> Bold broadcast to all channels.\n!#onlinesayhi -> Should Say Hi when on you come online (off / on)");
            else if (Args[0].ToLower() == "inviteme")
                e.Channel.SendMessage(":wave: Hello there! :wave: \nI am Kaname Chidori:kissing_heart:\nUse this link to invite me to your server:love_letter:\n" + @"https://discordapp.com/oauth2/authorize?&client_id=192361072039165953&scope=bot&permissions=194552229183750144");
            else if (Args[0].ToLower() == "broadcast")
            {
                if (e.User.ServerPermissions.Administrator)
                {
                    string message = "";
                    for (int i = 1; i < Args.Length; i++)
                        message += Args[i] + " ";
                    foreach (Channel c in e.Server.TextChannels)
                        c.SendMessage(message);
                }
                else
                {
                    e.Channel.SendMessage("I am sorry but you do not have the clearance for this!:lock::gun:");
                    e.Channel.SendFile(PathGetter.GetImagePath("images3.jpg"));
                }
            }
            else if (Args[0].ToLower() == "bbroadcast")
            {
                if (e.User.ServerPermissions.Administrator)
                {
                    string message = "";
                    for (int i = 1; i < Args.Length; i++)
                        message += Args[i] + " ";
                    foreach (Channel c in e.Server.TextChannels)
                        c.SendMessage("**" + message + "**");
                }
                else
                {
                    e.Channel.SendMessage("I am sorry but you do not have the clearance for this!:lock::gun:");
                    e.Channel.SendFile(PathGetter.GetImagePath("images3.jpg"));
                }

            }
            else if (Args.Count() == 2 && Args[0].ToLower() == "clean")
            {
                if (e.User.ServerPermissions.Administrator)
                {
                    Thread.Sleep(20);
                    int num;
                    if (int.TryParse(Args[1], out num))
                    {
                        Task<Message[]> test = e.Channel.DownloadMessages(num);
                        test.Wait();
                        if (test.IsCompleted)
                        {
                            Message[] ToDel = test.Result;
                            for (int i = 1; i < ToDel.Length; i++)
                            {
                                ToDel[i].Delete();
                            }
                        }
                    }
                    else
                        e.Channel.SendMessage("Tell me how many to remove and it will be done.");
                }
                else
                {
                    e.Channel.SendMessage("I am sorry but you do not have the clearance for this!:lock::gun:");
                    e.Channel.SendFile(PathGetter.GetImagePath("images3.jpg"));
                }
            }
            else if (Args.Count() == 2 && Args[0].ToLower() == "onlinesayhi")
            {
                if (e.User.ServerPermissions.Administrator)
                {
                    if (Args[1].ToLower() == "off")
                        OnlineSayHi = false;
                    else if (Args[1].ToLower() == "on")
                        OnlineSayHi = true;
                }
                else
                {
                    e.Channel.SendMessage("I am sorry but you do not have the clearance for this!:lock::gun:");
                    e.Channel.SendFile(PathGetter.GetImagePath("images3.jpg"));
                }
            }
            else if (Args[0].ToLower() == "onlinesayhi")
                e.Channel.SendMessage("Please tell me a state to go into after command (on / off)");
            else return false;
            return true;
        }
    }
}
