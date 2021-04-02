using System.Threading.Tasks;
using BL;
using System.Windows.Input;
using MVVM_Core;

namespace Main.ViewModels
{
    public class LoginViewModel : BasePageViewModel
    {
        private readonly ILoginService loginService;
        private readonly PageService pageService;

        public LoginViewModel(ILoginService loginService, PageService pageService) : base(pageService)
        {
            this.loginService = loginService;
            this.pageService = pageService;
        }
        public bool IsErrorVisible { get; set; }
        public string ErrorMessage { get; set; }

        public bool IsAnimationVisible { get; set; }

      


        bool _nextPossible;


        public bool IsCodeFiledVisible { get; set; }

        public ICommand GetCodeCommand => new CommandAsync(async x =>
        {
            //IsErrorVisible = false;

            //if (await loginService.SetupPhoneNumber(Phone))
            //{
            //    IsCodeFiledVisible = true;
            //}
            //else
            //{
            //    IsErrorVisible = true;
            //    ErrorMessage = loginService.ErrorMessage;
            //}
        });



        public ICommand LoginCommand => new Command(x =>
        {


        }, y => _nextPossible);

        public override int PoolIndex => Rules.Pages.MainPool;
    }
}
