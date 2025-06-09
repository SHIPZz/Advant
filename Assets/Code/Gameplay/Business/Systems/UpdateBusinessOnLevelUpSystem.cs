using System.Collections.Generic;
using Code.Common.Components;
using Code.Gameplay.Business.Components;
using Code.Gameplay.Business.Configs;
using Code.Gameplay.Business.Requests;
using Leopotam.EcsLite;

namespace Code.Gameplay.Business.Systems
{
    public class UpdateBusinessOnLevelUpSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld _world;
        private EcsFilter _businesses;
        private EcsFilter _levelUpRequests;

        private EcsPool<BusinessIdComponent> _businessIdPool;
        private EcsPool<LevelUpRequestComponent> _levelUpRequestPool;
        private EcsPool<LevelComponent> _levelPool;
        private EcsPool<PurchasedComponent> _purchasedPool;
        private EcsPool<IncomeСooldownAvailableComponent> _incomeCooldownAvailablePool;
        private EcsPool<AccumulatedModifierComponent> _accumulatedModifierComponent;

        public void Init(IEcsSystems systems)
        {
            _world = systems.GetWorld();
            InitializeFilters();
            InitializePools();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (int updateRequest in _levelUpRequests)
            {
                var request = _levelUpRequestPool.Get(updateRequest).Value;

                if (request.Level <= -1)
                    continue;

                foreach (int business in _businesses)
                {
                    if (!IsMatchingBusiness(business, request))
                        continue;

                    UpdateBusinessLevel(business, request.Level);
                    UpdateBusinessState(business);

                   ResetUpgradeModifiers(business);
                }
            }
        }

        private bool IsMatchingBusiness(int business, LevelUpRequest request)
        {
            int businessId = _businessIdPool.Get(business).Value;
            return request.BusinessId == businessId;
        }

        private void ResetUpgradeModifiers(int business)
        {
            ref List<AccumulatedModifiersData> accumulatedModifiersDatas = ref _accumulatedModifierComponent.Get(business).Value;

            foreach (AccumulatedModifiersData accumulatedModifiersData in accumulatedModifiersDatas)
            {
                accumulatedModifiersData.Purchased = false;
            }
        }

        private void UpdateBusinessLevel(int business, int level)
        {
            ref var currentLevel = ref _levelPool.Get(business).Value;
            currentLevel += level;
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
                .Inc<LevelComponent>()
                .Inc<NameComponent>()
                .End();

            _levelUpRequests = _world.Filter<LevelUpRequestComponent>()
                .End();
        }

        private void InitializePools()
        {
            _businessIdPool = _world.GetPool<BusinessIdComponent>();
            _levelUpRequestPool = _world.GetPool<LevelUpRequestComponent>();
            _accumulatedModifierComponent = _world.GetPool<AccumulatedModifierComponent>();
            _levelPool = _world.GetPool<LevelComponent>();
            _purchasedPool = _world.GetPool<PurchasedComponent>();
            _incomeCooldownAvailablePool = _world.GetPool<IncomeСooldownAvailableComponent>();
        }
    }
} 