using System;
using Code.Gameplay.Business;
using UniRx;

namespace Code.UI
{
    public class UpgradeBusinessScreenModel : IUIModel
    {
        private readonly BusinessService _businessService;
        private readonly ReactiveProperty<bool> _purchased;
        private readonly ReactiveProperty<float> _incomeMultiplier;
        private readonly ReactiveProperty<int> _price;
        private readonly ReactiveProperty<string> _name;
        private readonly ReactiveProperty<int> _id;
        private readonly ReactiveProperty<int> _businessId;
        private IDisposable _modifierSubscription;

        public IReadOnlyReactiveProperty<bool> Purchased => _purchased;
        public IReadOnlyReactiveProperty<float> IncomeMultiplier => _incomeMultiplier;
        public IReadOnlyReactiveProperty<int> Price => _price;
        public IReadOnlyReactiveProperty<string> Name => _name;
        public IReadOnlyReactiveProperty<bool> PurchaseAvailable => _businessService.GetPurchasedProperty(_businessId.Value);
        
        public UpgradeBusinessScreenModel(bool purchased, float income, int price, string name, int id, int businessId, BusinessService businessService)
        {
            _businessService = businessService;
            _purchased = new ReactiveProperty<bool>(purchased);
            _incomeMultiplier = new ReactiveProperty<float>(income);
            _price = new ReactiveProperty<int>(price);
            _name = new ReactiveProperty<string>(name);
            _id = new ReactiveProperty<int>(id);
            _businessId = new ReactiveProperty<int>(businessId);
        }

        public void Initialize()
        {
            _modifierSubscription = _businessService.BusinessModifierStateChanged
                .Where(modifier => modifier.Item1 == _businessId.Value && modifier.Item2 == _id.Value)
                .Subscribe(modifer => _purchased.Value = modifer.Item3);
        }

        public void OnUpgradeClicked()
        {
            if (_businessService.TryPurchaseUpgrade(_businessId.Value, _id.Value, _price.Value))
            {
                _purchased.Value = true;
            }
        }

        public void Dispose()
        {
            _modifierSubscription?.Dispose();
            _purchased?.Dispose();
            _incomeMultiplier?.Dispose();
            _price?.Dispose();
            _name?.Dispose();
            _id?.Dispose();
            _businessId?.Dispose();
        }
    }
}