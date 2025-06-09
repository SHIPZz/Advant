using System.Collections.Generic;
using Code.Common.Components;
using Code.Common.Services;
using Code.Gameplay.Business.Components;
using Code.Gameplay.Business.Configs;
using Code.Gameplay.Business.Factory;
using Code.Gameplay.Hero.Components;
using Code.Gameplay.Save;
using Code.Gameplay.Save.Models;
using Leopotam.EcsLite;

namespace Code.Gameplay.Business.Systems
{
    public class BusinessInitSystem : IEcsInitSystem
    {
        private readonly BusinessConfig _businessConfig;
        private readonly BusinessUpgradeNamesConfig _businessUpgradeNamesConfig;
        private readonly BusinessService _businessService;
        private readonly IIdentifierService _identifierService;
        private readonly ISaveService _saveService;

        private BusinessFactory _businessFactory;

        private EcsPool<IdComponent> _idPool;
        private EcsPool<BusinessIdComponent> _businessIdPool;
        private EcsPool<LevelComponent> _levelPool;
        private EcsPool<IncomeComponent> _incomePool;
        private EcsPool<LevelUpPriceComponent> _levelUpPricePool;
        private EcsPool<ProgressComponent> _progressPool;
        private EcsPool<IncomeСooldownLeftComponent> _cooldownLeftPool;
        private EcsPool<IncomeСooldownAvailableComponent> _cooldownAvailablePool;
        private EcsPool<PurchasedComponent> _purchasedPool;
        private EcsPool<AccumulatedModifierComponent> _accumulatedModifiersPool;

        public BusinessInitSystem(
            BusinessUpgradeNamesConfig businessUpgradeNamesConfig,
            IIdentifierService identifierService,
            BusinessConfig businessConfig,
            BusinessService businessService,
            ISaveService saveService)
        {
            _identifierService = identifierService;
            _businessUpgradeNamesConfig = businessUpgradeNamesConfig;
            _businessConfig = businessConfig;
            _businessService = businessService;
            _saveService = saveService;
        }

        public void Init(IEcsSystems systems)
        {
            var world = systems.GetWorld();

            _businessFactory = new BusinessFactory(world, _identifierService);
            InitializePools(world);

            var heroId = GetHeroId(world);
            var businessDatas = _businessConfig.GetBusinessDatas();
            var saveData = _saveService.HasSave() ? _saveService.LoadGame() : null;

            InitializeBusinesses(heroId, businessDatas, saveData);
        }

        private void InitializeBusinesses(int heroId, IReadOnlyList<BusinessData> businessDatas, GameSaveModel saveData)
        {
            for (int i = 0; i < businessDatas.Count; i++)
            {
                var businessData = businessDatas[i];
                var businessNameData = _businessUpgradeNamesConfig.BusinessUpgradeNameDatas[i];

                int entity = _businessFactory.CreateBusiness(businessData, businessNameData, i, heroId);

                if (saveData != null)
                    RestoreBusinessState(entity, saveData.Businesses.Find(b => b.Id == i));

                NotifyBusinessDataUpdated(entity, businessNameData.Name);
            }
        }

        private void RestoreBusinessState(int entity, BusinessSaveModel savedBusiness)
        {
            if (savedBusiness == null)
                return;

            RestoreBasicProperties(entity, savedBusiness);
            RestoreBusinessFlags(entity, savedBusiness);
            RestoreUpgrades(entity, savedBusiness);
        }

        private void RestoreBasicProperties(int entity, BusinessSaveModel savedBusiness)
        {
            _levelPool.Get(entity).Value = savedBusiness.Level;
            _incomePool.Get(entity).Value = savedBusiness.Income;
            _levelUpPricePool.Get(entity).Value = savedBusiness.LevelUpPrice;
            _cooldownLeftPool.Get(entity).Value = savedBusiness.Cooldown;
            _progressPool.Get(entity).Value = savedBusiness.Progress;
        }

        private void RestoreBusinessFlags(int entity, BusinessSaveModel savedBusiness)
        {
            if (savedBusiness == null)
                return;

            if (_purchasedPool.Has(entity))
            {
                _purchasedPool.Get(entity).Value = savedBusiness.IsPurchased;
            }
            else
            {
                _purchasedPool.Add(entity).Value = savedBusiness.IsPurchased;
            }

            if (savedBusiness.IsCooldownAvailable && !_cooldownAvailablePool.Has(entity))
                _cooldownAvailablePool.Add(entity).Value = true;
        }

        private void RestoreUpgrades(int entity, BusinessSaveModel savedBusiness)
        {
            if (!_accumulatedModifiersPool.Has(entity))
                return;

            ref List<AccumulatedModifiersData> accumulatedModifiers = ref _accumulatedModifiersPool.Get(entity).Value;
            accumulatedModifiers.Clear();

            foreach (UpgradeSaveModel savedUpgrade in savedBusiness.Upgrades)
            {
                accumulatedModifiers.Add(new AccumulatedModifiersData
                {
                    Id = savedUpgrade.Id,
                    Value = savedUpgrade.IncomeModifier,
                    Purchased = savedUpgrade.Purchased
                });
            }
        }

        private void InitializePools(EcsWorld world)
        {
            _idPool = world.GetPool<IdComponent>();
            _businessIdPool = world.GetPool<BusinessIdComponent>();
            _levelPool = world.GetPool<LevelComponent>();
            _incomePool = world.GetPool<IncomeComponent>();
            _levelUpPricePool = world.GetPool<LevelUpPriceComponent>();
            _progressPool = world.GetPool<ProgressComponent>();
            _cooldownLeftPool = world.GetPool<IncomeСooldownLeftComponent>();
            _cooldownAvailablePool = world.GetPool<IncomeСooldownAvailableComponent>();
            _purchasedPool = world.GetPool<PurchasedComponent>();
            _accumulatedModifiersPool = world.GetPool<AccumulatedModifierComponent>();
        }

        private int GetHeroId(EcsWorld world)
        {
            EcsFilter heroFilter = world.Filter<HeroComponent>().End();

            foreach (int hero in heroFilter)
                return _idPool.Get(hero).Value;

            return -1;
        }

        private void NotifyBusinessDataUpdated(int entity, string name)
        {
            var businessId = _businessIdPool.Get(entity).Value;
            var level = _levelPool.Get(entity).Value;
            var income = _incomePool.Get(entity).Value;
            var levelUpPrice = _levelUpPricePool.Get(entity).Value;
            var accumulatedModifiersDatas = _accumulatedModifiersPool.Get(entity).Value;
            
            _businessService.NotifyBusinessDataUpdated(businessId, level, income, levelUpPrice, name,  accumulatedModifiersDatas);
        }
    }
}