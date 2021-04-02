using MVVM_Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Main.Events
{
    public class AccountEntered : IEvent
    {
        public string Name { get; set;  }
    }
}
