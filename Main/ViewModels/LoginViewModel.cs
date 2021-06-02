using System.Threading.Tasks;
using BL;
using System.Windows.Input;
using MVVM_Core;
using System.Windows.Controls;

namespace Main.ViewModels
{
    public class LoginViewModel : BasePageViewModel
    {
        private readonly LoginService loginService;
        private readonly EventBus eventBus;

        public LoginViewModel(LoginService loginService, PageManager pageService, EventBus eventBus) : base(pageService)
        {
            this.loginService = loginService;
            this.eventBus = eventBus;
        }
        public bool IsErrorVisible { get; set; }
        public string ErrorMessage { get; set; }

        public bool IsAnimationVisible { get; set; }

        public string Login { get; set; }
        public PasswordBox PasswordBox { get; set; } = new PasswordBox();



        public ICommand LoginCommand => new CommandAsync(async x =>
        {
            IsErrorVisible = false;

            var id = await loginService.Login(Login, PasswordBox.Password);

            if (id > -1)
            {
                await eventBus.Publish(new Events.AccountEntered(id));

                if (!pageservice.Next())
                {
                    pageservice.Back(BackSlideAnim, true);
                }
            }
            else
            {
                IsErrorVisible = true;
                ErrorMessage = loginService.ErrorMessage;
            }


        }, y => Login != null && Login.Length > 1 && PasswordBox.Password != null && PasswordBox.Password.Length > 1);

        public override int PoolIndex => Rules.Pages.MainPool;
    }
}
