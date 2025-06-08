using Code.Components;
using Code.Requests;
using Leopotam.EcsLite;

namespace Code.Systems
{
    public class UpdateBusinessOnRequestSystem : IEcsInitSystem, IEcsRunSystem
    {
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

        public void Init(IEcsSystems systems)
        {
            _world = systems.GetWorld();

            _businesses = _world.Filter<BusinessComponent>()
                .Inc<BusinessIdComponent>()
                .Inc<IncomeComponent>()
                .Inc<LevelComponent>()
                .Inc<LevelUpPriceComponent>()
                .End();

            _businessIdPool = _world.GetPool<BusinessIdComponent>();

            _updateRequest = _world.Filter<UpdateBusinessRequestComponent>()
                .End();

            _businessUpdateRequestPool = _world.GetPool<UpdateBusinessRequestComponent>();
            _incomePool = _world.GetPool<IncomeComponent>();
            _levelPool = _world.GetPool<LevelComponent>();
            _purchasedPool = _world.GetPool<PurchasedComponent>();
            _incomeCooldownAvailablePool = _world.GetPool<IncomeСooldownAvailableComponent>();
            _levelUpPricePool = _world.GetPool<LevelUpPriceComponent>();
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

                if (!IsBought(upgradeRequest)) 
                    continue;
                
                UpdateData(business, upgradeRequest);
                
                MarkPurchasedIfNot(business);
                
                MarkIncomeCooldownAvailableIfNot(business);
            }
        }

        private static bool IsBought(UpdateBusinessRequest upgradeRequest)
        {
            return upgradeRequest.Level > 0;
        }

        private void MarkIncomeCooldownAvailableIfNot(int business)
        {
            if(!_incomeCooldownAvailablePool.Has(business))
                _incomeCooldownAvailablePool.Add(business).Value = true;
        }

        private void MarkPurchasedIfNot(int business)
        {
            if(!_purchasedPool.Has(business))
                _purchasedPool.Add(business).Value = true;
        }

        private void UpdateData(int business, UpdateBusinessRequest upgradeRequest)
        {
            _levelUpPricePool.Get(business).Value = upgradeRequest.LevelUpPrice;
            _levelPool.Get(business).Value = upgradeRequest.Level;
            _incomePool.Get(business).Value = upgradeRequest.Income;
        }
    }
}