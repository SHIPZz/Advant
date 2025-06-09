using System.Collections.Generic;
using Code.Gameplay.Business;
using UniRx;

namespace Code.UI
{
    public class BusinessScreenModel : IUIModel
    {
        private readonly BusinessService _businessService;
        private readonly int _businessId;
        private readonly ReactiveCollection<UpgradeBusinessScreenModel> _upgradeBusinessScreenModels;

        public BusinessScreenModel(BusinessService businessService, int businessId, List<UpgradeBusinessScreenModel> upgradeBusinessScreenModels)
        {
            _businessService = businessService;
            _businessId = businessId;
            _upgradeBusinessScreenModels = new ReactiveCollection<UpgradeBusinessScreenModel>(upgradeBusinessScreenModels);
        }

        public IReadOnlyReactiveProperty<int> Level => _businessService.GetLevelProperty(_businessId);
        public IReadOnlyReactiveProperty<string> Name => _businessService.GetNameProperty(_businessId);
        public IReadOnlyReactiveProperty<float> Progress => _businessService.GetProgressProperty(_businessId);
        public IReadOnlyReactiveProperty<int> Income => _businessService.GetIncomeProperty(_businessId);
        public IReadOnlyReactiveProperty<int> LevelUpPrice => _businessService.GetLevelUpPriceProperty(_businessId);
        
        public IReadOnlyReactiveCollection<UpgradeBusinessScreenModel> UpgradeBusinessScreenModels => _upgradeBusinessScreenModels;
        
        public void OnLevelUpButtonClicked()
        {
            _businessService.TryPurchaseLevelUp(_businessId, LevelUpPrice.Value,1);
        }

        public void Dispose()
        {
            _upgradeBusinessScreenModels?.Dispose();
        }
    }
}