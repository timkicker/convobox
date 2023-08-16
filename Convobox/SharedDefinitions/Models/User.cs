using System.Drawing;
using System.Runtime.InteropServices.JavaScript;

namespace Convobox.Server;


public partial class User
{

    private System.Drawing.Color color;
    private int _id;
    private string _name;
    private string _password;
    private DateTime _creation;
    private DateTime _lastOnline;
    private bool _admin;
    
    public User()
    {
        
    }
    public User(string name, string password)
    {
        _name = name;
        _password = password;
    }

    public bool Admin
    {
        get => _admin;
        set => _admin = value;
    }

    public int Id
    {
        get => _id;
        set => _id = value;
    }

    public string Name
    {
        get => _name;
        set => _name = value;
    }
    

    public string Password
    {
        get => _password;
        set => _password = value;
    }

    public DateTime Creation
    {
        get => _creation;
        set => _creation = value;
    }

    public DateTime LastOnline
    {
        get => _lastOnline;
        set => _lastOnline = value;
    }

    public System.Drawing.Color Color
    {
        get => color;
        set => color = value;
    }

    public string Symbol
    {
        get
        {
            if (Admin)
                return "★";
            return "➤";
        }
    }
}