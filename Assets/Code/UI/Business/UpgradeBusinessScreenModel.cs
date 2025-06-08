using Code.Gameplay.Business;
using UniRx;

namespace Code.UI.Business
{
    public struct UpgradeBusinessScreenModel
    {
        public readonly ReactiveProperty<bool> Purchased;
        public readonly ReactiveProperty<float> IncomeMultiplier;
        public readonly ReactiveProperty<int> Price;
        public readonly ReactiveProperty<string> Name;
        public readonly ReactiveProperty<int> Id;
        public readonly ReactiveProperty<int> BusinessId;
        private readonly BusinessService _businessService;

        public UpgradeBusinessScreenModel(bool purchased, float income, int price, string name, int id, int businessId,
            BusinessService businessService)
        {
            _businessService = businessService;
            Purchased = new ReactiveProperty<bool>(purchased);
            IncomeMultiplier = new ReactiveProperty<float>(income);
            Price = new ReactiveProperty<int>(price);
            Name = new ReactiveProperty<string>(name);
            Id = new ReactiveProperty<int>(id);
            BusinessId = new ReactiveProperty<int>(businessId);
        }

        public void OnUpgradeClicked()
        {
            Purchased.Value = _businessService.TryPurchaseUpgrade(BusinessId.Value, Id.Value, Price.Value);
        }
    }
}