using System.Collections.Generic;
using Code.Common.Components;
using Code.Gameplay.Business.Components;
using Code.Gameplay.Business.Configs;
using Code.Gameplay.Requests;
using Code.Utils;
using Leopotam.EcsLite;
using UnityEngine;

namespace Code.Gameplay.Business.Systems
{
    public class UpdateBusinessOnRequestSystem : IEcsInitSystem, IEcsRunSystem
    {
        private readonly BusinessService _businessService;

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

        public UpdateBusinessOnRequestSystem(BusinessService businessService)
        {
            _businessService = businessService;
        }

        public void Init(IEcsSystems systems)
        {
            _world = systems.GetWorld();
            InitializeFilters();
            InitializePools();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (int updateRequest in _updateRequest)
            {
                ProcessUpdateRequest(updateRequest);
            }
        }

        private void ProcessUpdateRequest(int updateRequest)
        {
            var request = _businessUpdateRequestPool.Get(updateRequest).Value;

            foreach (int business in _businesses)
            {
                if (!IsMatchingBusiness(business, request))
                    continue;

                UpdateBusinessData(business, request);
            }
        }

        private bool IsMatchingBusiness(int business, UpdateBusinessRequest request)
        {
            int businessId = _businessIdPool.Get(business).Value;
            return request.Id == businessId;
        }

        private void UpdateBusinessData(int business, UpdateBusinessRequest request)
        {
            ProcessUpgradeModifier(business, request);
            UpdateBusinessLevelAndIncome(business, request);
            UpdateBusinessState(business);
            NotifyBusinessUpdate(business);
        }

        private void ProcessUpgradeModifier(int business, UpdateBusinessRequest request)
        {
            if (request.UpdateModifierData.Id <= -1)
                return;

            ref var upgradeDatas = ref _updateBusinessModifiersPool.Get(business).Value;
            var upgradeData = upgradeDatas[request.UpdateModifierData.Id];

            if (!upgradeData.Purchased)
                upgradeData.Purchased = true;
        }

        private void UpdateBusinessLevelAndIncome(int business, UpdateBusinessRequest request)
        {
            ref var level = ref _levelPool.Get(business).Value;
            ref var income = ref _incomePool.Get(business).Value;
            ref var levelUpPrice = ref _levelUpPricePool.Get(business).Value;

            var baseIncome = _baseIncomePool.Get(business).Value;
            var baseCost = _baseCostPool.Get(business).Value;
            var upgradeDatas = _updateBusinessModifiersPool.Get(business).Value;

            if (request.Level > -1)
                level += request.Level;

            income = CalculateNewIncome(upgradeDatas, level, baseIncome);
            levelUpPrice = BusinessCalculator.CalculateLevelUpPrice(level, baseCost);
        }

        private void UpdateBusinessState(int business)
        {
            var level = _levelPool.Get(business).Value;

            if (level > 0)
            {
                MarkPurchasedIfNot(business);
                MarkIncomeCooldownAvailableIfNot(business);
            }
        }

        private void NotifyBusinessUpdate(int business)
        {
            var businessId = _businessIdPool.Get(business).Value;
            var level = _levelPool.Get(business).Value;
            var income = _incomePool.Get(business).Value;
            var levelUpPrice = _levelUpPricePool.Get(business).Value;
            var name = _namePool.Get(business).Value;

            Debug.Log($"{_purchasedPool.Get(businessId).Value} - purchased");

            _businessService.NotifyBusinessDataUpdated(businessId, level, income, levelUpPrice, name);
        }

        private static int CalculateNewIncome(List<UpgradeData> upgradeDatas, int level, int baseIncome)
        {
            float firstModifier = upgradeDatas[0].Purchased ? upgradeDatas[0].IncomeMultiplier : 0;
            float secondModifier = upgradeDatas[1].Purchased ? upgradeDatas[1].IncomeMultiplier : 0;

            return Mathf.RoundToInt(BusinessCalculator.CalculateIncome(level, baseIncome, firstModifier, secondModifier));
        }

        private void MarkIncomeCooldownAvailableIfNot(int business)
        {
            if (!_incomeCooldownAvailablePool.Has(business))
            {
                _incomeCooldownAvailablePool.Add(business).Value = true;
                return;
            }

            _incomeCooldownAvailablePool.Get(business).Value = true;
        }

        private void MarkPurchasedIfNot(int business)
        {
            if (!_purchasedPool.Has(business))
            {
                _purchasedPool.Add(business).Value = true;
                return;
            }

            _purchasedPool.Get(business).Value = true;
        }

        private void InitializeFilters()
        {
            _businesses = _world.Filter<BusinessComponent>()
                .Inc<BusinessIdComponent>()
                .Inc<IncomeComponent>()
                .Inc<UpdateBusinessModifiersComponent>()
                .Inc<LevelComponent>()
                .Inc<BaseCostComponent>()
                .Inc<LevelUpPriceComponent>()
                .End();

            _updateRequest = _world.Filter<UpdateBusinessRequestComponent>()
                .End();
        }

        private void InitializePools()
        {
            _businessIdPool = _world.GetPool<BusinessIdComponent>();
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
    }
}