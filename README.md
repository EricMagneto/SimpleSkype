# SimpleSkype
Modular, plugin based Skype bot utilizing Skype4Sharp

By reading this project's source code, compiling as a binary, redistributing assets found in this repository, etc, or ANY form of use, you must agree to the license enlisted below.

As this project uses [Skype4Sharp] (https://github.com/lin-e/Skype4Sharp/), which itself has some documentation, please visit there for additional usage information. The documentation provided here is solely for the bot itself.

# Official Forum Posts (others may be fake)
- [HackForums] (http://hackforums.net/showthread.php?tid=5201320)
- [LeakForums] (https://leakforums.net/thread-692877)

# Directory Structure
```
.
`-- SimpleSkype
    |-- SimpleSkype.exe
    |-- Skype4Sharp.dll
    |-- Newtonsoft.Json.dll
    |-- PluginInterface.dll
    |-- plugins
    |   |-- source
    |   |   |-- foobar.cs
    |   |   `-- example.cs
    |   |-- foobar.dll
    |   `-- example.dll
    `-- ranks
        |-- owners.txt
        |-- banned.txt
        |-- admins.txt
        |-- moderators.txt
        |-- ultimates.txt
        `-- premiums.txt
```

# Making a Plugin
### Example
Note, I will use '#' to note an actual comment, since // is used in the compiler.

Also, this plugin makes no sense, it's just to show what's possible.
```C#
//ref System.Web.dll # This adds a reference to an external assembly, which is not included by default in the compiler.
using System;
using PluginInterface;
using Skype4Sharp;
using System.Web;
using SimpleSkype;

namespace pluginBuild
{
    public class your_plugin : Plugin
    {
        public string Name { get { return "Foo"; } }
        public string Description { get { return "Great plugin!"; } }
        public string Author { get { return "YourNameHere"; } }
        public string[] Triggers { get { return new string[] { "foo", "bar" }; } }
        public string Syntax { get { return "<message>"; } }
        public UserRank RequiredRank { get { return UserRank.Premium; } }
        public string InvalidRankMessage { get { return "Please buy premium to get access to this command!"; } }
        public void Run(ChatMessage rMessage, ChatMessage pMessage, string[] commandArgs)
        {
            if (commandArgs.Length > 1)
            {
                SimpleSkype.Program.botName = HttpUtility.UrlEncode(pMessage.Body.Remove(0, commandArgs[0].Length + 1)); //# This literally makes no sense. 'botName' isn't even a variable.
                rMessage.Body = "You've changed the bot's name! I guess.";
            }
            else
            {
                throw new InvalidSyntaxException();
            }
        }
    }
}
```
### User Ranks
Presented in order of 'power'
```C#
UserRank.Owner
UserRank.Admin
UserRank.Moderator
UserRank.Ultimate
UserRank.Premium
UserRank.Normal
UserRank.Banned
```
### Adding references to external libraries
Put this at the top of the file (the comments). It's a comment anyways, so the compiler won't do anything with it.
```C#
//ref System.Web.dll
//ref ChatterBotAPI.dll
```
### Checking syntax
Don't forget that the command trigger itself is also an argument
```C#
if (commandArgs.Length == 2)
{
    rMessage.Body = "Well done! You have used syntax correctly.";
}
else
{
    throw new InvalidSyntaxException();
}
```
