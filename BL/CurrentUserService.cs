using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL;
using DAL.Dto;
using DAL.Models;


namespace BL
{
    public class UserService
    {
        public ClientDto CurrentUser { get; private set; }

        public bool IsAutorized { get; private set; }

        public event Action Autorized;
        public event Action Exited;

        public void Logout()
        {
            CurrentUser = null;
            Exited?.Invoke();
        }

        public void SetupUser(ClientDto user)
        {
            CurrentUser = user;
            IsAutorized = true;
            Autorized?.Invoke();
        }
        
    }
}
