using System.Drawing;
using System.Reflection.Metadata;
using System.Text.RegularExpressions;
using Microsoft.Data.Sqlite;
using SharedDefinitions;

namespace Convobox.Server;
public class DatabaseManager
{
    private static SqliteConnection _connection;
    static string databaseLocation = "";
    
    public static void SetDatabaseLocation()
    {
        if (OperatingSystem.IsWindows())
        {
            databaseLocation = $@"Data Source={AppDomain.CurrentDomain.BaseDirectory}convobase.sqlite";
        }
        else // GNU+Linux & MacOS
        {
            databaseLocation = $@"Data Source=/home/tim/Repo/convobox/Convobox/Convobox.Server/convobase.sqlite";
        }
    }

    public static int GetMessageCount()
    {
        try
        {
            using (var connection = new SqliteConnection(databaseLocation))
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = $"select count(*) from convo_message;";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        return reader.GetInt32(0);
                    }
                }

                return 0;

            }
        }
        catch (Exception)
        {
            return 0;
        }
    }

    public static List<ConvoMessage> GetLastMessages(int amount)
    {

        var list = new List<ConvoMessage>();
        
        try
        {
            using (var connection = new SqliteConnection(databaseLocation))
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = $"select * FROM (select convo_message.id,convo_message.user_id,convo_message.data,convo_message.creation_date, user.name,user.color from convo_message inner join user on user.id = convo_message.user_id order by convo_message.id desc limit @amount) order by id;";
                
                command.Parameters.Add("@amount", SqliteType.Text);
                command.Parameters["@amount"].Value = amount;

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int id = reader.GetInt32(0);
                        int user_id = reader.GetInt32(1);
                        string data = reader.GetString(2);
                        DateTime creation = ParseToCSDateTime(reader.GetString(3));
                        string name = reader.GetString(4);
                        Color userColor = GetColor(reader.GetString(5));

                        var user = new User();
                        user.Id = user_id;
                        user.Color = userColor;
                        user.Name = name;
                        
                        var msg = new ConvoMessage()
                        {
                            Id = id,
                            User = user,
                            Creation = creation,
                            Data = data
                        };
                        
                        list.Add(msg);

                    }
                }
                
            }

            return list;
        }
        catch (Exception e)
        {
            return null;
        }
    }

    public static ConvoMessage InsertMessage(ConvoMessage msg)
    {
        try
        {
            using (var connection = new SqliteConnection(databaseLocation))
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = $"insert into convo_message (user_id,data,creation_date) values(@userId,@data,@creationDate);";
                
                command.Parameters.Add("@userId", SqliteType.Integer);
                command.Parameters["@userId"].Value = msg.User.Id;
                
                command.Parameters.Add("@data", SqliteType.Text);
                command.Parameters["@data"].Value = msg.Data;
                
                command.Parameters.Add("@creationDate", SqliteType.Text);
                command.Parameters["@creationDate"].Value = ParseToSQLiteDateTime(msg.Creation);
                command.ExecuteReader();
                
                var getIdCommand = connection.CreateCommand();
                getIdCommand.CommandText = $"select * FROM (select convo_message.id,user.color from convo_message inner join user on user.id = convo_message.user_id order by convo_message.id desc limit 1) order by id;";
                

                using (var reader = getIdCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        msg.Id = reader.GetInt32(0);
                        msg.User.Color = GetColor(reader.GetString(1));
                        return msg;
                    }
                }

                return null;
            }
        }
        catch (Exception e)
        {
            return null;
        }
    }

    public static bool CheckLogin(User userToCheck)
    {
        try
        {
            using (var connection = new SqliteConnection(databaseLocation))
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = $"select user.name from user where name=@name and user.password=@password;";
                
                command.Parameters.Add("@name", SqliteType.Text);
                command.Parameters["@name"].Value = userToCheck.Name;
                
                command.Parameters.Add("@password", SqliteType.Text);
                command.Parameters["@password"].Value = userToCheck.Password;

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader.GetString(0) == userToCheck.Name)
                            return true;
                        else return false;
                    }
                }

            }
        }
        catch (Exception e)
        {
            return false;
        }

        return false;
    }

    public static int CreateUser(User userToCreate)
    {
        if (GetUser(userToCreate.Name) is not null)
        {
            // user already exists
            return 0;
        }
        
        try
        {
            using (var connection = new SqliteConnection(databaseLocation))
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = $"insert into user (name,password,creation_date,last_online_date,admin,color) values(@name,@password,@creation,@creation,0,@color);";
                
                command.Parameters.Add("@name", SqliteType.Text);
                command.Parameters["@name"].Value = userToCreate.Name;
                
                command.Parameters.Add("@password", SqliteType.Text);
                command.Parameters["@password"].Value = userToCreate.Password;
                
                command.Parameters.Add("@creation", SqliteType.Text);
                command.Parameters["@creation"].Value = ParseToSQLiteDateTime(DateTime.Now);
                
                command.Parameters.Add("@color", SqliteType.Text);
                command.Parameters["@color"].Value = GetColorString(Definition.GetRandomUserColor());

                command.ExecuteReader();
                
                connection.Open();

                var getIdCommand = connection.CreateCommand();
                getIdCommand.CommandText = $"select user.id from user where name=@name";
                
                getIdCommand.Parameters.Add("@name", SqliteType.Text);
                getIdCommand.Parameters["@name"].Value = userToCreate.Name;

                using (var reader = getIdCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        return reader.GetInt32(0);
                    }
                }
                
                
                return 0;
            }
        }
        catch (Exception e)
        {
            return 0;
        }
        
    }
    
    public static User GetUser(string username)
    {
        try
        {
            using (var connection = new SqliteConnection(databaseLocation))
            {

                
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = $"select id,name,password,creation_date,last_online_date,admin,color from user where name=@username;";
                
                command.Parameters.Add("@username", SqliteType.Text);
                command.Parameters["@username"].Value = username;

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var user = new User();
                        user.Id = reader.GetInt32(0);
                        user.Name = reader.GetString(1);
                        user.Password = reader.GetString(2);
                    
                        user.Creation = ParseToCSDateTime(reader.GetString(3));
                        user.LastOnline = ParseToCSDateTime(reader.GetString(4));
                        user.Admin = Convert.ToBoolean(reader.GetInt32(5));
                        user.Color = GetColor(reader.GetString(6));
                        return user;
                    }
                }
            }
        }
        catch (Exception e)
        {
            return null;
        }

        return null;

    }

    public static Color GetColor(string color)
    {
        string[] rgb = color.Split(':');
        return Color.FromArgb(255, Convert.ToInt32(rgb[0]), Convert.ToInt32(rgb[1]), Convert.ToInt32(rgb[2]));
    }

    public static string GetColorString(Color color)
    {
        return color.R + ":" + color.G + ":"+  color.B;
    }
    
    public static DateTime ParseToCSDateTime(string sqliteDateTime)
    {
        string pattern = @"(\d{4})-(\d{2})-(\d{2}) (\d{2}):(\d{2}):(\d{2})";
        if (Regex.IsMatch(sqliteDateTime, pattern))
        {
            Match match = Regex.Match(sqliteDateTime, pattern);
            int year = Convert.ToInt32(match.Groups[1].Value);
            int month = Convert.ToInt32(match.Groups[2].Value);
            int day = Convert.ToInt32(match.Groups[3].Value);
            int hour = Convert.ToInt32(match.Groups[4].Value);
            int minute = Convert.ToInt32(match.Groups[5].Value);
            int second = Convert.ToInt32(match.Groups[6].Value);
            return new DateTime(year, month, day, hour, minute, second, 0);
        }
        else
        {
            throw new Exception("Unable to parse string to DateTime");
        }
    }

    public static string ParseToSQLiteDateTime(DateTime csDateTime)
    {
        return csDateTime.ToString("yyyy-MM-dd HH:mm:ss");
    }
}