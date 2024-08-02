using Microsoft.AspNetCore.SignalR;
using MySql.Data.MySqlClient;

namespace SignalR
{
    
    public class FriendHub : Hub
    {
        Dictionary<int,string> userList = new Dictionary<int,string>();
        public FriendHub()
        {
        }

        public async Task OnConnect(int user_id)
        {
            userList.Add(user_id, Context.User.Identity.Name);

            await Clients.Caller.SendAsync("Connected", true, Context.User.Identity.Name);
            Console.WriteLine($"Был подключен пользователь с ID={user_id} и ID подключения={Context.User.Identity.Name}");
        }

        public async Task SendFriendRequest(int userId, string userName, int friendToAdd)
        {
            await Clients.User(userList[friendToAdd]).SendAsync("RecieveFriendRequest", userId, userName);
            DbUtils.ConnectionDB();
            DbUtils.command.CommandText = "INSERT users.friendtoadd(requester_id,friend_to_add) VALUES (@userId,@friendToAdd)";
            DbUtils.command.Parameters.AddWithValue("@userId", userId);
            DbUtils.command.Parameters.AddWithValue("@friendToAdd", friendToAdd);
            DbUtils.command.ExecuteNonQuery();
        }

        public async Task CheckFriendRequests(int user_id)
        {
            Console.WriteLine("Попытка получить приглашения в друзья от пользователя с id=" + user_id);
            Dictionary<int,string> friendToAdd = new Dictionary<int, string>();
            DbUtils.ConnectionDB();
            DbUtils.command.CommandText = "SELECT f.requester_id, u.name FROM users.friendtoadd f JOIN users.userdata u ON f.requester_id = u.id WHERE friend_to_add=@id";
            DbUtils.command.Parameters.AddWithValue("@id", user_id);
            MySqlDataReader reader = DbUtils.command.ExecuteReader();
            while (reader.Read())
            {
                int friendId = reader.GetInt32("id");
                string friendName = reader.GetString("name");
                friendToAdd.Add(friendId, friendName);
                Console.WriteLine($"Найдено приглашения в друзья для пользователя{user_id} от {friendToAdd}");
            }

            await Clients.Caller.SendAsync("RecieveFriendRequests", friendToAdd);
        }

        public async Task AcceptFriendRequest(int friendId, string userName,int userId)
        {
            await Clients.User(userList[friendId]).SendAsync("FriendRequestAccepted", userName, userId);
        }
    }
}
