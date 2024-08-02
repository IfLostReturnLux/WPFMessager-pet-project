
namespace CrystalMessagerWPF
{
    public class PersonData
    {
        public string Login;
        public string Password;
        public int Id;
        public bool CheckMarkState;
        public string Name;
          public PersonData(string login, string password, int id)
          {
              Login = login;
              Password = password;
              Id = id;
          }
    }
}
