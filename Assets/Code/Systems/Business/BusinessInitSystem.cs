using System.Collections.Generic;
using System.Linq;
using Code.Common.Services;
using Code.Components;
using Code.Configs;
using Code.Gameplay.Business;
using Leopotam.EcsLite;

namespace Code.Systems.Business
{
    public class BusinessInitSystem : IEcsInitSystem
    {
        private readonly BusinessConfig _businessConfig;
        private readonly BusinessUpgradeNamesConfig _businessUpgradeNamesConfig;
        private readonly IIdentifierService _identifierService;
        private readonly BusinessService _businessService;

        public BusinessInitSystem(
            BusinessUpgradeNamesConfig businessUpgradeNamesConfig,
            IIdentifierService identifierService,
            BusinessConfig businessConfig, BusinessService businessService)
        {
            _businessUpgradeNamesConfig = businessUpgradeNamesConfig;
            _identifierService = identifierService;
            _businessConfig = businessConfig;
            _businessService = businessService;
        }

        public void Init(IEcsSystems systems)
        {
            var world = systems.GetWorld();
            var businessPool = world.GetPool<BusinessComponent>();
            var businessIdPool = world.GetPool<BusinessIdComponent>();
            var levelPool = world.GetPool<LevelComponent>();
            var incomePool = world.GetPool<IncomeComponent>();
            var incomeCooldownPool = world.GetPool<IncomeСooldownComponent>();
            var incomeCooldownAvailablePool = world.GetPool<IncomeСooldownAvailableComponent>();
            var incomeCooldownLeftPool = world.GetPool<IncomeСooldownLeftComponent>();
            var incomeCooldownUpPool = world.GetPool<IncomeСooldownUpComponent>();
            var baseIncomePool = world.GetPool<BaseIncomeComponent>();
            var updateModifiers = world.GetPool<UpdateBusinessModifiersComponent>();
            var idPool = world.GetPool<IdComponent>();
            var levelUpPricePool = world.GetPool<LevelUpPriceComponent>();
            var purchasedPool = world.GetPool<PurchasedComponent>();
            var baseCostPool = world.GetPool<BaseCostComponent>();
            var namePool = world.GetPool<NameComponent>();
            var progressPool = world.GetPool<ProgressComponent>();
            var ownerIdPool = world.GetPool<OwnerIdComponent>();
            var heroFilter = world.Filter<HeroComponent>().End();

            var businessDatas = _businessConfig.GetBusinessDatas();

            for (int i = 0; i < businessDatas.Count; i++)
            {
                int entity = world.NewEntity();
                var businessData = businessDatas[i];
                var businessNameData = _businessUpgradeNamesConfig.BusinessUpgradeNameDatas[i];

                ref var business = ref businessPool.Add(entity);
                business.Value = true;

                ref var businessId = ref businessIdPool.Add(entity);
                businessId.Value = i;

                ref var name = ref namePool.Add(entity);
                name.Value = businessNameData.Name;

                ref var level = ref levelPool.Add(entity);
                level.Value = i == 0 ? 1 : 0;

                ref var income = ref incomePool.Add(entity);
                income.Value = businessData.BaseIncome;

                ref var baseIncome = ref baseIncomePool.Add(entity);
                baseIncome.Value = businessData.BaseIncome;

                ref var id = ref idPool.Add(entity);
                id.Value = _identifierService.Next();

                ref var ownerId = ref ownerIdPool.Add(entity);

                foreach (var hero in heroFilter)
                {
                    int heroId = idPool.Get(hero).Value;
                    ownerId.Value = heroId;
                }

                ref var incomeСooldown = ref incomeCooldownPool.Add(entity);
                incomeСooldown.Value = businessData.IncomeDelay;

                ref var incomeСooldownLeft = ref incomeCooldownLeftPool.Add(entity);
                incomeСooldownLeft.Value = businessData.IncomeDelay;

                ref var incomeСooldownUp = ref incomeCooldownUpPool.Add(entity);
                incomeСooldownUp.Value = false;

                ref var progress = ref progressPool.Add(entity);
                progress.Value = 0f;

                ref var levelUpPrice = ref levelUpPricePool.Add(entity);

                ref var baseCost = ref baseCostPool.Add(entity).Value;
                baseCost = businessData.BaseCost;

                if (level.Value > 0)
                {
                    levelUpPrice.Value = (level.Value + 1) * baseCost;

                    ref var incomeСooldownAvailable = ref incomeCooldownAvailablePool.Add(entity);
                    incomeСooldownAvailable.Value = true;
                }

                if (i == 0)
                {
                    ref var purchased = ref purchasedPool.Add(entity);
                    purchased.Value = true;
                }

                ref var modifiers = ref updateModifiers.Add(entity);
                modifiers.Value = new List<UpgradeData>(businessData.Upgrades.ToList());

                _businessService.NotifyBusinessDataUpdated(businessId.Value, level.Value, income.Value,
                    levelUpPricePool.Get(entity).Value);
            }
        }
    }
}