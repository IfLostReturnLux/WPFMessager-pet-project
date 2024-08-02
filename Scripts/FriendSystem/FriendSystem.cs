using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using MySql.Data.MySqlClient;

namespace CrystalMessagerWPF
{
    internal class FriendSystem
    {
        MainWindowViewModel _viewModel;
        HubConnection _hubConnection;
        public FriendSystem(MainWindowViewModel viewModel, int userId)
        {


            Console.WriteLine("Загружена система друзей");
            _viewModel = viewModel;
            _hubConnection = new HubConnectionBuilder()
                .WithUrl("http://localhost/friendHub", options =>
                {
                    options.AccessTokenProvider = () => Task.FromResult(userId.ToString());
                })
                .Build();

            try
            {
                _hubConnection.StartAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            _hubConnection.InvokeAsync("OnConnect", userId);
            _hubConnection.InvokeAsync("CheckFriendRequests", userId);

            _hubConnection.On<Dictionary<int, string>>("RecieveFriendRequests", (friendToAdd) =>
            {
                foreach (KeyValuePair<int, string> fr in friendToAdd)
                {
                    FriendToAdd friend = new FriendToAdd();
                    friend.Id = fr.Key;
                    friend.Name = fr.Value;
                    Console.WriteLine($"Получено приглашение от {fr.Key} с именем {fr.Value}");
                    _viewModel.FriendsToAdd.Add(friend);
                }
            });
        }

        public void GetFriends(int user_id)
        {
            List<Friend> friends = new List<Friend>();
            if (DbUtils.ConnectionDB())
            {
                DbUtils.command.CommandText = @"
                      SELECT f.friend_id, u.name
                      FROM userfriend f
                      JOIN userdata u ON f.friend_id = u.id
                      WHERE f.user_id = @userId";
                DbUtils.command.Parameters.AddWithValue("@userId", user_id);
                MySqlDataReader reader;
                reader = DbUtils.command.ExecuteReader();
                while (reader.Read())
                {
                    Friend friend = new Friend();
                    friend.Id = Convert.ToInt32(reader["friend_id"]);
                    friend.Name = reader["name"].ToString();

                    Console.WriteLine($"Был добавлен друг с именем:{friend.Name} и Id:{friend.Id} ");
                    _viewModel.Friends.Add(friend);
                }
            }
            else
            {
                Console.WriteLine("Ошибка подключения в БД");
            }
            DbUtils.CloseDB();
        }

        public void AddFriend(Friend friend)
        {
            _viewModel.Friends.Add(friend);
            _viewModel.FriendsToAdd.Remove(friend);
            Console.WriteLine($"Был добавлен друг {friend.Name}");
        }

        public void SendFriendRequest(int friendId, int userId, string userName)
        {
            _hubConnection.InvokeAsync("SendFriendRequest", friendId, userName, userId);
        }
    }
}
