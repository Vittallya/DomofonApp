using Microsoft.Extensions.DependencyInjection;
using System;
using Main.ViewModels;
using DAL;
using BL;
using System.Threading.Tasks;
using Main.Pages;

namespace Main
{
    public class Locator
    {
        public static IServiceProvider Services { get; private set; }
        public static void InitServices(IServiceProvider provider)
        {
            Services = provider;
        }

        public  MainViewModel MainViewModel => Services.GetRequiredService<MainViewModel>();
        public  LoginViewModel LoginViewModel => Services.GetRequiredService<LoginViewModel>();
        public  CatalogViewModel CatalogViewModel => Services.GetRequiredService<CatalogViewModel>();
    }
}

