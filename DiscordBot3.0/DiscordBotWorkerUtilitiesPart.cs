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
    // the utilites part for
    partial class DiscordBotWorker
    {

        /// <summary>
        /// The anime list getter
        /// </summary>
        MyAnimeListClient _MyAnimeListClient;

        /// <summary>
        /// The utility tree for utility commands
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private bool UtiliTree(MessageEventArgs e)
        {
            string[] Args = ArgMaker(e.Message.Text.Remove(0, 2));
            if (Args[0] == "help")
                e.Channel.SendMessage("Thanks for asking!:kissing_heart:\nFor utilitie Commands.:wrench:\n!!d4 -> rolls dice\n!!d6 -> rolls dice\n!!d8 -> rolls dice\n!!d10 -> rolls dice\n!!d12 -> rolls dice\n!!d20 -> rolls dice\n!!d100 -> rolls dice\n!!Coinflip -> I flip a coin for you baka:military_medal:\n!!Random -> I roll a random number for you (can be between two numbers):slot_machine:");
            else if (Args.Count() > 2 && Args[0].ToLower() == "animelist")
            {
                int Number;
                if (int.TryParse(Args[1], out Number))
                    GetAnimes(e, Args, Number);
                else
                    e.Channel.SendMessage("Please give me the number of results to return.\nYou can follow this with the anime.\nExample !!animelist 1 And you thought there is never a girl online?");
            }
            else if (Args.Count() > 2 && Args[0].ToLower() == "mangalist")
            {
                int Number;
                if (int.TryParse(Args[1], out Number))
                    GetMangas(e, Args, Number);
                else
                    e.Channel.SendMessage("Please give me the number of results to return.\nYou can follow this with the manga.\nExample !!mangalist 1 And you thought there is never a girl online?");
            }
            else if (Args[0].ToLower() == "coinflip")
            {
                if (Rand.Next(0, 101) < 50)
                    e.Channel.SendMessage("I flipped a coin for " + e.User.Mention + ". It came up heads.:military_medal:");
                else
                    e.Channel.SendMessage("I flipped a coin for " + e.User.Mention + ". It came up tails.:military_medal:");
            }
            else if (Args.Count() == 3 && Args[0].ToLower() == "random")
            {
                int Max, Min;
                if (int.TryParse(Args[2], out Max) && int.TryParse(Args[1], out Min))
                    e.Channel.SendMessage("I Rolled a random number for " + e.User.Mention + ". It came up " + Rand.Next(Min, Max + 1) + ":slot_machine:.");
                else
                    e.Channel.SendMessage("Did You mean to use numbers for the max and min");
            }
            else if (Args.Count() == 2 && Args[0].ToLower() == "random")
            {
                int Max;
                if (int.TryParse(Args[1], out Max))
                    e.Channel.SendMessage("I Rolled a random number for " + e.User.Mention + ". It came up " + Rand.Next(Max + 1) + ":slot_machine:.");
                else
                    e.Channel.SendMessage("Did You mean to use a number for the max");
            }
            else if (Args[0].ToLower() == "random")
            {
                e.Channel.SendMessage("I Rolled a random number for " + e.User.Mention + ". It came up " + Rand.Next() + ":slot_machine:.");
            }
            else if (Args[0].ToLower() == "d4")
            {
                e.Channel.SendMessage("I Rolled a 4 sided dice for " + e.User.Mention + ". It came up " + Rand.Next(1, 5) + ":slot_machine:.");
            }
            else if (Args[0].ToLower() == "d6")
            {
                e.Channel.SendMessage("I Rolled a 6 sided dice for " + e.User.Mention + ". It came up " + Rand.Next(1, 7) + ":slot_machine:.");
            }
            else if (Args[0].ToLower() == "d8")
            {
                e.Channel.SendMessage("I Rolled a 8 sided dice for " + e.User.Mention + ". It came up " + Rand.Next(1, 9) + ":slot_machine:.");
            }
            else if (Args[0].ToLower() == "d10")
            {
                e.Channel.SendMessage("I Rolled a 10 sided dice for " + e.User.Mention + ". It came up " + Rand.Next(1, 11) + ":slot_machine:.");
            }
            else if (Args[0].ToLower() == "d12")
            {
                e.Channel.SendMessage("I Rolled a 12 sided dice for " + e.User.Mention + ". It came up " + Rand.Next(1, 13) + ":slot_machine:.");
            }
            else if (Args[0].ToLower() == "d20")
            {
                e.Channel.SendMessage("I Rolled a 20 sided dice for " + e.User.Mention + ". It came up " + Rand.Next(1, 21) + ":slot_machine:.");
            }
            else if (Args[0].ToLower() == "d100")
            {
                e.Channel.SendMessage("I Rolled a 100 sided dice for " + e.User.Mention + ". It came up " + Rand.Next(1, 101) + ":slot_machine:.");
            }
            else return false;
            return true;
        }

        async void GetAnimes(MessageEventArgs e, string[] Args, int Number)
        {
            string temp = Args[2];
            for (int i = 3; i < Args.Count(); i++)
            {
                temp += " " + Args[i];
            }
            AnimeResult anime = await _MyAnimeListClient.SearchAnime(temp);
            if (anime.Count == 0)
                await e.Channel.SendMessage("Sorry, but no Matches were found!");
            for (int i = 0; i < Number && i < anime.Count; i++)
            {
                await e.Channel.SendMessage((i + 1) + "." + anime[i]);
                Thread.Sleep(10);
            }
        }

        async void GetMangas(MessageEventArgs e, string[] Args, int Number)
        {
            string temp = Args[2];
            for (int i = 3; i < Args.Count(); i++)
            {
                temp += " " + Args[i];
            }
            MangaResult manga = await _MyAnimeListClient.SearchManga(temp);
            if (manga.Count == 0)
                await e.Channel.SendMessage("Sorry, but no Matches were found!");
            for (int i = 0; i < Number && i < manga.Count; i++)
            {
                await e.Channel.SendMessage((i + 1) + "." + manga[i]);
                Thread.Sleep(20);
            }
        }

    }
}
