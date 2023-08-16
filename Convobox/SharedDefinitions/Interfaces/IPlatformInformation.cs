namespace SharedDefinitions.Interfaces;

public interface IPlatformInformation
{
    public static abstract string GetWorkingDir();
    public static abstract string GetApplicationFileStorage();
    public static abstract string GetApplicationTempFolder();
    public static abstract string GetApplicationTempImageFolder();
    // must return empty string if folder does not exist
    public static abstract string GetDownloadsFolder();
    public static abstract void ManuallySetWorkingDir(string dir);
}