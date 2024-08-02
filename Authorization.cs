using System;
using ZstdSharp.Unsafe;

namespace CrystalMessagerWPF
{
    internal class Authorization
    {
        public static int Authorize(string login, string password)
        {
            DbUtils.command.CommandText = @"SELECT id from logindata WHERE login = @login and password = @password";
            DbUtils.command.Parameters.AddWithValue("@login", login);
            DbUtils.command.Parameters.AddWithValue("@password", password);
            var result = DbUtils.command.ExecuteScalar();

            if (result != null)
            {
                return Convert.ToInt32(result);
            }
            else
            {
                return 0;
            }
        }

        public static bool CheckUser(string login)
        {
            DbUtils.command.CommandText = @"SELECT login, password from logindata WHERE login = @login";
            DbUtils.command.Parameters.AddWithValue("@login", login);
            var result = DbUtils.command.ExecuteScalar();

            if (result != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
