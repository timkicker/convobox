using System.Collections.Generic;
using System.Linq;
using Convobox.Client.ViewModels;
using Convobox.Client.Views;
using Convobox.Server;
using DynamicData;

namespace Convobox.Client.Models;

public static class ClientMessageValidator
{
    public static void Validate(CommandMessge msg)
    {
        switch (msg.Type)
        {
            case CommandType.LoginSuccess:
                ClientConversationManager.User = msg.UserData;
                ClientConversationManager.IsLoggedIn = true;
                break;
            case CommandType.LoginError:
                ClientConversationManager.IsLoggedIn = false;
                break;
            case CommandType.RegisterSuccess:
                ClientConversationManager.User = msg.UserData;
                ClientConversationManager.IsLoggedIn = true;
                break;
            case CommandType.RegisterError:
                ClientConversationManager.IsLoggedIn = false;
                break;
            case CommandType.MessagesRep:
                for (int i = 0; i < msg.Messages.Count; i++)
                {
                    if (i < ChatViewModel.History.Count())
                    {
                        // skip till new entries
                        break;
                    }
                    ChatViewModel.History.Add(msg.Messages[i]);
                }
                List<ConvoMessage> sorted = ChatViewModel.History.OrderBy(o=>o.Id).ToList();
                ChatViewModel.History.Clear();
                ChatViewModel.History.AddRange(sorted);
                break;

            
        }
    }
}