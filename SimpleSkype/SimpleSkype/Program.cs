using System;
using System.Collections.Generic;
using System.Linq;
using PluginInterface;
using Skype4Sharp;
using Skype4Sharp.Auth;
using Skype4Sharp.Enums;
using Skype4Sharp.Events;
using Skype4Sharp.Exceptions;
using Skype4Sharp.Helpers;
using System.Threading;
using System.Net;
using System.Diagnostics;
using System.IO;

namespace SimpleSkype
{
    public class Program
    {
        public static List<Plugin> loadedPlugins;
        public static Dictionary<string, Plugin> commandMap = new Dictionary<string, Plugin>();
        public static string triggerString = "!";
        public static SkypeCredentials authCreds;
        public static Skype4Sharp.Skype4Sharp mainSkype;
        static void Main(string[] args)
        {
            #region rankChecks
            if (!(Directory.Exists("ranks")))
            {
                Directory.CreateDirectory("ranks");
            }
            string[] neededFiles = { "owners.txt", "admins.txt", "moderators.txt", "ultimates.txt", "premiums.txt", "banned.txt" };
            foreach (string singleFile in neededFiles)
            {
                if (!(File.Exists(string.Format("ranks/{0}", singleFile))))
                {
                    File.Create(string.Format("ranks/{0}", singleFile));
                }
            }
            Ranks.loadDictionary();
            #endregion
            Console.WriteLine(string.Join("", Enumerable.Repeat("_", Console.WindowWidth)));
            Stopwatch allTimer = new Stopwatch();
            allTimer.Start();
            loadedPlugins = new Loader().Load().ToList();
            loadedPlugins.Insert(0, (Plugin)Activator.CreateInstance(typeof(Credits))); // Credit menu. Added at start so it has highest priority (basically just keep it there. Developers deserve credit.)
            loadedPlugins.Add((Plugin)Activator.CreateInstance(typeof(Help))); // Default menu. Added at end, so that it has lowest priority
            foreach (Plugin loadedPlugin in loadedPlugins)
            {
                foreach (string singleTrigger in loadedPlugin.Triggers)
                {
                    string toAdd = singleTrigger.ToLower();
                    if (commandMap.ContainsKey(toAdd))
                    {
                        Console.WriteLine("[PLUGINS]: Trigger conflict! '{0}' in {1}, conflicts with {2}.", toAdd, loadedPlugin.Name, commandMap[toAdd].Name);
                    }
                    else
                    {
                        commandMap.Add(toAdd, loadedPlugin);
                    }
                }
            }
            allTimer.Stop();
            Console.WriteLine(string.Format("[PLUGINS]: {0} plugin{1} loaded in {2}ms", loadedPlugins.Count.ToString(), ((loadedPlugins.Count == 1) ? "" : "s" ), allTimer.ElapsedMilliseconds.ToString()));
            Console.WriteLine(string.Join("", Enumerable.Repeat("_", Console.WindowWidth)));
            authCreds = new SkypeCredentials("tafabot.exe", "abcdefg226400*123abc");
            mainSkype = new Skype4Sharp.Skype4Sharp(authCreds);
            Console.WriteLine("[ LOGIN ]: Logging in with {0}:{1}", authCreds.Username, string.Join("", Enumerable.Repeat("*", authCreds.Password.Length)));
            if (mainSkype.Login())
            {
                Console.WriteLine("[ LOGIN ]: Login complete");
                mainSkype.messageReceived += MainSkype_messageReceived;
                mainSkype.contactRequestReceived += MainSkype_contactRequestReceived;
                mainSkype.StartPoll();
                Console.WriteLine("[ LOGIN ]: Poll started");
            }
            else
            {
                Console.WriteLine("[ ERROR ]: Login failed");
            }
            Console.WriteLine(string.Join("", Enumerable.Repeat("_", Console.WindowWidth)));
            while (true) { }
        }
        private static void MainSkype_contactRequestReceived(ContactRequest sentRequest)
        {
            new Thread(() =>
            {
                Console.WriteLine("[ EVENT ]: REQUEST_RECEIVED > {0} has added you with the message '{1}'.", sentRequest.Sender.Username, sentRequest.Body);
                sentRequest.Accept();
                sentRequest.Sender.SendMessage(string.Format("Thanks for adding me! Use '{0}help' for a list of commands!", triggerString));
            }).Start();
        }
        private static void MainSkype_messageReceived(ChatMessage pMessage)
        {
            new Thread(() =>
            {
                try
                {
                    Console.WriteLine("[ EVENT ]: MESSAGE_RECEIVED > {0} ({2}): {1}", pMessage.Sender.Username, pMessage.Body, pMessage.Chat.ID);
                    string[] commandArgs = pMessage.Body.Split(' ');
                    if (commandArgs[0].ToLower().StartsWith(triggerString))
                    {
                        ChatMessage rMessage = pMessage.Chat.SendMessage("Processing your command...");
                        WebClient webClient = new WebClient();
                        bool apiError = false;
                        string toCheck = commandArgs[0].Remove(0, triggerString.Length).ToLower();
                        try
                        {
                            if (commandMap.ContainsKey(toCheck))
                            {
                                try
                                {
                                    if (Ranks.canRun(commandMap[toCheck], pMessage.Sender))
                                    {
                                        commandMap[toCheck].Run(rMessage, pMessage, commandArgs);
                                    }
                                    else
                                    {
                                        rMessage.Body = (commandMap[toCheck].InvalidRankMessage == null) ? "You do not have the required permissions to run this command." : commandMap[toCheck].InvalidRankMessage;
                                    }
                                }
                                catch (Exception thrownException)
                                {
                                    if (thrownException is InvalidSyntaxException)
                                    {
                                        rMessage.Body = string.Format("Invalid syntax! The correct syntax for this command is '{0}{1} {2}'", triggerString, toCheck, commandMap[toCheck].Syntax);
                                    }
                                    else
                                    {
                                        throw thrownException;
                                    }
                                }
                            }
                            else
                            {
                                rMessage.Body = string.Format("Invalid command! Use '{0}help' for a list of commands.", triggerString);
                            }
                        }
                        catch
                        {
                            apiError = true;
                        }
                        if (apiError)
                        {
                            rMessage.Type = MessageType.RichText;
                            rMessage.Body = "<font color=\"#b30000\"><b>Error in processing your command</b></font>";
                        }
                        Console.WriteLine("[ EVENT ]: MESSAGE_SENT > {0} ({2}): {1}", rMessage.Sender.Username, rMessage.Body, rMessage.Chat.ID);
                    }
                }
                catch { }
            }).Start();
        }
    }
}
