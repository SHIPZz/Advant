using Code.Gameplay.Business.Components;
using Code.Gameplay.Business.Configs;
using Code.Gameplay.Money;
using Code.Gameplay.Requests;
using Leopotam.EcsLite;

namespace Code.Gameplay.Business
{
    public class BusinessRequestCreator
    {
        private readonly EcsWorld _ecsWorld;
        private readonly IMoneyService _moneyService;
        private EcsPool<UpdateBusinessRequestComponent> _updateRequestPool;

        public BusinessRequestCreator(EcsWorld ecsWorld, IMoneyService moneyService)
        {
            _ecsWorld = ecsWorld;
            _moneyService = moneyService;
        }

        public void Initialize()
        {
            _updateRequestPool = _ecsWorld.GetPool<UpdateBusinessRequestComponent>();
        }

        public bool TryPurchaseLevelUp(int id, int levelPrice, int level)
        {
            if(!_moneyService.TryPurchase(levelPrice))
                return false;
            
            CreateUpdateRequest(level, levelPrice, -1, id, new UpdateModifierData(-1));
            return true;
        }

        public bool TryPurchaseUpgrade(int businessId, int upgradeId, int price)
        {
            if (!_moneyService.TryPurchase(price))
                return false;

            CreateUpdateRequest(-1, -1, -1, businessId, new UpdateModifierData(upgradeId));
            return true;
        }

        private void CreateUpdateRequest(int level, int levelPrice, int income, int businessId, UpdateModifierData modifierData)
        {
            int updateRequest = _ecsWorld.NewEntity();
            _updateRequestPool.Add(updateRequest).Value = new UpdateBusinessRequest(level, levelPrice, income, businessId, modifierData);
        }
    }
} 