using AutoMapper;

using FlyFramework.Repositories.Uow;


namespace FlyFramework.Domain.ApplicationServices
{
    public abstract class ApplicationServiceBase
    {
        protected ApplicationServiceBase()
        {
            //LocalizationManager = NullLocalizationManager.Instance;
        }
        public IServiceProvider ServiceProvider { get; set; } = default!;
        public IMapper ObjectMapper { get; set; }

        private IUnitOfWorkManager _unitOfWorkManager;
        //private ILocalizationSource _localizationSource;
        public IUnitOfWorkManager UnitOfWorkManager
        {
            get
            {
                if (_unitOfWorkManager == null)
                {
                    throw new Exception("Must set UnitOfWorkManager before use it.");
                }

                return _unitOfWorkManager;
            }
            set
            {
                _unitOfWorkManager = value;
            }
        }

    }
}
