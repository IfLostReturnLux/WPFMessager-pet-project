using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Controls;

namespace CrystalMessagerWPF
{
    internal class MainWindowViewModel : INotifyPropertyChanged
    {
        public int ID;
        public MainWindowViewModel(int id) 
        {
            Friends = new ObservableCollection<Friend>();
            ID = id;
        }
        public event PropertyChangedEventHandler PropertyChanged;
        private ObservableCollection<Friend> _friends;
        public ObservableCollection<Friend> Friends
        {
            get { return _friends; }
            set
            {
                _friends = value;
                OnPropertyChanged(nameof(Friends));
            }
        }
        private ObservableCollection<Friend> _friendsToAdd;
        public ObservableCollection<Friend> FriendsToAdd
        {
            get { return _friendsToAdd; }
            set
            {
                _friendsToAdd = value;
                OnPropertyChanged(nameof(FriendsToAdd));
            }
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void OpenSettings(Grid closeGrid, Grid openGrid)
        {
            closeGrid.Visibility = System.Windows.Visibility.Hidden;
            openGrid.Visibility = System.Windows.Visibility.Visible;
        }
    }
}
