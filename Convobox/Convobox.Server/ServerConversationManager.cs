using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Convobox.Server;

public static class ServerConversationManager
{
    const int port = 5000;
    const string domain = "localhost";
    static readonly object _lock = new object();
    static readonly Dictionary<int, TcpClient> _clients = new Dictionary<int, TcpClient>();
    static readonly Dictionary<int, User> _clientAuth = new Dictionary<int, User>();

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
        TcpClient client;
        lock (_lock) client = _clients[clientId];
        NetworkStream stream = client.GetStream();
        stream.Write(msg.Serialize());
    }

    public static void ClientThreadHandler(object o)
    {
        TcpClient client;
        int id = (int)o;
        
        lock (_lock) client = _clients[id];

        while (true)
        {
            try
            {
                NetworkStream stream = client.GetStream();
                byte[] buffer = new byte[1024];
                int byte_count = stream.Read(buffer, 0, buffer.Length);

                if (byte_count == 0)
                {
                    break;
                }

                var receivedMessage = CommandMessge.Deserialize(buffer);
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

    public static object Lock => _lock;
}