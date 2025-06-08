using Code.Common.Features;
using Code.Common.Services;
using Code.Gameplay.Business;
using Code.Gameplay.Business.Configs;
using Code.Gameplay.Business.Systems;
using Code.Gameplay.Money;
using Leopotam.EcsLite;

namespace Code.Gameplay.Business.Features
{
    public class BusinessFeature : Feature
    {
        private readonly BusinessService _businessService;
        private readonly IMoneyService _moneyService;
        private readonly IIdentifierService _identifierService;
        private readonly BusinessUpgradeNamesConfig _businessUpgradeNamesConfig;
        private readonly BusinessConfig _businessConfig;

        public BusinessFeature(
            EcsWorld world, 
            IEcsSystems systems,
            BusinessService businessService,
            IMoneyService moneyService,
            IIdentifierService identifierService,
            BusinessUpgradeNamesConfig businessUpgradeNamesConfig,
            BusinessConfig businessConfig) 
            : base(world, systems)
        {
            _businessService = businessService;
            _moneyService = moneyService;
            _identifierService = identifierService;
            _businessUpgradeNamesConfig = businessUpgradeNamesConfig;
            _businessConfig = businessConfig;
        }

        public override void RegisterSystems()
        {
            Systems
                .Add(new BusinessInitSystem(_businessUpgradeNamesConfig, _identifierService, _businessConfig, _businessService))
                .Add(new CalculateIncomeCooldownSystem())
                .Add(new CalculateBusinessProgressSystem(_businessService))
                .Add(new UpdateBusinessOnRequestSystem(_businessService, _moneyService))
                .Add(new CleanupBusinessRequestsSystem());
        }
    }
} 