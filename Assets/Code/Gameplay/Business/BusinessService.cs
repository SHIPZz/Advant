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

        public ReactiveProperty<int> GetLevelProperty(int id) => _properties.GetLevelProperty(id);
        public ReactiveProperty<string> GetNameProperty(int id) => _properties.GetNameProperty(id);
        public ReactiveProperty<float> GetProgressProperty(int id) => _properties.GetProgressProperty(id);
        public ReactiveProperty<int> GetIncomeProperty(int id) => _properties.GetIncomeProperty(id);
        public ReactiveProperty<int> GetLevelUpPriceProperty(int id) => _properties.GetLevelUpPriceProperty(id);
        public ReactiveProperty<bool> GetPurchasedProperty(int id) => _properties.GetPurchasedProperty(id);

        public bool TryPurchaseLevelUp(int id, int levelPrice, int level)
        {
            return _requestCreator.TryPurchaseLevelUp(id, levelPrice, level);
        }

        public void UpdateBusinessProgress(int id, float progress)
        {
            _properties.UpdateProgress(id, progress);
        }

        public void NotifyBusinessDataUpdated(int id, int level, int income, int levelUpPrice, string name)
        {
            _properties.UpdateBusinessData(id, level, income, levelUpPrice, name);
        }

        public bool TryPurchaseUpgrade(int businessId, int upgradeId, int price)
        {
            return _requestCreator.TryPurchaseUpgrade(businessId, upgradeId, price);
        }
    }
}