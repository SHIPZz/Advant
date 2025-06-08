using System.Collections.Generic;
using Code.Components;
using Code.Configs;
using Code.Gameplay.Business;
using Code.Requests;
using Code.Utils;
using Leopotam.EcsLite;
using UnityEngine;

namespace Code.Systems.Business
{
    public class CreateMoneyUpdateRequestOnIncomeCooldownUpSystem : IEcsPostRunSystem, IEcsInitSystem
    {
        private EcsWorld _world;

        private EcsFilter _businesses;
        private EcsPool<IncomeСooldownUpComponent> _cooldownUpPool;
        private EcsPool<MoneyUpdateRequestComponent> _moneyUpdateRequestPool;
        private EcsPool<OwnerIdComponent> _ownerIdPool;
        private EcsPool<LevelComponent> _levelPool;
        private EcsPool<UpdateBusinessModifiersComponent> _updateBusinessModifiersPool;
        private EcsPool<BaseIncomeComponent> _baseIncomePool;


        public void Init(IEcsSystems systems)
        {
            _world = systems.GetWorld();

            _businesses = _world.Filter<IncomeСooldownLeftComponent>()
                .Inc<IncomeСooldownComponent>()
                .Inc<IncomeСooldownUpComponent>()
                .Inc<PurchasedComponent>()
                .Inc<BusinessComponent>()
                .Inc<BaseIncomeComponent>()
                .Inc<UpdateBusinessModifiersComponent>()
                .Inc<OwnerIdComponent>()
                .Inc<IdComponent>()
                .End();

            _cooldownUpPool = _world.GetPool<IncomeСooldownUpComponent>();
            _updateBusinessModifiersPool = _world.GetPool<UpdateBusinessModifiersComponent>();
            _levelPool = _world.GetPool<LevelComponent>();
            _baseIncomePool = _world.GetPool<BaseIncomeComponent>();
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

                int ownerId = _ownerIdPool.Get(business).Value;

                int level = _levelPool.Get(business).Value;
                
                int baseIncome = _baseIncomePool.Get(business).Value;

               List<UpgradeData> modifiers =  _updateBusinessModifiersPool.Get(business).Value;

               float firstModifier = modifiers[0].Purchased ? modifiers[0].IncomeMultiplier : 0f;
               float secondModifier = modifiers[1].Purchased ? modifiers[1].IncomeMultiplier : 0f;
               
               int totalIncome = Mathf.RoundToInt(BusinessCalculator.CalculateIncome(
                   level,
                   baseIncome,
                   firstModifier,
                   secondModifier));
                
                int updateRequest = _world.NewEntity();
                
                _moneyUpdateRequestPool.Add(updateRequest)
                    .Value = new MoneyUpdateRequest(ownerId,totalIncome);
                
                Debug.Log($"@@@: income {totalIncome}");
            }
        }
    }
}