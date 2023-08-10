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
                msg.Messages.Reverse();
                for (int i = 0; i < msg.Messages.Count; i++)
                {
                    if (i < ChatViewModel.Current.History.Count())
                    {
                        // skip till new entries
                        continue;
                    }

                    msg.Messages[i].ClientMessages = ChatViewModel.Current.History;
                    ChatViewModel.Current.MessagesSum = msg.Amount;
                    ChatViewModel.Current.History.Add(msg.Messages[i]);
                }
                List<ConvoMessage> sorted = ChatViewModel.Current.History.OrderBy(o=>o.Id).ToList();
                ChatViewModel.Current.History.Clear();
                ChatViewModel.Current.History.AddRange(sorted);
                msg.ConvoMessage.ClientMessages = ChatViewModel.Current.History;
                ChatViewModel.Current.UpdateForNewMessages();
                break;
            case CommandType.NewSingleMessage:
                msg.ConvoMessage.ClientMessages = ChatViewModel.Current.History;
                ChatViewModel.Current.History.Add(msg.ConvoMessage);
                ChatViewModel.Current.CheckScroll();
                ChatViewModel.Current.UpdateForNewMessages();
                Notifier.ShowMessage(msg.ConvoMessage);
                break;
            
        }
    }
}