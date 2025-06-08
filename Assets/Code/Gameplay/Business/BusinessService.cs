using System.Collections.Generic;
using Code.Common;
using Code.Gameplay.Business.Components;
using Code.Gameplay.Business.Configs;
using Code.Gameplay.Money;
using Code.Gameplay.Requests;
using Code.Utils;
using Leopotam.EcsLite;
using UniRx;

namespace Code.Gameplay.Business
{
    public class BusinessService : IInitializable
    {
        private readonly IMoneyService _moneyService;
        private readonly EcsWorld _ecsWorld;
        private EcsPool<UpdateBusinessRequestComponent> _updateRequestPool;

        private readonly Dictionary<int, ReactiveProperty<int>> _levelProperties = new();
        private readonly Dictionary<int, ReactiveProperty<string>> _nameProperties = new();
        private readonly Dictionary<int, ReactiveProperty<float>> _progressProperties = new();
        private readonly Dictionary<int, ReactiveProperty<int>> _incomeProperties = new();
        private readonly Dictionary<int, ReactiveProperty<int>> _levelUpPriceProperties = new();
        private readonly Dictionary<int, ReactiveProperty<bool>> _purchasedProperties = new();

        public BusinessService(EcsWorld ecsWorld, IMoneyService moneyService)
        {
            _ecsWorld = ecsWorld;
            _moneyService = moneyService;
        }

        public ReactiveProperty<int> GetLevelProperty(int id) => GetOrCreateProperty(_levelProperties, id);
        public ReactiveProperty<string> GetNameProperty(int id) => GetOrCreateProperty(_nameProperties, id);
        public ReactiveProperty<float> GetProgressProperty(int id) => GetOrCreateProperty(_progressProperties, id);
        public ReactiveProperty<int> GetIncomeProperty(int id) => GetOrCreateProperty(_incomeProperties, id);

        public ReactiveProperty<int> GetLevelUpPriceProperty(int id) =>
            GetOrCreateProperty(_levelUpPriceProperties, id);

        public ReactiveProperty<bool> GetPurchasedProperty(int id) => GetOrCreateProperty(_purchasedProperties, id);

        public void Initialize()
        {
            _updateRequestPool = _ecsWorld.GetPool<UpdateBusinessRequestComponent>();
        }

        public bool TryPurchaseLevelUp(int id, int levelPrice, int level)
        {
            if(!_moneyService.TryPurchase(levelPrice))
                return false;
            
            int updateRequest = _ecsWorld.NewEntity();

            _updateRequestPool.Add(updateRequest).Value =
                new UpdateBusinessRequest(level, levelPrice, -1, id, new UpdateModifierData(-1));

            return true;
        }

        public void UpdateBusinessProgress(int id, float progress)
        {
            GetOrCreateProperty(_progressProperties, id).Value = progress;
        }

        public void NotifyBusinessDataUpdated(int id, int level, int income, int levelUpPrice, string name)
        {
            if (level > -1)
                GetOrCreateProperty(_levelProperties, id).Value = level;

            if (level > 0)
                GetOrCreateProperty(_purchasedProperties, id).Value = true;

            if (income > -1)
                GetOrCreateProperty(_incomeProperties, id).Value = income;

            if (levelUpPrice > -1)
                GetOrCreateProperty(_levelUpPriceProperties, id).Value = levelUpPrice;

            if (!string.IsNullOrEmpty(name))
                GetOrCreateProperty(_nameProperties, id).Value = name;
        }

        public bool TryPurchaseUpgrade(int businessId, int upgradeId, int price)
        {
            if (!_moneyService.TryPurchase(price))
                return false;

            int updateRequest = _ecsWorld.NewEntity();

            _updateRequestPool.Add(updateRequest).Value =
                new UpdateBusinessRequest(-1, -1, -1, businessId, new UpdateModifierData(upgradeId));

            return true;
        }

        private ReactiveProperty<T> GetOrCreateProperty<T>(Dictionary<int, ReactiveProperty<T>> properties, int id)
        {
            if (!properties.ContainsKey(id))
            {
                properties[id] = new ReactiveProperty<T>();
            }

            return properties[id];
        }
    }
}