using MVVM_Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Main.Events
{
    public class AccountEntered : IEvent
    {
        public AccountEntered(int id)
        {
            Id = id;
        }

        public int Id { get; set;  }
    }
}
