using System;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Threading;
using Convobox.Server;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Convobox.Client.Models;

public static class ClientConversationManager
{
    private static User _user;
    private static int _port;
    private static bool _isConnected;
    private static string _instance;
    private static bool _isLoggedIn;
    private static TcpClient _client;
    private static SslStream _stream;
    private static IPAddress _serverAdress;
    private static DateTime _lastCommandReceivedTime;
    private static CancellationTokenSource _cancellationTokenSource;
    private static Thread _receiveThread;
    private static Thread _connectionThread;
    
    public static bool Register(string username, string password)
    {
        var registerReq = new CommandMessge(CommandType.RegisterReq);
        registerReq.UserData = new User(username,password);
        
        Send(registerReq);
        
        long ticksMax = (DateTime.Now + new TimeSpan(0, 0, 0, 6)).Ticks;
        
        
        
        while (!IsLoggedIn && (DateTime.Now.Ticks < ticksMax))
        {
            // wait till logged in or timeout
        }
        
        if (IsLoggedIn)
        {
            return true;
        }

        return false;
    }
    
    public static bool Login(string name, string password)
    {
        var loginReq = new CommandMessge(CommandType.LoginReq);
        loginReq.UserData = new User(name,password);
        
        Send(loginReq);
        
        long ticksMax = (DateTime.Now + new TimeSpan(0, 0, 0, 6)).Ticks;
        
        
        
        while (!IsLoggedIn && (DateTime.Now.Ticks < ticksMax))
        {
            // wait till logged in or timeout
        }
        
        if (IsLoggedIn)
        {
            return true;
        }

        return false;
    }

    public static void Send(CommandMessge msg)
    {
        try
        {
            _stream.Write(msg.Serialize());
        }
        catch (Exception e)
        {
            NavigationStore.InternLogger.Log("Conversation","Could not send message");
        }
    }
    
    public static void CloseConnection()
    {
        _cancellationTokenSource?.Cancel();
        _client?.Close();
        _client?.Dispose();
        
    }
    
    public static bool Connect(string instance, string port)
    {
        try
        {
            // check if instance is an ip adress
            var match = Regex.Match(instance, @"\b(\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3})\b");
            if (match.Success)
            {
                _serverAdress = IPAddress.Parse(instance);
            }
            else // must be a domain?
            {
                _serverAdress = Dns.GetHostAddresses(instance)[0];
            }

            _port = Convert.ToInt32(port);
            _client = new TcpClient();
            _client.Connect(_serverAdress, _port);
            
            _stream = new SslStream(
                _client.GetStream(),
                false,
                new RemoteCertificateValidationCallback (ClientCryptoManager.ValidateServerCertificate),
                null
            );
            
            _stream.AuthenticateAsClient("convobox");

            // start receive thread
            _cancellationTokenSource = new CancellationTokenSource();
            _receiveThread = new Thread(ReceiveThread);
            _receiveThread.Start(_cancellationTokenSource.Token);
            _connectionThread = new Thread(ConnectionCheckThread);
            _connectionThread.Start(_cancellationTokenSource.Token);
            
            _lastCommandReceivedTime = DateTime.Now;
            return _isConnected = true;
        }
        catch (Exception e)
        {
            NavigationStore.InternLogger.Log("Connection","Connection to server failed");
            return false;
        }
        
    }
    
    
    
    #region threads

    private static void ConnectionCheckThread(System.Object obj)
    {
        CancellationToken token = (CancellationToken)obj;
        while (!token.IsCancellationRequested)
        {
            var timeSinceLatestContact = DateTime.Now - _lastCommandReceivedTime;

            if (timeSinceLatestContact.TotalSeconds > 20)
            {
                CloseConnection();
                NavigationStore.BackToLogin("Connection timeout");
            }
            
            if (timeSinceLatestContact.TotalSeconds > 10)
            {
                Send(new CommandMessge(){Type = CommandType.EchoReq});
            }
            
        }
    }
    
    private static void ReceiveThread(System.Object obj)
    {
        CancellationToken token = (CancellationToken)obj;
        while (!token.IsCancellationRequested)
        {

            if (_client.Available > 0)
            {
                byte[] buffer = new byte[_client.ReceiveBufferSize];
                int byte_count = _stream.Read(buffer, 0, buffer.Length);

                if (byte_count == 0)
                {
                    break;
                }
                
                var bytesWanted = buffer.Skip(0).Take(byte_count).ToArray();         
            
                LastCommandReceivedTime = DateTime.Now;
                try
                {
                    var receivedMessage = CommandMessge.Deserialize(bytesWanted);
                    ClientMessageValidator.Validate((receivedMessage));
                }
                catch (Exception e)
                {

                }
            }
        }
        
    }
    

    #endregion

    public static User User
    {
        get => _user;
        set => _user = value;
    }

    public static bool IsConnected
    {
        get => _isConnected;
        set => _isConnected = value;
    }

    public static bool IsLoggedIn
    {
        get => _isLoggedIn;
        set => _isLoggedIn = value;
    }

    public static DateTime LastCommandReceivedTime
    {
        get => _lastCommandReceivedTime;
        set => _lastCommandReceivedTime = value;
    }


    
}