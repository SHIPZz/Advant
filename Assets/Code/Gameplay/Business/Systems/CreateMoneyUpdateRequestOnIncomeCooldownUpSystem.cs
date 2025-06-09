using Code.Common.Components;
using Code.Gameplay.Business.Components;
using Code.Gameplay.Money.Components;
using Code.Gameplay.Money.Requests;
using Leopotam.EcsLite;

namespace Code.Gameplay.Business.Systems
{
    public class CreateMoneyUpdateRequestOnIncomeCooldownUpSystem : IEcsPostRunSystem, IEcsInitSystem
    {
        private EcsWorld _world;
        private EcsFilter _businesses;
        
        private EcsPool<IncomeСooldownUpComponent> _cooldownUpPool;
        private EcsPool<MoneyUpdateRequestComponent> _moneyUpdateRequestPool;
        private EcsPool<OwnerIdComponent> _ownerIdPool;
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

                CreateMoneyUpdateRequest(business);
            }
        }

        private bool IsCooldownUp(int business)
        {
            return _cooldownUpPool.Get(business).Value;
        }

        private void CreateMoneyUpdateRequest(int business)
        {
            var ownerId = _ownerIdPool.Get(business).Value;
            var totalIncome = _totalIncomePool.Get(business).Value;
            
            CreateNewMoneyUpdateRequest(ownerId, totalIncome);
        }

        private void CreateNewMoneyUpdateRequest(int ownerId, int totalIncome)
        {
            int updateRequest = _world.NewEntity();
            _moneyUpdateRequestPool.Add(updateRequest)
                .Value = new MoneyUpdateRequest(ownerId, totalIncome);
        }

        private void InitializeFilters()
        {
            _businesses = _world.Filter<IncomeСooldownLeftComponent>()
                .Inc<IncomeСooldownComponent>()
                .Inc<IncomeСooldownUpComponent>()
                .Inc<PurchasedComponent>()
                .Inc<BusinessComponent>()
                .Inc<TotalIncomeComponent>()
                .Inc<OwnerIdComponent>()
                .Inc<IdComponent>()
                .End();
        }

        private void InitializePools()
        {
            _cooldownUpPool = _world.GetPool<IncomeСooldownUpComponent>();
            _ownerIdPool = _world.GetPool<OwnerIdComponent>();
            _moneyUpdateRequestPool = _world.GetPool<MoneyUpdateRequestComponent>();
            _totalIncomePool = _world.GetPool<TotalIncomeComponent>();
        }
    }
}