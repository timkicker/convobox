using MemoryPack;

namespace Convobox.Server;

[MemoryPackable]
public partial class ConvoMessage
{
    private int _id;
    private string _data;
    private User _user;
    private DateTime _creation;

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