using System.Collections.Generic;
using Code.Gameplay.Business.Configs;
using UniRx;
using UnityEngine;

namespace Code.Gameplay.Business
{
    public class BusinessProperties
    {
        private readonly Dictionary<int, ReactiveProperty<int>> _levelProperties = new();
        private readonly Dictionary<int, ReactiveProperty<string>> _nameProperties = new();
        private readonly Dictionary<int, ReactiveProperty<float>> _progressProperties = new();
        private readonly Dictionary<int, ReactiveProperty<int>> _incomeProperties = new();
        private readonly Dictionary<int, ReactiveProperty<int>> _levelUpPriceProperties = new();
        private readonly Dictionary<int, ReactiveProperty<bool>> _purchasedProperties = new();
        private readonly ReactiveProperty<(int, int, bool)> _businessModifierStateChanged = new();
        
        public IReadOnlyReactiveProperty<(int, int, bool)> BusinessModifierStateChanged => _businessModifierStateChanged;

        public IReadOnlyReactiveProperty<int> GetLevelProperty(int id)
        {
            if (!_levelProperties.ContainsKey(id))
                _levelProperties[id] = new ReactiveProperty<int>();
            
            return _levelProperties[id];
        }

        public IReadOnlyReactiveProperty<string> GetNameProperty(int id)
        {
            if (!_nameProperties.ContainsKey(id))
                _nameProperties[id] = new ReactiveProperty<string>();
            
            return _nameProperties[id];
        }

        public IReadOnlyReactiveProperty<float> GetProgressProperty(int id)
        {
            if (!_progressProperties.ContainsKey(id))
                _progressProperties[id] = new ReactiveProperty<float>();
            
            return _progressProperties[id];
        }

        public IReadOnlyReactiveProperty<int> GetIncomeProperty(int id)
        {
            if (!_incomeProperties.ContainsKey(id))
                _incomeProperties[id] = new ReactiveProperty<int>();
            
            return _incomeProperties[id];
        }

        public IReadOnlyReactiveProperty<int> GetLevelUpPriceProperty(int id)
        {
            if (!_levelUpPriceProperties.ContainsKey(id))
                _levelUpPriceProperties[id] = new ReactiveProperty<int>();
            
            return _levelUpPriceProperties[id];
        }

        public IReadOnlyReactiveProperty<bool> GetPurchasedProperty(int id)
        {
            if (!_purchasedProperties.ContainsKey(id))
                _purchasedProperties[id] = new ReactiveProperty<bool>();
            
            return _purchasedProperties[id];
        }

        public void UpdateBusinessData(int id, int level, int income, int levelUpPrice, string name,
            List<AccumulatedModifiersData> upgrades = null)
        {
            if (level > -1)
            {
                if (!_levelProperties.ContainsKey(id))
                    _levelProperties[id] = new ReactiveProperty<int>();
                
                _levelProperties[id].Value = level;
            }

            if (income > -1)
            {
                if (!_incomeProperties.ContainsKey(id))
                    _incomeProperties[id] = new ReactiveProperty<int>();
                
                _incomeProperties[id].Value = income;
            }

            if (levelUpPrice > -1)
            {
                if (!_levelUpPriceProperties.ContainsKey(id))
                    _levelUpPriceProperties[id] = new ReactiveProperty<int>();
                
                _levelUpPriceProperties[id].Value = levelUpPrice;
            }

            if (!string.IsNullOrEmpty(name))
            {
                if (!_nameProperties.ContainsKey(id))
                    _nameProperties[id] = new ReactiveProperty<string>();
                
                _nameProperties[id].Value = name;
            }

            if (upgrades != null)
            {
                foreach (var upgrade in upgrades)
                {
                    _businessModifierStateChanged.Value = (id, upgrade.Id, upgrade.Purchased);
                }
            }

            if (!_purchasedProperties.ContainsKey(id))
                _purchasedProperties[id] = new ReactiveProperty<bool>();
            
            _purchasedProperties[id].Value = level > 0;
        }

        public void UpdateProgress(int id, float progress)
        {
            if (!_progressProperties.ContainsKey(id))
                _progressProperties[id] = new ReactiveProperty<float>();
            _progressProperties[id].Value = progress;
        }
    }
}