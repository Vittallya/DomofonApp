using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL;
using DAL.Models;

namespace BL
{    

    public class LoginService : ILoginService
    {
        private AllDbContext dbContext;

        public int Time { get; } = 5;

        public LoginService(AllDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public string ErrorMessage { get; private set; }

        public bool CheckCode(string code)
        {
            return true;
        }

        public async Task<bool> SendCodeAsync()
        {
            await Task.Delay(Time * 1000);
            return true;
        }

    }
}
