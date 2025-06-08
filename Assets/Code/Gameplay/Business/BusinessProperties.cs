using System.Collections.Generic;
using Code.Utils;
using UniRx;

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

        public ReactiveProperty<int> GetLevelProperty(int id) => GetOrCreateProperty(_levelProperties, id);
        public ReactiveProperty<string> GetNameProperty(int id) => GetOrCreateProperty(_nameProperties, id);
        public ReactiveProperty<float> GetProgressProperty(int id) => GetOrCreateProperty(_progressProperties, id);
        public ReactiveProperty<int> GetIncomeProperty(int id) => GetOrCreateProperty(_incomeProperties, id);
        public ReactiveProperty<int> GetLevelUpPriceProperty(int id) => GetOrCreateProperty(_levelUpPriceProperties, id);
        public ReactiveProperty<bool> GetPurchasedProperty(int id) => GetOrCreateProperty(_purchasedProperties, id);

        public void UpdateBusinessData(int id, int level, int income, int levelUpPrice, string name)
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

        public void UpdateProgress(int id, float progress)
        {
            GetOrCreateProperty(_progressProperties, id).Value = progress;
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