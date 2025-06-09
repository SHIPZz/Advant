using System.Collections.Generic;
using Code.Common.Components;
using Code.Gameplay.Business.Components;
using Code.Gameplay.Business.Configs;
using Code.Utils;
using Leopotam.EcsLite;
using UnityEngine;

namespace Code.Gameplay.Business.Systems
{
    public class CalculateTotalIncomeOnCooldownUpSystem : IEcsPostRunSystem, IEcsInitSystem
    {
        private EcsWorld _world;
        private EcsFilter _businesses;
        
        private EcsPool<IncomeСooldownUpComponent> _cooldownUpPool;
        private EcsPool<UpdateBusinessModifiersComponent> _updateBusinessModifiersPool;
        private EcsPool<LevelComponent> _levelPool;
        private EcsPool<BaseIncomeComponent> _baseIncomePool;
        private EcsPool<TotalIncomeComponent> _totalIncomePool;
        
        public void Init(IEcsSystems systems)
        {
            _world = systems.GetWorld();
            InitializeFilters();
            InitializePools();
        }

        public void PostRun(IEcsSystems systems)
        {
            foreach (int business in _businesses)
            {
                if (!IsCooldownUp(business))
                    continue;

                CalculateAndSetTotalIncome(business);
            }
        }

        private bool IsCooldownUp(int business)
        {
            return _cooldownUpPool.Get(business).Value;
        }

        private void CalculateAndSetTotalIncome(int business)
        {
            var level = _levelPool.Get(business).Value;
            var baseIncome = _baseIncomePool.Get(business).Value;
            var modifiers = _updateBusinessModifiersPool.Get(business).Value;

            var (firstModifier, secondModifier) = GetIncomeModifiers(modifiers);
            
            var totalIncome = Mathf.RoundToInt(BusinessCalculator.CalculateIncome(
                level,
                baseIncome,
                firstModifier,
                secondModifier));

            _totalIncomePool.Get(business).Value = totalIncome;
        }

        private (float firstModifier, float secondModifier) GetIncomeModifiers(List<UpgradeData> modifiers)
        {
            float firstModifier = modifiers[0].Purchased ? modifiers[0].IncomeMultiplier : 0f;
            float secondModifier = modifiers[1].Purchased ? modifiers[1].IncomeMultiplier : 0f;
            
            return (firstModifier, secondModifier);
        }

        private void InitializeFilters()
        {
            _businesses = _world.Filter<IncomeСooldownLeftComponent>()
                .Inc<IncomeСooldownComponent>()
                .Inc<IncomeСooldownUpComponent>()
                .Inc<PurchasedComponent>()
                .Inc<BusinessComponent>()
                .Inc<BaseIncomeComponent>()
                .Inc<UpdateBusinessModifiersComponent>()
                .Inc<IdComponent>()
                .End();
        }

        private void InitializePools()
        {
            _cooldownUpPool = _world.GetPool<IncomeСooldownUpComponent>();
            _updateBusinessModifiersPool = _world.GetPool<UpdateBusinessModifiersComponent>();
            _levelPool = _world.GetPool<LevelComponent>();
            _baseIncomePool = _world.GetPool<BaseIncomeComponent>();
            _totalIncomePool = _world.GetPool<TotalIncomeComponent>();
        }
    }
} 