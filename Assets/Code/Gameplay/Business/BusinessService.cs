using System.Collections.Generic;
using Code.Common;
using Code.Gameplay.Money;
using Leopotam.EcsLite;
using UniRx;

namespace Code.Gameplay.Business
{
    public class BusinessService : IInitializable
    {
        private readonly BusinessProperties _properties;
        private readonly BusinessRequestCreator _requestCreator;

        public BusinessService(EcsWorld ecsWorld, IMoneyService moneyService)
        {
            _properties = new BusinessProperties();
            _requestCreator = new BusinessRequestCreator(ecsWorld, moneyService);
        }

        public void Initialize()
        {
            _requestCreator.Initialize();
        }

        public IReadOnlyReactiveProperty<int> GetLevelProperty(int id) => _properties.GetLevelProperty(id);
        public IReadOnlyReactiveProperty<string> GetNameProperty(int id) => _properties.GetNameProperty(id);
        public IReadOnlyReactiveProperty<float> GetProgressProperty(int id) => _properties.GetProgressProperty(id);
        public IReadOnlyReactiveProperty<int> GetIncomeProperty(int id) => _properties.GetIncomeProperty(id);
        public IReadOnlyReactiveProperty<int> GetLevelUpPriceProperty(int id) => _properties.GetLevelUpPriceProperty(id);
        public IReadOnlyReactiveProperty<bool> GetPurchasedProperty(int id) => _properties.GetPurchasedProperty(id);
        public IReadOnlyReactiveProperty<bool> GetUpgradeProperty(int businessId, int upgradeId) => _properties.GetUpgradeProperty(businessId, upgradeId);

        public bool TryPurchaseLevelUp(int id, int levelPrice, int level)
        {
            return _requestCreator.TryPurchaseLevelUp(id, levelPrice, level);
        }

        public void UpdateBusinessProgress(int id, float progress)
        {
            _properties.UpdateProgress(id, progress);
        }

        public void NotifyBusinessDataUpdated(int id, int level, int income, int levelUpPrice, string name, Dictionary<int, bool> upgrades = null)
        {
            _properties.UpdateBusinessData(id, level, income, levelUpPrice, name, upgrades);
        }

        public bool TryPurchaseUpgrade(int businessId, int upgradeId, int price)
        {
            return _requestCreator.TryPurchaseUpgrade(businessId, upgradeId, price);
        }
    }
}