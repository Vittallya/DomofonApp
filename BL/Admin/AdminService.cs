using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Admin
{
    public class AdminService
    {
        public bool IsAutorized { get; private set; }

        public string ErrorMessage { get; private set; }

        public string Name { get; private set; }

        public bool Login(string login, string password)
        {
            IsAutorized = false;

            if(string.Compare(login, Properties.Resources.AdminLogin) == 0 && string.Compare(password, Properties.Resources.AdminPassword) == 0)
            {
                IsAutorized = true;
                Name = "Администратор";
            }
            else
            {
                ErrorMessage = "Неверный логин и (или) пароль";
            }


            return IsAutorized;
        }
    }
}
