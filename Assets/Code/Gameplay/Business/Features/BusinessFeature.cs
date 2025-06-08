using Code.Common.Features;
using Code.Common.Services;
using Code.Gameplay.Business;
using Code.Gameplay.Business.Configs;
using Code.Gameplay.Business.Systems;
using Code.Gameplay.Save;
using Leopotam.EcsLite;

namespace Code.Gameplay.Business.Features
{
    public class BusinessFeature : Feature
    {
        private readonly BusinessService _businessService;
        private readonly IIdentifierService _identifierService;
        private readonly BusinessUpgradeNamesConfig _businessUpgradeNamesConfig;
        private readonly BusinessConfig _businessConfig;
        private readonly ISaveService _saveService;

        public BusinessFeature(
            EcsWorld world, 
            IEcsSystems systems,
            BusinessService businessService,
            IIdentifierService identifierService,
            BusinessUpgradeNamesConfig businessUpgradeNamesConfig,
            BusinessConfig businessConfig, 
            ISaveService saveService) 
            : base(world, systems)
        {
            _businessService = businessService;
            _identifierService = identifierService;
            _businessUpgradeNamesConfig = businessUpgradeNamesConfig;
            _businessConfig = businessConfig;
            _saveService = saveService;
        }

        public override void RegisterSystems()
        {
            Systems
                .Add(new BusinessInitSystem(_businessUpgradeNamesConfig, _identifierService, _businessConfig, _businessService, _saveService))
                .Add(new CalculateIncomeCooldownSystem())
                .Add(new CalculateBusinessProgressSystem(_businessService))
                .Add(new UpdateBusinessOnRequestSystem(_businessService))
                .Add(new CreateSaveRequestOnBusinessUpdateSystem())
                .Add(new CleanupBusinessRequestsSystem());
        }
    }
} 