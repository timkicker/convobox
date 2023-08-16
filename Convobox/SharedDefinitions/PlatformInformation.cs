using SharedDefinitions.Interfaces;

namespace SharedDefinitions;

public class PlatformInformation : IPlatformInformation
{
    private static string _workingDirOverride;
    public static string GetWorkingDir()
    {
        if (!string.IsNullOrEmpty(_workingDirOverride))
        {
            return _workingDirOverride;
        }
        else return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
    }

    public static string GetDownloadsFolder()
    {
        var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");

        if (Path.Exists(path))
            return path;

        return null;
    }
    
    public static string GetApplicationTempImageFolder()
    {
        var path = Path.Combine(GetApplicationTempFolder(), "Images");

        if (!Directory.Exists(path))
        {
            try
            {
                Directory.CreateDirectory(path);
            }
            catch (Exception e)
            {
                // a file exists with the same name of this path. hm
            }
        }

        return path;
    }

    public static string GetApplicationTempFolder()
    {
        var path = Path.Combine(GetWorkingDir(), "temp");
        
        if (!Directory.Exists(path))
        {
            try
            {
                Directory.CreateDirectory(path);
            }
            catch (Exception e)
            {
                // a file exists with the same name of this path. hm
            }
        }

        return path;
    }
    
    public static string GetApplicationFileStorage()
    {
        var path = Path.Combine(GetWorkingDir(), "sentfiles");
        
        if (!Directory.Exists(path))
        {
            try
            {
                Directory.CreateDirectory(path);
            }
            catch (Exception e)
            {
                // a file exists with the same name of this path. hm
            }
        }

        return path;
    }
    
    public static string GetApplicationSettingsPath()
    {
        var path = Path.Combine(PlatformInformation.GetWorkingDir(), "convobox-settings.js");
        if (!File.Exists(path))
        {
            File.Create(path);
        }

        return path;
    }

    public static void ManuallySetWorkingDir(string dir)
    {
        if (!Directory.Exists(_workingDirOverride))
        {
            Directory.CreateDirectory(_workingDirOverride);
        }
        _workingDirOverride = dir;
    }
}