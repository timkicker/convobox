using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Convobox.Server;

public static class ServerConversationManager
{
    const int port = 5000;
    const string domain = "localhost";
    static readonly object _lock = new object();
    static readonly Dictionary<int, TcpClient> _clients = new Dictionary<int, TcpClient>();
    static readonly Dictionary<TcpClient, SslStream> _sslStreams = new Dictionary<TcpClient, SslStream>();
    static readonly Dictionary<int, User> _clientAuth = new Dictionary<int, User>();
    private static ServerInfo _serverInfo;

    public static bool Start()
    {
        TcpListener ServerSocket = new TcpListener(IPAddress.Any, port);
        ServerSocket.Start();
                
        int count = 1;
                
        while (true)
        {
            var client = ServerSocket.AcceptTcpClient();
            lock (_lock) _clients.Add(count, client);
            Console.WriteLine("[INFO][Connection] New client connected");
            Thread t = new Thread(ClientThreadHandler);
            t.Start(count);
            count++;
        }
    }

    public static void SendMessage(CommandMessge msg, int clientId)
    {
        var stream = _sslStreams[_clients[clientId]];
        stream.Write(msg.Serialize());
    }
    public static void SendMessage(CommandMessge msg, TcpClient client)
    {
        var stream = _sslStreams[client];
        stream.Write(msg.Serialize());
    }

    public static void SendNewMessageToAllConnected(CommandMessge msg)
    {
        foreach (var client in _clients)
        {
            if (client.Value.Connected)
            {
                SendMessage(msg,client.Value);
            }
        }
    }

    public static void ClientThreadHandler(object o)
    {
        TcpClient client;
        int id = (int)o;
        
        lock (_lock) client = _clients[id];
        var serverCertificate = CryptographyManager.GenerateSelfSignedCertificate("convobox", 365);
        SslStream stream = new SslStream(client.GetStream(), false);
        _sslStreams[client] = stream;
        stream.AuthenticateAsServer(serverCertificate, clientCertificateRequired: false, checkCertificateRevocation: true);

        while (true)
        {
            try
            {
                byte[] buffer = new byte[client.ReceiveBufferSize];
                int byte_count = stream.Read(buffer, 0, buffer.Length);

                if (byte_count == 0)
                {
                    break;
                }
                
                var bytesWanted = buffer.Skip(0).Take(byte_count).ToArray();

                var receivedMessage = CommandMessge.Deserialize(bytesWanted);
                Console.WriteLine("[INFO][Received] Client " + id + "sent a " + receivedMessage.Type.ToString());
                ServerMessageController.Validate(receivedMessage,id);
            }
            catch (Exception e)
            {

            }
        }

        lock (_lock) _clients.Remove(id);
        client.Client.Shutdown(SocketShutdown.Both);
        client.Close();
    }

    public static Dictionary<int, TcpClient> Clients => _clients;

    public static Dictionary<int, User> ClientAuth => _clientAuth;

    public static ServerInfo ServerInfo
    {
        get => _serverInfo;
        set => _serverInfo = value ?? throw new ArgumentNullException(nameof(value));
    }

    public static object Lock => _lock;
}