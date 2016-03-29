using PluginInterface;
using Skype4Sharp;
using Skype4Sharp.Enums;
using System;

namespace SimpleSkype
{
    #region defaultPlugins
    public class Help : Plugin
    {
        public string Name { get { return "Help"; } }
        public string Description { get { return "Shows the help menu"; } }
        public string Author { get { return "Commodity"; } }
        public string[] Triggers { get { return new string[] { "help", "cmd", "commands" }; } }
        public string Syntax { get { return null; } }
        public UserRank RequiredRank { get { return UserRank.Normal; } }
        public string InvalidRankMessage { get { return null; } }
        public void Run(ChatMessage rMessage, ChatMessage pMessage, string[] commandArgs)
        {
            string helpMenu = "Available commands are: ";
            foreach (Plugin singlePlugin in Program.loadedPlugins)
            {
                helpMenu += Environment.NewLine + "    " + breakDownPlugin(singlePlugin);
            }
            rMessage.Body = helpMenu;
        }
        string breakDownPlugin(Plugin inputPlugin)
        {
            return string.Format("{0}{1}{2} - {3} ('{4}' by {5})", Program.triggerString, string.Join(", ", inputPlugin.Triggers), ((inputPlugin.Syntax == null) ? "" : " " + inputPlugin.Syntax), inputPlugin.Description, inputPlugin.Name, inputPlugin.Author);
        }
    }
    public class Credits : Plugin
    {
        public string Name { get { return "Credits"; } }
        public string Description { get { return "People who made this possible"; } }
        public string Author { get { return "Commodity"; } }
        public string[] Triggers { get { return new string[] { "credits", "about" }; } }
        public string Syntax { get { return null; } }
        public UserRank RequiredRank { get { return UserRank.Normal; } }
        public string InvalidRankMessage { get { return null; } }
        public void Run(ChatMessage rMessage, ChatMessage pMessage, string[] commandArgs)
        {
            rMessage.Type = MessageType.RichText;
            rMessage.Body = "About this bot:" + Environment.NewLine
            + "    API: <a href=\"https://github.com/lin-e/Skype4Sharp\">Skype4Sharp</a>" + Environment.NewLine
            + "    Creator: <a href=\"https://twitter.com/c0mmodity\">Commodity</a>" + Environment.NewLine
            + "    Project: <a href=\"https://github.com/lin-e/SimpleSkype\">SimpleSkype</a>";
        }
    }
    #endregion
}
