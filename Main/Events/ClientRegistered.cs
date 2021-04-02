using BL;
using DAL;
using MVVM_Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.Events
{
    public class ClientRegistered: IEvent
    {
        public ClientRegistered(IUser user)
        {
            User = user;
        }

        public IUser User { get; set; }

    }
}
