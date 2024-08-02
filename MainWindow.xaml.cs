using System;
using System.Windows;
using System.Windows.Controls;
using MySql.Data.MySqlClient;
using Microsoft.AspNetCore.SignalR.Client;
using System.IO;
using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;
using System.Collections.Generic;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Win32;
using System.Windows.Controls.Primitives;
using System.Runtime.Remoting.Messaging;

namespace CrystalMessagerWPF
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private PersonData _userData;
        private bool _connectionToServer = false;
        private Dictionary<string, List<Message>> _messageList;
        private Dictionary<int, string> _roomList;
        private string _currentRoom;
        private HubConnection _hubConnection;
        private MainWindowViewModel _viewModel;
        private string _profileImagePathAbsolute = Environment.CurrentDirectory + @"\Resources\profile\profile_image.jpg";
        private string _profileImagePathRelative = @"\profile\profile_image.jpg";

        private FriendSystem _friendSystem;


        public MainWindow(PersonData personData)
        {
            InitializeComponent();

            _userData = personData;
            _messageList = new Dictionary<string, List<Message>>();
            _roomList = new Dictionary<int, string>();
            _viewModel = new MainWindowViewModel(_userData.Id);
            DataContext = _viewModel;
            _friendSystem = new FriendSystem(_viewModel, _userData.Id);
            _friendSystem.GetFriends(_userData.Id);

            idLabel.Content = "UID: " + _userData.Id;
            userName.Content = _userData.Name;


            UpdateProfileImage();

            Console.WriteLine("Мой айди: " + _userData.Id + ". Мое имя:" + _userData.Name);


            _hubConnection = new HubConnectionBuilder()
                .WithUrl("http://localhost/chatHub")
                .Build();

            try
            {
                _hubConnection.StartAsync();
                _connectionToServer = true;
            }
            catch (Exception ex)
            {
                _connectionToServer = false;
            }

            //Получение и отображение полученных сообщений
            _hubConnection.On<string,string, string>("RecieveMessage", (roomName,user, message) =>
            {   

                _messageList[roomName].Add(new Message(user,message));
                Console.WriteLine($"{user}: {message}");
                Dispatcher.Invoke(() =>
                {
                    CustomMessage msg = new CustomMessage(user, message);
                    TextBlock[] textBlock = msg.CreateMessage();
                    foreach (var item in textBlock)
                    {
                        MessagePanel.Children.Insert(0, item);
                        Console.WriteLine(item.Text);
                    }
                });
            });

            //Получение истории сообщений всех друзей
            _hubConnection.On<string,List<Message>>("RecieveMessageHistory", (roomName,msgHistory) =>
            {
                    _messageList.Add(roomName, msgHistory);
            });
            Console.WriteLine(_viewModel.Friends);
            //Присоединение к комнатам со всеми друзьями
            foreach(var friend in _viewModel.Friends)
            {
                if(_userData.Id > friend.Id)
                {
                    _hubConnection.InvokeAsync("JoinRoom",$"{friend.Id}_{_userData.Id}");
                    _roomList.Add(friend.Id, $"{friend.Id}_{_userData.Id}");
                }
                else
                {
                    _hubConnection.InvokeAsync("JoinRoom", $"{_userData.Id}_{friend.Id}");
                    _roomList.Add(friend.Id, $"{_userData.Id}_{friend.Id}");
                }
            }
        }


        private void ItemClicked(object sender, MouseButtonEventArgs e)
        {
            Console.WriteLine("Была нажата кнопка!");
            MessagePanel.Children.Clear();
            // Получаем StackPanel, который является источником события
            StackPanel stackPanel = (StackPanel)sender;

            // Получаем ListBoxItem, содержащий StackPanel
            ListBoxItem clickedItem = FindAncestor<ListBoxItem>(stackPanel);

            if (clickedItem != null)
            {
                // Получаем данные привязки из DataContext этого элемента
                Friend clickedFriend = (Friend)clickedItem.DataContext;

                // Выполняем действия, соответствующие нажатию на элемент
                friendNameInDialogue.Content = clickedFriend.Name;
                _currentRoom = _roomList[clickedFriend.Id];

                //Отображение в списке сообщений всех сообщений комнаты 
                Dispatcher.Invoke(() =>
                {
                    if (_connectionToServer == true)
                    {
                        foreach (var message in _messageList[_currentRoom])
                        {
                            CustomMessage msg = new CustomMessage(message.UserName, message.UserMessage);

                            TextBlock[] textBlock = msg.CreateMessage();
                            foreach (var item in textBlock)
                            {
                                MessagePanel.Children.Insert(0, item);
                            }
                        }
                    }
                    else
                    {
                        CustomMessage msg = new CustomMessage("Система", "К сожалению отсутствует соединение с сервером! Приносим свои извинения :3");
                        TextBlock[] textBlock = msg.CreateMessage();
                        foreach (var item in textBlock)
                        {
                            MessagePanel.Children.Insert(0, item);
                        }
                    }
                });
            }
        }

        // Метод для нахождения родительского ListBoxItem
        private T FindAncestor<T>(DependencyObject current) where T : DependencyObject
        {
            do
            {
                if (current is T result)
                    return result;
                current = VisualTreeHelper.GetParent(current);
            }
            while (current != null);
            return null;
        }

        private async void SendMessage(object sender, RoutedEventArgs e)
        {
            string message = messageTextBox.Text;
            messageTextBox.Text = "";
            try
            {

                await _hubConnection.InvokeAsync("SendMessage",_currentRoom, _userData.Name + "  " + DateTime.Now, message);
                messageTextBox.Text = null;

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка отправки сообщения: {ex.Message}");
            }

        }


        private void OpenSettings_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _viewModel.OpenSettings(MainGrid, SettingsGrid);
        }
        private void CloseSettings_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _viewModel.OpenSettings(SettingsGrid, MainGrid);
        }

        private void ProfileSettings_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SaveNewProfileImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.ShowReadOnly = true;
            dialog.ShowDialog();
            string filePath = dialog.FileName;
            BitmapImage image = new BitmapImage(new Uri(filePath, UriKind.Relative));
            using (var fileStream = new FileStream(_profileImagePathAbsolute, FileMode.Create))
            {
                BitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(image));
                encoder.Save(fileStream);
            }
            UpdateProfileImage();
        }

        private void UpdateProfileImage()
        {
            if(File.Exists(_profileImagePathAbsolute))
            {
                Console.WriteLine("Было обновлена картинка профиля");
                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.UriSource = new Uri(_profileImagePathRelative, UriKind.Relative);
                image.EndInit();
                Console.WriteLine(image.UriSource.ToString());
                profileImage.Source = image;
            }

        }

        private void friendadd_MouseDown(object sender, MouseButtonEventArgs e)
        {
            addFriendPanel.Visibility = Visibility.Visible;
        }

        private void SendFriendRequestButton_Click(object sender, RoutedEventArgs e)
        {
            int friendId;

            if (Int32.TryParse(addFriendTextBox.Text, out int result))
            {
                friendId = result;
                _friendSystem.SendFriendRequest(friendId, _userData.Id, _userData.Name);
            }
            else
            {
                MessageBox.Show("Был неправильно введен ID пользователя!");
            }
        }

        private void CloseFriendAddPanel(object sender, MouseButtonEventArgs e)
        {
            addFriendPanel.Visibility = Visibility.Hidden;
            addFriendTextBox.Text = "";
        }

        private void ChangeUserNameButton(object sender, RoutedEventArgs e)
        {
            if(changeUserNameTextBox.Text != "")
            {
                _userData.Name = changeUserNameTextBox.Text;
                userName.Content = _userData.Name;
                DbUtils.ConnectionDB();
                DbUtils.command.CommandText = "UPDATE userdata SET name = @name WHERE id=@id";
                DbUtils.command.Parameters.AddWithValue("@name", _userData.Name);
                DbUtils.command.Parameters.AddWithValue("@id", _userData.Id);
                DbUtils.command.ExecuteNonQuery();
                DbUtils.CloseDB();
            }


        }

        private void AddFriendButton_Click(object sender, RoutedEventArgs e)
        {
            Friend friend = (sender as FrameworkElement).DataContext as Friend;
            _friendSystem.AddFriend(friend);
        }
    }
}
