using System;
using System.Drawing;
using System.IO;
using Material.Styles.Themes;
using Material.Styles.Themes.Base;
using Newtonsoft.Json;

namespace Convobox.Client.Models;

public class Settings
{
    private static Settings _current;

    private Avalonia.Media.Color _colorTheme;
    private IBaseTheme _theme;

    public static Settings Load()
    {
        string jsonString = File.ReadAllText(FilePath);
        
        Current = JsonConvert.DeserializeObject<Settings>(jsonString);
        return Current;
    }

    public static void Save()
    {
        string jsonString = JsonConvert.SerializeObject(Current);
        File.WriteAllText(FilePath, jsonString);
    }

    public static string FilePath
    {
        get
        {

            string path;
            
            if (OperatingSystem.IsWindows())
            {
                path =  Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\convobox-settings.js";
            }
            else if (OperatingSystem.IsLinux())
            {
                path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/convobox-settings.js";
            }
            else throw new Exception("settings not supported");

            if (!File.Exists(path))
            {
                File.Create(path);
            }
            
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

    public IBaseTheme Theme
    {
        get => _theme;
        set => _theme = value ?? throw new ArgumentNullException(nameof(value));
    }
}