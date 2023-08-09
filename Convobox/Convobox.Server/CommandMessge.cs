using System.Text;
using System.Text.Json;
using MemoryPack;
namespace Convobox.Server;

[MemoryPackable]
public partial class CommandMessge
{
    private CommandType _type;
    private User _userData; // for login, etc.
    private DateTime _creationTime;
    private ConvoMessage _convoMessage;
    private int _amount;
    private List<ConvoMessage> _messages;

    
    [MemoryPackConstructor]
    public CommandMessge()
    {
        _messages = new List<ConvoMessage>();
        _type = CommandType.Error;
        _userData = new User();
        _convoMessage = new ConvoMessage();
        _creationTime = DateTime.Now;
    }
    
    public CommandMessge(CommandType type)
    {
        _type = type;
        _creationTime = DateTime.Now;   
    }
    
    public byte[] Serialize()
    {
        //return MemoryPackSerializer.Serialize(this);
        string jsonString = JsonSerializer.Serialize(this);
        return Encoding.UTF8.GetBytes(jsonString);
    }

    public static CommandMessge Deserialize(byte[] messageBytes)
    {
        //return MemoryPackSerializer.Deserialize<CommandMessge>(messageBytes);
        string jsonString = Encoding.UTF8.GetString(messageBytes);
        return JsonSerializer.Deserialize<CommandMessge>(jsonString);
    }

    public List<ConvoMessage> Messages
    {
        get => _messages;
        set => _messages = value ;
    }

    public CommandType Type
{
    get { return _type; }
    set { _type = value; }
}

    public User UserData
    {
        get => _userData;
        set => _userData = value;
    }

    public ConvoMessage ConvoMessage
    {
        get => _convoMessage;
        set => _convoMessage = value;
    }
    public int Amount
    {
        get => _amount;
        set => _amount = value;
    }

    public DateTime CreationTime => _creationTime;
}

public enum CommandType
{
    Error,
    LoginReq,
    RegisterReq,
    GetMessagesReq,
    EchoReq,
    MessagesReq,
    SendMessage,
    
    
    LoginError,
    LoginSuccess,
    RegisterError,
    RegisterSuccess,
    GetMessagesRep,
    EchoRep,
    NewSingleMessage,
    MessagesRep,
}