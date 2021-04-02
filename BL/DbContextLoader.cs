using DAL;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BL
{
    public class DbContextLoader
    {

        private bool _loaded;
        private bool _error;
        private AllDbContext _dbContext;

        public string Message { get; private set; }
        public string MessageDetail { get; private set; }


        public async Task<bool> LoadAsync()
        {
            Thread thread = new Thread(() =>
            {
                try
                {
                    _dbContext.Set<object>().Load();
                    _loaded = true;
                }
                catch(Exception ex)
                {
                    Message = ex.Message;
                    MessageDetail = ex?.InnerException?.Message;
                    _error = true;
                }
            });
            thread.Start();


            await Task.Run(() =>
            {
                while (!_loaded && !_error)
                {
                    Task.Delay(100);
                }
            });
            return !_error;            
        }

        public DbContextLoader(AllDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void DisposeConnection()
        {
            _dbContext.Database.Connection.Dispose();  
        }
    }
}
