using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System.IO;    

namespace SignalR
{
    public class ChatHub : Hub
    {
        private Dictionary<string, List<Message>> _messageList;
        private string _path = "D://webserver//messages";
        public ChatHub()
        {
            _messageList = new Dictionary<string, List<Message>>();
        }
        public async Task SendMessage(string roomName ,string user, string message)
        {
            //Отправка сообщения и сохранения его в комнате
            await Clients.Group(roomName).SendAsync("RecieveMessage",roomName , user, message);
            Console.WriteLine($"Пришло сообщение от пользователя в комнате {roomName}, {user}:{message}");
            Message msg = new Message(user, message);
           
            try
            {
                _messageList[roomName].Add(msg);
            }
            catch (Exception ex)
            {
                _messageList.Add(roomName, new List<Message>());
                _messageList[roomName].Add(msg);
            }

            //Сериализаяция сообщений в json файл
            string json;
            List<Message> tempMsgList = new List<Message>();
            if(File.Exists(_path + $"/{roomName}.json")) 
            {
                json = File.ReadAllText(_path + $"/{roomName}.json");
                tempMsgList = JsonConvert.DeserializeObject<List<Message>>(json) ?? new List<Message>();
                tempMsgList.Add(msg);
            }
            else
            {
                tempMsgList.Add(msg);
            }
            json = JsonConvert.SerializeObject(tempMsgList);
            File.WriteAllText(_path + $"/{roomName}.json", json);
            Console.WriteLine(json);
        }

        public async Task JoinRoom(string roomName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, roomName);
            List<Message> msgHistory;

            if(File.Exists(_path + $"/{roomName}.json"))
            {
                string json = File.ReadAllText(_path + $"/{roomName}.json");
                msgHistory = JsonConvert.DeserializeObject<List<Message>>(json);
            }
            else
            {
                msgHistory = new List<Message>();
                msgHistory.Add(new Message("Система", "Это сообщение обозначает начало вашей истории сообщений."));
                string json = JsonConvert.SerializeObject(msgHistory);
                File.WriteAllText(_path + $"/{roomName}.json", json);
            }


            await Clients.Group(roomName).SendAsync("RecieveMessageHistory", roomName, msgHistory);
        }
        public async Task LeaveRoom(string roomName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomName);
        }



    }
}
