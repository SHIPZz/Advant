using Code.Common.Components;
using Code.Gameplay.Business.Components;
using Leopotam.EcsLite;

namespace Code.Gameplay.Business.Systems
{
    public class UpdateBusinessOnUpgradePurchasedSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld _world;
        private EcsFilter _businesses;
        private EcsFilter _upgradeRequests;

        private EcsPool<BusinessIdComponent> _businessIdPool;
        private EcsPool<UpgradePurchasedRequestComponent> _upgradeRequestPool;
        private EcsPool<UpdateBusinessModifiersComponent> _updateBusinessModifiersPool;

        public void Init(IEcsSystems systems)
        {
            _world = systems.GetWorld();
            InitializeFilters();
            InitializePools();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (int request in _upgradeRequests)
            {
                var upgradeRequest = _upgradeRequestPool.Get(request).Value;

                foreach (int business in _businesses)
                {
                    if (!IsMatchingBusiness(business, upgradeRequest.BusinessId))
                        continue;

                    ProcessUpgradeModifier(business, upgradeRequest.UpgradeId);
                }
            }
        }

        private bool IsMatchingBusiness(int business, int businessId)
        {
            return _businessIdPool.Get(business).Value == businessId;
        }

        private void ProcessUpgradeModifier(int business, int upgradeId)
        {
            ref var upgradeDatas = ref _updateBusinessModifiersPool.Get(business).Value;
            var upgradeData = upgradeDatas[upgradeId];
            upgradeData.Purchased = true;
        }

        private void InitializeFilters()
        {
            _businesses = _world.Filter<BusinessComponent>()
                .Inc<BusinessIdComponent>()
                .Inc<UpdateBusinessModifiersComponent>()
                .Inc<NameComponent>()
                .End();

            _upgradeRequests = _world.Filter<UpgradePurchasedRequestComponent>().End();
        }

        private void InitializePools()
        {
            _businessIdPool = _world.GetPool<BusinessIdComponent>();
            _upgradeRequestPool = _world.GetPool<UpgradePurchasedRequestComponent>();
            _updateBusinessModifiersPool = _world.GetPool<UpdateBusinessModifiersComponent>();
        }
    }
} 