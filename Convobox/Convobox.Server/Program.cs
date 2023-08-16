using System;
using SharedDefinitions;

namespace Convobox.Server
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ServerConversationManager.ServerInfo = new ServerInfo()
            {
                Domain = Definition.DefaultDomain,
                PortCommunication = Definition.DefaultPortCommunication,
                PortFiles = Definition.DefaultPortFiles
            };
            
            FileHoster.Start();
            CryptographyManager.Init();
            DatabaseManager.SetDatabaseLocation();
            ServerConversationManager.Start();
            
        }
    }
}