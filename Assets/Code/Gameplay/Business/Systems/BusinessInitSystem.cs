using System.Collections.Generic;
using System.Linq;
using Code.Common.Components;
using Code.Common.Services;
using Code.Gameplay.Business.Components;
using Code.Gameplay.Business.Configs;
using Code.Gameplay.Hero.Components;
using Leopotam.EcsLite;

namespace Code.Gameplay.Business.Systems
{
    public class BusinessInitSystem : IEcsInitSystem
    {
        private readonly BusinessConfig _businessConfig;
        private readonly BusinessUpgradeNamesConfig _businessUpgradeNamesConfig;
        private readonly BusinessService _businessService;
        private readonly IIdentifierService _identifierService;
        private BusinessFactory _businessFactory;

        private EcsPool<HeroComponent> _heroPool;
        private EcsPool<IdComponent> _idPool;
        private EcsPool<BusinessIdComponent> _businessIdPool;
        private EcsPool<LevelComponent> _levelPool;
        private EcsPool<IncomeComponent> _incomePool;
        private EcsPool<LevelUpPriceComponent> _levelUpPricePool;

        public BusinessInitSystem(
            BusinessUpgradeNamesConfig businessUpgradeNamesConfig,
            IIdentifierService identifierService,
            BusinessConfig businessConfig, 
            BusinessService businessService)
        {
            _identifierService = identifierService;
            _businessUpgradeNamesConfig = businessUpgradeNamesConfig;
            _businessConfig = businessConfig;
            _businessService = businessService;
        }

        public void Init(IEcsSystems systems)
        {
            var world = systems.GetWorld();
            
            _businessFactory = new BusinessFactory(world, _identifierService);
            
            InitializePools(world);
            var heroId = GetHeroId(world);
            var businessDatas = _businessConfig.GetBusinessDatas();

            for (int i = 0; i < businessDatas.Count; i++)
            {
                var businessData = businessDatas[i];
                var businessNameData = _businessUpgradeNamesConfig.BusinessUpgradeNameDatas[i];

                int entity = _businessFactory.CreateBusiness(businessData, businessNameData, i, heroId);
                NotifyBusinessDataUpdated(world, entity, businessNameData.Name);
            }
        }

        private void InitializePools(EcsWorld world)
        {
            _heroPool = world.GetPool<HeroComponent>();
            _idPool = world.GetPool<IdComponent>();
            _businessIdPool = world.GetPool<BusinessIdComponent>();
            _levelPool = world.GetPool<LevelComponent>();
            _incomePool = world.GetPool<IncomeComponent>();
            _levelUpPricePool = world.GetPool<LevelUpPriceComponent>();
        }

        private int GetHeroId(EcsWorld world)
        {
            var heroFilter = world.Filter<HeroComponent>().End();
            foreach (var hero in heroFilter)
            {
                return _idPool.Get(hero).Value;
            }
            return -1;
        }

        private void NotifyBusinessDataUpdated(EcsWorld world, int entity, string name)
        {
            var businessId = _businessIdPool.Get(entity).Value;
            var level = _levelPool.Get(entity).Value;
            var income = _incomePool.Get(entity).Value;
            var levelUpPrice = _levelUpPricePool.Get(entity).Value;

            _businessService.NotifyBusinessDataUpdated(businessId, level, income, levelUpPrice, name);
        }
    }
}