using System;

namespace Convobox.Server
{
    internal class Program
    {
        static void Main(string[] args)
        {
            CryptographyManager.Init();
            DatabaseManager.SetDatabaseLocation();
            ServerConversationManager.Start();
        }
    }
}