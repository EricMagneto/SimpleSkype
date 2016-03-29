using Skype4Sharp;
using System;

namespace PluginInterface
{
    public interface Plugin
    {
        string Name { get; }
        string Author { get; }
        string Description { get; }
        string[] Triggers { get; }
        string Syntax { get; }
        UserRank RequiredRank { get; }
        string InvalidRankMessage { get; }
        void Run(ChatMessage rMessage, ChatMessage pMessage, string[] commandArgs);
    }
    public class InvalidSyntaxException : Exception
    {
        public InvalidSyntaxException() { }
        public InvalidSyntaxException(string message) : base(message) { }
        public InvalidSyntaxException(string message, Exception inner) : base(message, inner) { }
    }
    public enum UserRank
    {
        Owner,
        Admin,
        Moderator,
        Ultimate,
        Premium,
        Normal,
        Banned
    }
}