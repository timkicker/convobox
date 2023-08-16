
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using Microsoft.Extensions.FileProviders;

namespace Convobox.Server;


public partial class ConvoMessage
{
    private int _id;
    private string _data;
    private User _user = new User();
    private DateTime _creation;
    private string _base64File;
    private string _uniqueFileName;
    private string _fileName;
    private string _localFilePath;
    
    // for client display
    public ObservableCollection<ConvoMessage> ClientMessages { get; set; } = new ObservableCollection<ConvoMessage>();

    public bool SetFile(byte[] fileBytes)
    {
        if (fileBytes.Length <= 0)
            return false;
        
        try
        {
            Base64File = Convert.ToBase64String(fileBytes);
            return true;
        }
        catch (Exception e)
        {
            return false;
        }
    }

    public byte[] GetFile()
    {
        if (string.IsNullOrEmpty(Base64File))
        {
            return null;
        }
        else return Convert.FromBase64String(Base64File);
    }
    
    public string UserSymbol
    {
        get
        {
            if (UsernameDisplay?.Length > 0)
            {
                return User.Symbol;
            }

            return "";
        }
    }
    
    public string UsernameDisplay
    {
        get
        {
            try
            {
                int index = ClientMessages.IndexOf(this);
                if (index > -1)
                {
                    string lastUsername = ClientMessages[index - 1].User.Name;
                    string username = this.User.Name;

                    if (this.Creation.Day != ClientMessages[index - 1].Creation.Day)
                        return username;
                    
                    if ((username == lastUsername) && ClientMessages[index - 1].DateDisplay.Length > this.DateDisplay.Length)
                    {
                        return username;
                    }
                    else if (username == lastUsername)
                    {
                        return "";
                    }
                    

                    return username;
                }
                return this.User.Name;
            }
            catch (Exception e)
            {
                return this.User.Name;
            }
            return this.User.Name;
        }
    }

    public string Test
    {
        get
        {
            return Data + " | " + Space;
        }
    }

    public bool ShowGeneralFileDisplay
    {
        get
        {
            return !IsImage && FileAttached;
        }
    }

    public bool IsImage
    {
        get
        {
            if (Path.GetExtension(FileName) == ".webp")
                return true;
            return false;
        }
    }
    
    public string LocalFilePath
    {
        get => _localFilePath;
        set => _localFilePath = value;
    }

    public int Space
    {
        get
        {
            if (UsernameDisplay?.Length > 1)
            {
                return 20;
            }

            return 0;
        }
    }

    public string FileName
    {
        get => _fileName;
        set => _fileName = value;
    }

    
    
    public string DateDisplay
    {
        get
        {
            // today
            if (Creation.Day == DateTime.Now.Day &&
                Creation.Month == DateTime.Now.Month &&
                Creation.Year == DateTime.Now.Year)                      
            {
                return Creation.ToString("HH:mm");
            }
            // yesterday
            /*else if ((Creation.Day == DateTime.Now.Day - 1) && 
                     Creation.Month == DateTime.Now.Month &&
                     Creation.Year == DateTime.Now.Year)
            {
                return "Yesterday, " + Creation.ToString("hh:mm");
            }*/
            // same year
            else if (Creation.Year == DateTime.Now.Year)
            {
                return Creation.ToString("dd-MM HH:mm");
            }
            else
            {
                return Creation.ToString("yyyy-dd-MM HH:mm");
            }
            
        }
    }

    public string UniqueFileName
    {
        get => _uniqueFileName;
        set => _uniqueFileName = value;
    }

    public int Id
    {
        get => _id;
        set => _id = value;
    }

    public string Data
    {
        get => _data;
        set => _data = value;
    }

    public User User
    {
        get => _user;
        set => _user = value ;
    }

    public string Base64File
    {
        get => _base64File;
        set => _base64File = value ;
    }

    public DateTime Creation
    {
        get => _creation;
        set => _creation = value;
    }

    public bool HasText
    {
        get
        {
            if (string.IsNullOrEmpty(Data) && Data != "11010100100110001001101010110101")
            {
                return false;
            }
            else return true;
        }
    }
  
    
    public bool FileAttached
    {
        get
        {
            if (!string.IsNullOrEmpty(FileName) && !string.IsNullOrEmpty(UniqueFileName))
            {
                return true;
            }

            return false;
        }
    }
    
}