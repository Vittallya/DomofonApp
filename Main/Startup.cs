using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BL;
using DAL;

namespace Main
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<AllDbContext>();
            services.AddTransient<DbContextLoader>();
            services.AddTransient<ILoginService, LoginService>();
            services.AddSingleton<ClientPipeHanlder>();
            services.AddTransient<Services.UpdateHandlerService>();
        }
    }
}
