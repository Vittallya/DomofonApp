using BL;
using DAL.Dto;
using MVVM_Core;

namespace Main.ViewModels
{
    public class ClientRegisterViewModel : BasePageViewModel
    {
        private readonly RegisterService registerService;
        private readonly EventBus eventBus;

        public ClientDto ClientDto { get; set; }
        public ProfileDto ProfileDto { get; set; } = new ProfileDto();

        public bool IsRegisterRequiered { get; set; }

        public ClientRegisterViewModel(PageService pageservice, RegisterService registerService, EventBus eventBus) : base(pageservice)
        {
            this.registerService = registerService;
            this.eventBus = eventBus;
            Init();
        }

        public bool IsProfileRegister { get; set; }
        
        void Init()
        {
            ClientDto = registerService.GetClient();
            IsRegisterRequiered = registerService.IsRegisterRequiered;
            IsProfileRegister = IsRegisterRequiered;
        }

        protected override async void Next()
        {
            IsErrorVisible = false;
            registerService.SetupClient(ClientDto);

            if (IsProfileRegister && !(await registerService.SetupProfile(ProfileDto)))
            {
                Message = registerService.ErrorMessage;
                IsErrorVisible = true;
                return;
            }

            var res = await registerService.RegisterAsync();

            if (res.Item1)
            {
                ClientDto.Id = res.Item2;
                await eventBus.Publish(new Events.ClientRegistered(ClientDto));
                registerService.Clear();
            }
        }

        public override int PoolIndex => Rules.Pages.MainPool;

        public string Message { get; private set; }

        public bool IsErrorVisible { get; set; }
    }
}