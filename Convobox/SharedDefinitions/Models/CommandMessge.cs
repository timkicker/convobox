using System.Text;

using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Convobox.Server;


public partial class CommandMessge
{
    private CommandType _type;
    private User _userData; // for login, etc.
    private DateTime _creationTime;
    private ConvoMessage _convoMessage;
    private int _amount;
    private List<ConvoMessage> _messages;
    private ServerInfo _serverInfo;
    
    public CommandMessge()
    {
        _serverInfo = new ServerInfo();
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
        string jsonString = JsonConvert.SerializeObject(this);
        return Encoding.UTF8.GetBytes(jsonString);
    }

    public static CommandMessge Deserialize(byte[] messageBytes)
    {
        //return MemoryPackSerializer.Deserialize<CommandMessge>(messageBytes);
        string jsonString = Encoding.UTF8.GetString(messageBytes);
        return JsonConvert.DeserializeObject<CommandMessge>(jsonString);
    }

    public List<ConvoMessage> Messages
    {
        get => _messages;
        set => _messages = value ;
    }

    public ServerInfo ServerInfo
    {
        get => _serverInfo;
        set => _serverInfo = value;
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
    GetServerInfoReq,
    
    LoginError,
    LoginSuccess,
    RegisterError,
    RegisterSuccess,
    GetServerInfoRep,
    GetMessagesRep,
    EchoRep,
    NewSingleMessage,
    MessagesRep,
}