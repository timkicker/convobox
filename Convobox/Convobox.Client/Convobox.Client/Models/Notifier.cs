using System;
using Avalonia;
using Avalonia.Controls.Notifications;
using Convobox.Server;
using DesktopNotifications.Avalonia;

namespace Convobox.Client.Models;

public static class Notifier
{
    private static INotificationManager _notificationManager;


    
    
    public static void ShowMessage(ConvoMessage msg)
    {
        if (msg.User.Name == ClientConversationManager.User.Name)
        {
            return;
        }

        INotification notification = new Notification(msg.User.Name,msg.DateDisplay);
        
        // not working atm :c
        
       // _notificationManager.Show(notification);
    }

    public static INotificationManager NotificationManager
    {
        get => _notificationManager;
        set => _notificationManager = value ?? throw new ArgumentNullException(nameof(value));
    }
}