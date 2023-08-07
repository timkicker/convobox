using System;

namespace Convobox.Server
{
    internal class Program
    {
        static void Main(string[] args)
        {
            DatabaseManager.SetDatabaseLocation();
            ServerConversationManager.Start();
        }
    }
}