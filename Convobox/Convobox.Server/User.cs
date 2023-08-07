using System.Runtime.InteropServices.JavaScript;
using MemoryPack;

namespace Convobox.Server;

[MemoryPackable]
public partial class User
{
    

    private int _id;
    private string _name;
    private string _password;
    private DateTime _creation;
    private DateTime _lastOnline;
    private bool _admin;

    [MemoryPackConstructor]
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

}