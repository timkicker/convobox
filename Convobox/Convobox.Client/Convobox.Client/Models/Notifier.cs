using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Convobox.Server;
using DesktopNotifications;
using DesktopNotifications.FreeDesktop;
using DesktopNotifications.Windows;

namespace Convobox.Client.Models;

public static class Notifier
{
    private static INotificationManager _notificationManager;

    public static void CreateManager()
    {
        if (OperatingSystem.IsLinux())
        {
            _notificationManager = new FreeDesktopNotificationManager();
        }
        else if (OperatingSystem.IsWindows())
        {
            _notificationManager = new WindowsNotificationManager();
        }
        else
        {
            _notificationManager = null;
        }
        _notificationManager.Initialize();
    }
    
    
    public static async Task ShowReceivedMessage(ConvoMessage msg)
    {
        if (_notificationManager is null)
            return;
        
        if (msg.User.Name == ClientConversationManager.User.Name || !Settings.Current.Notifications)
        {
            return;
        }

        string notificationText;

        if (msg.FileAttached)
            notificationText = "File: " + msg.FileName;
        else notificationText = msg.Data;

        var notification = new Notification
        {
            Title = msg.User.Name,
            Body = notificationText
        };
        
        DateTimeOffset dateOffset1, dateOffset2;
        
        
        dateOffset1 = DateTimeOffset.Now;
        dateOffset2 = DateTimeOffset.Now + new TimeSpan(0,0,0,10);
        
        _notificationManager.ShowNotification(notification);
        
    }
    
    public static async Task ShowCustom(string title, string message)
    {

        var notification = new Notification
        {
            Title = title,
            Body = message,
        };
        
        DateTimeOffset dateOffset1, dateOffset2;
        
        
        dateOffset1 = DateTimeOffset.Now;
        dateOffset2 = DateTimeOffset.Now + new TimeSpan(0,0,0,10);
        
        _notificationManager.ShowNotification(notification);
        
    }
    
}