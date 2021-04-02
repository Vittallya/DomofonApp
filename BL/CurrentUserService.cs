using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL;
using DAL.Models;


namespace BL
{
    public class UserService
    {
        public IUser CurrentUser { get; private set; }

        public bool IsAutorized { get; private set; }

        public bool IsSkipped { get; set; }

        public event Action Autorized;
        public event Action Exited;
        public event Action Skipped;

        public void Skip()
        {
            IsSkipped = true;
            IsAutorized = false;
            Skipped?.Invoke();
        }

        public void Logout()
        {
            CurrentUser = null;
            IsAutorized = IsSkipped = false;
            Exited?.Invoke();
        }

        public void SetupUser(IUser user)
        {
            CurrentUser = user;
            IsAutorized = true;
            Autorized?.Invoke();
        }
        
    }
}
