using System.Collections.Generic;
using System.Linq;
using Code.Common.Components;
using Code.Gameplay.Business.Components;
using Code.Gameplay.Business.Configs;
using Code.Gameplay.Business.Utils;
using Code.Utils;
using Leopotam.EcsLite;
using UnityEngine;

namespace Code.Gameplay.Business.Systems
{
    public class RecalculateBusinessValuesSystem : IEcsInitSystem, IEcsRunSystem
    {
        private readonly BusinessService _businessService;

        private EcsWorld _world;
        private EcsFilter _businesses;

        private EcsPool<IncomeComponent> _incomePool;
        private EcsPool<LevelComponent> _levelPool;
        private EcsPool<LevelUpPriceComponent> _levelUpPricePool;
        private EcsPool<AccumulatedModifierComponent> _accumulatedModifiersPool;
        private EcsPool<BaseIncomeComponent> _baseIncomePool;
        private EcsPool<BaseCostComponent> _baseCostPool;
        private EcsPool<BusinessIdComponent> _businessIdPool;
        private EcsPool<NameComponent> _namePool;
        private EcsPool<IdComponent> _idPool;

        public RecalculateBusinessValuesSystem(BusinessService businessService)
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
            foreach (int business in _businesses)
            {
                RecalculateValues(business);
                NotifyBusinessUpdate(business);
            }
        }

        private void RecalculateValues(int business)
        {
            var level = _levelPool.Get(business).Value;
            var baseIncome = _baseIncomePool.Get(business).Value;
            var baseCost = _baseCostPool.Get(business).Value;
            var modifiers = _accumulatedModifiersPool.Get(business).Value;

            ref var income = ref _incomePool.Get(business).Value;
            ref var levelUpPrice = ref _levelUpPricePool.Get(business).Value;

            var (firstModifier, secondModifier) = BusinessModifierUtils.GetModifiers(modifiers);

            income = Mathf.RoundToInt(BusinessCalculator.CalculateIncome(level, baseIncome, firstModifier, secondModifier));
            levelUpPrice = BusinessCalculator.CalculateLevelUpPrice(level, baseCost);
        }

        private void NotifyBusinessUpdate(int business)
        {
            var businessId = _businessIdPool.Get(business).Value;
            var level = _levelPool.Get(business).Value;
            var income = _incomePool.Get(business).Value;
            var levelUpPrice = _levelUpPricePool.Get(business).Value;
            var name = _namePool.Get(business).Value;
            var accumulatedModifiersDatas = _accumulatedModifiersPool.Get(business).Value;

            _businessService.NotifyBusinessDataUpdated(businessId, level, income, levelUpPrice, name,accumulatedModifiersDatas);
        }

        private void InitializeFilters()
        {
            _businesses = _world.Filter<BusinessComponent>()
                .Inc<IncomeComponent>()
                .Inc<LevelComponent>()
                .Inc<BaseIncomeComponent>()
                .Inc<BaseCostComponent>()
                .Inc<AccumulatedModifierComponent>()
                .Inc<LevelUpPriceComponent>()
                .Inc<BusinessIdComponent>()
                .Inc<NameComponent>()
                .End();
        }

        private void InitializePools()
        {
            _incomePool = _world.GetPool<IncomeComponent>();
            _levelPool = _world.GetPool<LevelComponent>();
            _idPool = _world.GetPool<IdComponent>();
            _levelUpPricePool = _world.GetPool<LevelUpPriceComponent>();
            _accumulatedModifiersPool = _world.GetPool<AccumulatedModifierComponent>();
            _baseIncomePool = _world.GetPool<BaseIncomeComponent>();
            _baseCostPool = _world.GetPool<BaseCostComponent>();
            _businessIdPool = _world.GetPool<BusinessIdComponent>();
            _namePool = _world.GetPool<NameComponent>();
        }
    }
}