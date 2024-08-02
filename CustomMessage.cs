using System;
using System.Windows.Controls;
using System.Windows.Media;

namespace CrystalMessagerWPF
{
    internal class CustomMessage : Panel
    {
        private string _userName;
        private string _message;
        private Grid _grid;
        public CustomMessage(string userName, string message) 
        {
            _userName = userName;
            _message = message;
        }

        public TextBlock[] CreateMessage()
        {
            TextBlock nameBox = new TextBlock();
            nameBox.Text = _userName + "     ";
            nameBox.FontFamily = new FontFamily("Bahnschrift SemiBold");
            nameBox.FontSize = 16;
            nameBox.Height = 20;
            nameBox.Foreground = new SolidColorBrush(Color.FromArgb(100, 147, 146, 169));
            nameBox.Width = 473;
            TextBlock messageBox = new TextBlock();
            messageBox.Text = _message;
            messageBox.FontFamily = new FontFamily("Bahnschrift Light");
            messageBox.FontSize = 14;
            messageBox.Height = 30;
            messageBox.Width = 478;
            messageBox.TextWrapping = System.Windows.TextWrapping.Wrap;
            TextBlock[] massive = new TextBlock[] { nameBox, messageBox };
            return massive;
        }
    }
}
