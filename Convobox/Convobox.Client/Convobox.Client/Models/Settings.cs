using System;
using System.Drawing;
using System.IO;
using System.Net;
using Convobox.Client.Interfaces;
using Convobox.Server;
using Material.Colors;
using Material.Styles.Themes;
using Material.Styles.Themes.Base;
using Newtonsoft.Json;
using SharedDefinitions;
using Color = Avalonia.Media.Color;

namespace Convobox.Client.Models;

public class Settings
{
    private static Settings _current = new Settings();
    private string _username;
    private string _encryptedPassword;
    private Avalonia.Media.Color _colorTheme;
    private string _themeName;
    private bool _notifications;
    private bool _notifyOnlyMention;
    private IClientCryptographyManager _cryptoManager;
    private ServerInfo _serverInfo;
    
    public Settings()
    {
        _serverInfo = new ServerInfo();
        _cryptoManager = new ClientCryptoManager();
        Notifications = true;
        NotifyOnlyMention = false;
        ColorTheme = App.ThemeManager.CurrentPrimaryColor;
        ThemeName = "Dark";
    }
    
    public static Settings Load()
    {
        string jsonString = File.ReadAllText(FilePath);
        
        Current = JsonConvert.DeserializeObject<Settings>(jsonString);
        return Current;
    }

    public static void TrySave()
    {
        try
        {
            string jsonString = JsonConvert.SerializeObject(Current);
            File.WriteAllText(FilePath, jsonString);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    public static string FilePath
    {
        get
        {

            string path;
            
            if (OperatingSystem.IsWindows() || OperatingSystem.IsLinux() || OperatingSystem.IsMacOS())
            {
                path = PlatformInformation.GetApplicationSettingsPath();
            }
            
            else throw new Exception("settings not supported");

            
            
            return path;
        }
    }
    
    public static Settings Current
    {
        get => _current;
        set => _current = value ?? throw new ArgumentNullException(nameof(value));
    }

    public Avalonia.Media.Color ColorTheme
    {
        get => _colorTheme;
        set => _colorTheme = value;
    }

    public bool Notifications
    {
        get => _notifications;
        set => _notifications = value;
    }

    public ServerInfo ServerInfo
    {
        get => _serverInfo;
        set => _serverInfo = value;
    }

    public bool NotifyOnlyMention
    {
        get => _notifyOnlyMention;
        set => _notifyOnlyMention = value;
    }

    public IBaseTheme GetTheme()
    {
        switch (ThemeName)
        {
            case "Dark":
                return Material.Styles.Themes.Theme.Dark;
                break;
            case "Light":
                return Material.Styles.Themes.Theme.Light;
                break;
            default:
                return Material.Styles.Themes.Theme.Dark;
        }
    }
    
    public Color GetPrimaryColor()
    {
        return ColorTheme;
    }

    public string GetPassword()
    {
        return _cryptoManager.Decrypt(_encryptedPassword);
    }

    public void SetPassword(string pass)
    {
        _encryptedPassword = _cryptoManager.Encrypt(pass);
    }

    public string Username
    {
        get => _username;
        set => _username = value ;
    }

    public string EncryptedPassword
    {
        get => _encryptedPassword;
        set => _encryptedPassword = value;
    }

    public IBaseTheme Theme
    {
        set
        {
            if (value == Material.Styles.Themes.Theme.Dark)
            {
                ThemeName = "Dark";
            }
            else if (value == Material.Styles.Themes.Theme.Light)
            {
                ThemeName = "Light";
            }
        }

        get
        {
            if (ThemeName == "Dark")
            {
                return Material.Styles.Themes.Theme.Dark;
            }
            else if (ThemeName == "Light")
            {
                return Material.Styles.Themes.Theme.Light;
            }

            return null;
        }
    }

    public string ThemeName
    {
        get => _themeName;
        set => _themeName = value ;
    }
}