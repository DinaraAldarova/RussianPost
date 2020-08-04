using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace post_service
{
    public class AuthInfo
    {
        public string Login { get; private set; }
        public string Password { get; private set; }

        public AuthInfo(string login, string password)
        {
            Login = login;
            Password = password;
        }
    }
}
