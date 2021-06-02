using MVVM_Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BL.Admin;
using System.Windows.Controls;

namespace Main.ViewModels
{
    public class LoginAdminViewModel : BasePageViewModel
    {
        private readonly AdminService adminService;

        public LoginAdminViewModel(PageManager pageservice, AdminService adminService) : base(pageservice)
        {
            this.adminService = adminService;
        }

        public string Login { get; set; }
        public string Error { get; set; }
        public bool ErrorVis { get; set; }
        public PasswordBox PasswordBox { get; set; } = new PasswordBox();

        protected override void Next(object param)
        {
            ErrorVis = false;
            if(adminService.Login(Login, PasswordBox.Password))
            {
                pageservice.ChangeNewPage<Pages.AdminPage>(BackSlideAnim);
            }
            else
            {
                ErrorVis = true;
                Error = adminService.ErrorMessage;
            }
        }

    }
}
