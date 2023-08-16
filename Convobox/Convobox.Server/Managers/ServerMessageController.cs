using System.IO.Pipes;

namespace Convobox.Server;

public static class ServerMessageController
{
    public static async Task Validate(CommandMessge commandMsg,int clientId)
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
                    var loginSuccess = new CommandMessge();
                    loginSuccess.Type = CommandType.LoginSuccess;
                    loginSuccess.UserData = DatabaseManager.GetUser(commandMsg.UserData.Name);
                    ServerConversationManager.ClientAuth[clientId] = loginSuccess.UserData;
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
                    int createdId = DatabaseManager.CreateUser(commandMsg.UserData);
                    if (createdId == 0)
                        throw new Exception("Could not create user");
                    registered = true;
                }
                catch (Exception e)
                {
                    
                }

                if (registered)
                {
                    var registerSuccess = new CommandMessge(CommandType.RegisterSuccess);   
                    registerSuccess.UserData = DatabaseManager.GetUser(commandMsg.UserData.Name);
                    ServerConversationManager.ClientAuth[clientId] = registerSuccess.UserData;
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
                    Type = CommandType.MessagesRep,
                    Amount = DatabaseManager.GetMessageCount()
                };
                
                ServerConversationManager.SendMessage(messagesRep,clientId);
                Console.WriteLine($"[INFO][Account] Sent client {clientId} history of {list.Count} messages");
                break;
            case CommandType.SendMessage:
                
                var user = ServerConversationManager.ClientAuth[clientId];
                commandMsg.UserData = user;
                commandMsg.ConvoMessage.User = user;
                commandMsg.ConvoMessage.Creation = DateTime.Now;    // do not trust client
                
                Console.WriteLine($"[INFO][Message] {commandMsg.UserData.Name} under {clientId} sent: {commandMsg.ConvoMessage.Data} ");
                commandMsg.ConvoMessage = DatabaseManager.InsertMessage(commandMsg.ConvoMessage);
                commandMsg.ConvoMessage.Base64File = "";
                // overwrite user so clients do not receiver the other user's password
                commandMsg.UserData = new User() { Name = user.Name , Id = user.Id, Creation = user.Creation, LastOnline = user.LastOnline, Color = user.Color};
                
                var singleMessage = new CommandMessge()
                {
                    Type = CommandType.NewSingleMessage,
                    ConvoMessage = commandMsg.ConvoMessage
                };
                singleMessage.UserData = commandMsg.UserData;
                singleMessage.ConvoMessage.User = commandMsg.UserData;
                
                ServerConversationManager.SendNewMessageToAllConnected(singleMessage);
                break;
            case CommandType.EchoReq:
                var echoRep = new CommandMessge()
                {
                    Type = CommandType.EchoRep
                };
                ServerConversationManager.SendNewMessageToAllConnected(echoRep);
                break;
            case CommandType.GetServerInfoReq:
                var serverInfoRep = new CommandMessge()
                {
                    Type = CommandType.GetServerInfoRep,
                    ServerInfo = ServerConversationManager.ServerInfo
                };
                ServerConversationManager.SendMessage(serverInfoRep,clientId);
                break;
            
                
                
                
        }
    }
}