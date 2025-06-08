using System.Collections.Generic;
using Code.Components;
using Code.Configs;
using Code.Extensions;
using Code.Requests;
using Code.Gameplay.Business;
using Code.Gameplay.Money;
using Code.Utils;
using Leopotam.EcsLite;
using UnityEngine;

namespace Code.Systems.Business
{
    public class UpdateBusinessOnRequestSystem : IEcsInitSystem, IEcsRunSystem
    {
        private readonly BusinessService _businessService;
        private readonly IMoneyService _moneyService;

        private EcsWorld _world;
        private EcsFilter _businesses;
        private EcsFilter _updateRequest;

        private EcsPool<BusinessIdComponent> _businessIdPool;
        private EcsPool<UpdateBusinessRequestComponent> _businessUpdateRequestPool;
        private EcsPool<IncomeComponent> _incomePool;
        private EcsPool<LevelComponent> _levelPool;
        private EcsPool<LevelUpPriceComponent> _levelUpPricePool;
        private EcsPool<PurchasedComponent> _purchasedPool;
        private EcsPool<IncomeСooldownAvailableComponent> _incomeCooldownAvailablePool;
        private EcsPool<UpdateBusinessModifiersComponent> _updateBusinessModifiersPool;
        private EcsPool<BaseIncomeComponent> _baseIncomePool;
        private EcsPool<BaseCostComponent> _baseCostPool;
        private EcsPool<NameComponent> _namePool;

        public UpdateBusinessOnRequestSystem(BusinessService businessService, IMoneyService moneyService)
        {
            _moneyService = moneyService;
            _businessService = businessService;
        }

        public void Init(IEcsSystems systems)
        {
            _world = systems.GetWorld();

            _businesses = _world.Filter<BusinessComponent>()
                .Inc<BusinessIdComponent>()
                .Inc<IncomeComponent>()
                .Inc<UpdateBusinessModifiersComponent>()
                .Inc<LevelComponent>()
                .Inc<BaseCostComponent>()
                .Inc<LevelUpPriceComponent>()
                .End();

            _businessIdPool = _world.GetPool<BusinessIdComponent>();

            _updateRequest = _world.Filter<UpdateBusinessRequestComponent>()
                .End();

            _businessUpdateRequestPool = _world.GetPool<UpdateBusinessRequestComponent>();
            _incomePool = _world.GetPool<IncomeComponent>();
            _baseIncomePool = _world.GetPool<BaseIncomeComponent>();
            _baseCostPool = _world.GetPool<BaseCostComponent>();
            _levelPool = _world.GetPool<LevelComponent>();
            _updateBusinessModifiersPool = _world.GetPool<UpdateBusinessModifiersComponent>();
            _purchasedPool = _world.GetPool<PurchasedComponent>();
            _incomeCooldownAvailablePool = _world.GetPool<IncomeСooldownAvailableComponent>();
            _levelUpPricePool = _world.GetPool<LevelUpPriceComponent>();
            _namePool = _world.GetPool<NameComponent>();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (int updateRequest in _updateRequest)
            foreach (int business in _businesses)
            {
                int id = _businessIdPool.Get(business).Value;
                UpdateBusinessRequest upgradeRequest = _businessUpdateRequestPool.Get(updateRequest).Value;

                if (upgradeRequest.Id != id)
                    continue;

                UpdateData(business, upgradeRequest);
            }
        }

        private void UpdateData(int business, UpdateBusinessRequest upgradeRequest)
        {
            int businessId = _businessIdPool.Get(business).Value;
            ref int levelUpPrice = ref _levelUpPricePool.Get(business).Value;

            ref List<UpgradeData> upgradeDatas = ref _updateBusinessModifiersPool.Get(business).Value;

            Debug.Log($"{upgradeRequest.UpdateModifierData.Id} - id");

            if (upgradeRequest.UpdateModifierData.Id > -1)
            {
                UpgradeData upgradeData = upgradeDatas[upgradeRequest.UpdateModifierData.Id];

                if (!upgradeData.Purchased)
                    upgradeData.Purchased = true;

                Debug.Log($"{upgradeData.Purchased} - {upgradeData.Cost} - {upgradeData.IncomeMultiplier} - {upgradeRequest.Id}");
            }

            ref int level = ref _levelPool.Get(business).Value;
            ref int income = ref _incomePool.Get(business).Value;
            int baseIncome = _baseIncomePool.Get(business).Value;
            int baseCost = _baseCostPool.Get(business).Value;

            if (upgradeRequest.Level > -1)
                level += upgradeRequest.Level;

            income = CalculateNewIncome(upgradeDatas, level, baseIncome);
            levelUpPrice = BusinessCalculator.CalculateLevelUpPrice(level, baseCost);

            MarkPurchasedIfNot(business, level);

            MarkIncomeCooldownAvailableIfNot(business, level);

            string name = _namePool.Get(business).Value;

            _businessService.NotifyBusinessDataUpdated(businessId, level, income, levelUpPrice, name);
        }

        private static int CalculateNewIncome(List<UpgradeData> upgradeDatas, int level, int baseIncome)
        {
            float firstModifier = upgradeDatas[0].Purchased ? upgradeDatas[0].IncomeMultiplier : 0;
            float secondModifier = upgradeDatas[1].Purchased ? upgradeDatas[1].IncomeMultiplier : 0;

            Debug.Log($"{secondModifier}");

            return Mathf.RoundToInt(
                BusinessCalculator.CalculateIncome(level, baseIncome, firstModifier, secondModifier));
            ;
        }

        private void MarkIncomeCooldownAvailableIfNot(int business, int level)
        {
            if (!_incomeCooldownAvailablePool.Has(business) && level > 0)
                _incomeCooldownAvailablePool.Add(business).Value = true;
        }

        private void MarkPurchasedIfNot(int business, int level)
        {
            if (!_purchasedPool.Has(business) && level > 0)
                _purchasedPool.Add(business).Value = true;
        }
    }
}