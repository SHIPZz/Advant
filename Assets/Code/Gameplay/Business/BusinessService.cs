using System.Collections.Generic;
using Code.Common;
using Code.Components;
using Code.Configs;
using Code.Gameplay.Money;
using Code.Requests;
using Code.Utils;
using Leopotam.EcsLite;
using UniRx;
using UnityEngine;

namespace Code.Gameplay.Business
{
    public class BusinessService : IInitializable
    {
        private readonly Dictionary<int, Business> _businesses = new();

        private readonly BusinessConfig _businessConfig;
        private readonly BusinessUpgradeNamesConfig _businessUpgradeNamesConfig;
        private readonly IMoneyService _moneyService;
        private readonly EcsWorld _ecsWorld;
        private EcsPool<UpdateBusinessRequestComponent> _updateRequestPool;

        public BusinessService(BusinessConfig businessConfig,
            EcsWorld ecsWorld,
            BusinessUpgradeNamesConfig businessUpgradeNamesConfig,
            IMoneyService moneyService)
        {
            _ecsWorld = ecsWorld;
            _moneyService = moneyService;
            _businessUpgradeNamesConfig = businessUpgradeNamesConfig;
            _businessConfig = businessConfig;
        }

        public ReactiveProperty<int> GetLevelProperty(int id) => GetBusiness(id).Level;

        public ReactiveProperty<string> GetNameProperty(int id) => GetBusiness(id).Name;

        public ReactiveProperty<float> GetProgressProperty(int id) => GetBusiness(id).Progress;

        public ReactiveProperty<int> GetIncomeProperty(int id) => GetBusiness(id).Income;

        public ReactiveProperty<int> GetLevelUpPriceProperty(int id) => GetBusiness(id).LevelUpPrice;

        public void Initialize()
        {
            _updateRequestPool = _ecsWorld.GetPool<UpdateBusinessRequestComponent>();
        }

        public bool TryUpdateBusinessLevel(int id)
        {
            Business business = GetBusiness(id);
            
            int levelUpPrice = business.LevelUpPrice.Value;

            if (_moneyService.TryPurchase(levelUpPrice))
            {
                business.Level.Value += 1;
                business.Income.Value = GetTotalIncome(id);
                UpdateLevelUpPrice(business);

                int updateRequest = _ecsWorld.NewEntity();

                 _updateRequestPool.Add(updateRequest)
                     .Value = new UpdateBusinessRequest(business.Level.Value, business.LevelUpPrice.Value, business.Income.Value,id);
                
                return true;
            }

            return false;
        }

        public void UpdateBusinessProgress(int id, float progress)
        {
            GetBusiness(id).Progress.Value = progress;
        }

        public bool TryPurchaseUpgrade(int id, int upgradeId, int price)
        {
            Business business = GetBusiness(id);

            if (!_moneyService.TryPurchase(price))
                return false;

            business.Upgrades[upgradeId].Purchased = true;
            business.Income.Value = GetTotalIncome(id);
            return true;
        }

        public int GetTotalIncome(int businessId)
        {
            Business business = GetBusiness(businessId);

            int level = business.Level.Value;

            UpgradeData firstUpgrade = business.Upgrades[0];
            UpgradeData secondUpgrade = business.Upgrades[1];
            
            float firstModifier = firstUpgrade.Purchased ? firstUpgrade.IncomeMultiplier : 0;
            float secondModifier = secondUpgrade.Purchased ? secondUpgrade.IncomeMultiplier : 0;

            float totalIncome = BusinessCalculator.CalculateIncome(
                level,
                business.BaseIncome.Value,
                firstModifier,
                secondModifier
            );

            Debug.Log($"{totalIncome}");
            
            return Mathf.RoundToInt(totalIncome);
        }

        private Business GetBusiness(int id)
        {
            if (!_businesses.ContainsKey(id))
            {
                CreateBusiness(id);
            }

            return _businesses[id];
        }

        private Business CreateBusiness(int id)
        {
            Business business = new Business(id);

            IReadOnlyList<BusinessData> datas = _businessConfig.GetBusinessDatas();
            BusinessData businessData = datas[id];

            if (id == 0)
                business.Level.Value = 1;

            BusinessUpgradeNameData businessUpgradeNameData = _businessUpgradeNamesConfig.BusinessUpgradeNameDatas[id];

            business.Name.Value = businessUpgradeNameData.Name;
            business.BaseIncome.Value = businessData.BaseIncome;
            business.Income.Value = businessData.BaseIncome;

            foreach (UpgradeData upgradeData in businessData.Upgrades)
                business.Upgrades.Add(upgradeData);

            UpdateLevelUpPrice(business);

            business.SetUpgradeNames(businessUpgradeNameData.UpgradeNames);

            return _businesses[id] = business;
        }

        private static void UpdateLevelUpPrice(Business business)
        {
            business.LevelUpPrice.Value = BusinessCalculator.CalculateLevelUpPrice(business.Level.Value, business.BaseIncome.Value);
        }
    }
}