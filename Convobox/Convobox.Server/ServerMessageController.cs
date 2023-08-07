using System.IO.Pipes;

namespace Convobox.Server;

public static class ServerMessageController
{
    public static void Validate(CommandMessge commandMsg,int clientId)
    {
        switch (commandMsg.Type)
        {
            case CommandType.LoginReq:
                bool loggedIn = false;
                Console.WriteLine($"[INFO][Account] Client {clientId} wants to log in");
                try
                {
                    loggedIn = DatabaseManager.CheckLogin(commandMsg.UserData);
                }
                catch (Exception e)
                {
                    
                }

                if (loggedIn)
                {
                    ServerConversationManager.ClientAuth[clientId] = commandMsg.UserData;
                    var loginSuccess = new CommandMessge();
                    loginSuccess.Type = CommandType.LoginSuccess;
                    loginSuccess.UserData = DatabaseManager.GetUser(commandMsg.UserData.Name);
                    ServerConversationManager.SendMessage(loginSuccess,clientId);
                    Console.WriteLine($"[INFO][Account] Client {clientId} successfully logged in");
                }
                else
                {
                    ServerConversationManager.SendMessage(new CommandMessge(CommandType.LoginError),clientId);
                    Console.WriteLine($"[ERROR][Account] Client {clientId} failed to log in");
                }

                break;
            case CommandType.RegisterReq:
                bool registered = false;
                Console.WriteLine($"[INFO][Account] Client {clientId} wants to register");
                try
                {
                    registered = DatabaseManager.CreateUser(commandMsg.UserData);
                }
                catch (Exception e)
                {
                    
                }

                if (registered)
                {
                    ServerConversationManager.ClientAuth[clientId] = commandMsg.UserData;
                    var registerSuccess = new CommandMessge(CommandType.RegisterSuccess);   
                    registerSuccess.UserData = DatabaseManager.GetUser(commandMsg.UserData.Name);
                    ServerConversationManager.SendMessage(registerSuccess,clientId);
                    Console.WriteLine($"[INFO][Account] Client {clientId} successfully registered");
                }
                else
                {
                    ServerConversationManager.SendMessage(new CommandMessge(CommandType.RegisterError),clientId);
                    Console.WriteLine($"[ERROR][Account] Client {clientId} failed to register");
                }
                break;
            case CommandType.MessagesReq:
                Console.WriteLine($"[INFO][Account] Client {clientId} requested history of {commandMsg.Amount} messages");
                var list = DatabaseManager.GetLastMessages(commandMsg.Amount);
                var messagesRep = new CommandMessge()
                {
                    Messages = list,
                    Type = CommandType.MessagesRep
                };
                ServerConversationManager.SendMessage(messagesRep,clientId);
                Console.WriteLine($"[INFO][Account] Sent client {clientId} history of {list.Count} messages");
                break;
                
        }
    }
}