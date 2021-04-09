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
        }

        protected override async void Next()
        {
            registerService.SetupClient(ClientDto);

            if (IsProfileRegister)
            {
                registerService.SetupProfile(ProfileDto);
            }

            var res = await registerService.RegisterAsync();

            if (res.Item1)
            {
                ClientDto.Id = res.Item2;
                await eventBus.Publish(new Events.ClientRegistered(ClientDto));
            }
        }

        public override int PoolIndex => Rules.Pages.MainPool;
    }
}