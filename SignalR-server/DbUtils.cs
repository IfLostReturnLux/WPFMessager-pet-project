using MySql.Data.MySqlClient;

namespace SignalR
{
    public class DbUtils
    {
        static public MySqlDataAdapter myDataAdapter;
        static readonly string connString = "server=localhost; user=root; password=root;database=users";
        static MySqlConnection myconnect;
        static public MySqlCommand command;

        public static bool ConnectionDB()
        {
            try
            {
                myconnect = new MySqlConnection(connString);
                myconnect.Open();
                command = new MySqlCommand();
                command.Connection = myconnect;
                myDataAdapter = new MySqlDataAdapter(command);
                return (true);

            }
            catch
            {
                return (false);
            }
        }
        public static void CloseDB()
        {
            myconnect.Close();
        }
        public MySqlConnection GetConnection()
        {
            return myconnect;
        }

    }
}
