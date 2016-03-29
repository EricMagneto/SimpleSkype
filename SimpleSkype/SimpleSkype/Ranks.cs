using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using PluginInterface;
using Skype4Sharp;

namespace SimpleSkype
{
    class Ranks
    {
        static Dictionary<UserRank, string[]> rankDictionary = new Dictionary<UserRank, string[]>();
        public static bool canRun(Plugin runningCommand, User commandSender)
        {
            if (getRank(commandSender.Username) <= runningCommand.RequiredRank)
            {
                return true;
            }
            return false;
        }
        public static void loadDictionary()
        {
            rankDictionary.Clear();
            rankDictionary.Add(UserRank.Owner, File.ReadAllLines("ranks/owners.txt"));
            rankDictionary.Add(UserRank.Banned, File.ReadAllLines("ranks/banned.txt"));
            rankDictionary.Add(UserRank.Admin, File.ReadAllLines("ranks/admins.txt"));
            rankDictionary.Add(UserRank.Moderator, File.ReadAllLines("ranks/owners.txt"));
            rankDictionary.Add(UserRank.Ultimate, File.ReadAllLines("ranks/owners.txt"));
            rankDictionary.Add(UserRank.Premium, File.ReadAllLines("ranks/owners.txt"));
        }
        static UserRank getRank(string inputUser)
        {
            foreach (UserRank listedRank in rankDictionary.Keys)
            {
                if (rankDictionary[listedRank].Contains(inputUser))
                {
                    return listedRank;
                }
            }
            return UserRank.Normal;
        }
    }
}
