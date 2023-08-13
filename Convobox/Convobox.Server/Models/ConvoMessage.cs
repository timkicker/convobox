using MemoryPack;
using System;
using System.Collections.ObjectModel;

namespace Convobox.Server;


public partial class ConvoMessage
{
    private int _id;
    private string _data;
    private User _user = new User();
    private DateTime _creation;
    
    // for client display
    public ObservableCollection<ConvoMessage> ClientMessages { get; set; } = new ObservableCollection<ConvoMessage>();


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

    public DateTime Creation
    {
        get => _creation;
        set => _creation = value;
    }

    
}