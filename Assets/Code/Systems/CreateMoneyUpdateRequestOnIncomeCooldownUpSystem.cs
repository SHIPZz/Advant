using Code.Components;
using Code.Configs;
using Code.Gameplay.Business;
using Code.Requests;
using Leopotam.EcsLite;
using UnityEngine;

namespace Code.Systems
{
    public class CreateMoneyUpdateRequestOnIncomeCooldownUpSystem : IEcsPostRunSystem, IEcsInitSystem
    {
        private readonly BusinessService _businessService;

        private EcsWorld _world;

        private EcsPool<BusinessIdComponent> _businessIdPool;
        private EcsFilter _businesses;
        private EcsPool<IncomeСooldownUpComponent> _cooldownUpPool;
        private EcsPool<MoneyUpdateRequestComponent> _moneyUpdateRequestPool;
        private EcsPool<OwnerIdComponent> _ownerIdPool;

        public CreateMoneyUpdateRequestOnIncomeCooldownUpSystem(BusinessService businessService)
        {
            _businessService = businessService;
        }

        public void Init(IEcsSystems systems)
        {
            _world = systems.GetWorld();

            _businesses = _world.Filter<IncomeСooldownLeftComponent>()
                .Inc<IncomeСooldownComponent>()
                .Inc<IncomeСooldownUpComponent>()
                .Inc<PurchasedComponent>()
                .Inc<BusinessComponent>()
                .Inc<OwnerIdComponent>()
                .Inc<IdComponent>()
                .End();

            _cooldownUpPool = _world.GetPool<IncomeСooldownUpComponent>();
            _businessIdPool = _world.GetPool<BusinessIdComponent>();
            _ownerIdPool = _world.GetPool<OwnerIdComponent>();
            _moneyUpdateRequestPool = _world.GetPool<MoneyUpdateRequestComponent>();
        }

        public void PostRun(IEcsSystems systems)
        {
            foreach (int business in _businesses)
            {
                bool cooldownUp = _cooldownUpPool.Get(business).Value;

                if (!cooldownUp)
                    continue;

                int businessId = _businessIdPool.Get(business).Value;

                int ownerId = _ownerIdPool.Get(business).Value;

                int totalIncome = _businessService.GetTotalIncome(businessId);
                
                int updateRequest = _world.NewEntity();
                
                _moneyUpdateRequestPool.Add(updateRequest)
                    .Value = new MoneyUpdateRequest(ownerId,totalIncome);
                
                Debug.Log($"@@@: income {totalIncome}");
            }
        }
    }
}