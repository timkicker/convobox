using System.Net;
using System.Text;
using SharedDefinitions;

namespace Convobox.Server;

public class FileHoster
{
    private static CancellationTokenSource _cancellationTokenSource;
    private static string url = "http://localhost:8081/";
    
    public static void Start()
    {
        url = $"http://{ServerConversationManager.ServerInfo.Domain}:{ServerConversationManager.ServerInfo.PortFiles}/";
        
        Thread thread = new Thread(HosterHandler);
        _cancellationTokenSource = new CancellationTokenSource();
        thread.Start(_cancellationTokenSource.Token);
    }

    static void Stop()
    {
        
    }
    
    static bool CheckCredentials(string authHeader)
    {
        if (!string.IsNullOrEmpty(authHeader))
        {
            string encodedCredentials = authHeader.Substring(6);
            byte[] credentialsBytes = Convert.FromBase64String(encodedCredentials);
            string credentials = Encoding.UTF8.GetString(credentialsBytes);
            string[] credentialsArray = credentials.Split(':');
            string username = credentialsArray[0];
            string password = credentialsArray[1];

            var user = new User(username, password);

            return DatabaseManager.CheckLogin(user);
        }

        return false;
    }
    
    static void ServeFile(string fileName, HttpListenerResponse response, string contentType)
    {
        var filePath = Path.Combine(PlatformInformation.GetApplicationFileStorage(), fileName);
        
        if (File.Exists(filePath))
        {
            response.ContentType = "application/octet-stream";
            response.AddHeader("Content-Disposition", $"attachment; filename=\"{fileName}\"");
                        
            using (FileStream fileStream = new FileStream(filePath, FileMode.Open))
            {
                // Kopiere den Inhalt der Datei in die Response
                byte[] buffer = new byte[File.ReadAllBytes(filePath).Length];
                int bytesRead;
                while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    response.OutputStream.Write(buffer, 0, bytesRead);
                }
            }
                        
            response.OutputStream.Close();
        }
        else
        {
            // Handle file not found
            response.StatusCode = 404;
        }
    }
    
    static void HosterHandler(System.Object obj)
    {
        CancellationToken token = (CancellationToken)obj;
        using (HttpListener listener = new HttpListener())
        {
            listener.Prefixes.Add(url);
            listener.Start();
            Console.WriteLine($"Listening for requests on {url}");

            while (!token.IsCancellationRequested)
            {
                HttpListenerContext context = listener.GetContext();
                ThreadPool.QueueUserWorkItem((_) =>
                {
                    HttpListenerRequest request = context.Request;
                    HttpListenerResponse response = context.Response;

                    string authHeader = request.Headers["Authorization"];
                    bool isAuthorized = CheckCredentials(authHeader);

                    if (isAuthorized)
                    {
                        string requestPath = request.RawUrl.TrimStart('/'); // requested filename
                        
                        ServeFile(requestPath, response, "text/html");
                        
                        
                    }
                    else
                    {
                        // Unauthorized
                        response.StatusCode = 401;
                        response.Headers.Add("WWW-Authenticate", "Basic realm=\"Access to files\"");
                        byte[] unauthorizedBytes = Encoding.UTF8.GetBytes("Unauthorized");
                        response.OutputStream.Write(unauthorizedBytes, 0, unauthorizedBytes.Length);
                    }

                    
                });
            }
        }
    }
}