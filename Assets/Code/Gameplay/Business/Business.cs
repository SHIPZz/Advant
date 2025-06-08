using System.Collections.Generic;
using Code.Configs;
using UniRx;

namespace Code.Gameplay.Business
{
    public class Business
    {
        public int Id { get; }
        public ReactiveProperty<int> Level { get; }
        public ReactiveProperty<string> Name { get; }
        public ReactiveProperty<float> Progress { get; }
        public ReactiveProperty<int> Income { get; }

        public ReactiveProperty<int> BaseIncome { get; }

        public ReactiveProperty<int> LevelUpPrice { get; }

        public ReactiveCollection<string> UpgradeNames { get; }
        
        public ReactiveCollection<UpgradeData> Upgrades { get; }

        public Business(int id)
        {
            Level = new ReactiveProperty<int>();
            Name = new ReactiveProperty<string>();
            Progress = new ReactiveProperty<float>();
            Income = new ReactiveProperty<int>();
            BaseIncome = new ReactiveProperty<int>();
            LevelUpPrice = new ReactiveProperty<int>();
            Id = id;
            UpgradeNames = new ReactiveCollection<string>();
            Upgrades = new ReactiveCollection<UpgradeData>();
        }

        public void SetUpgradeNames(IEnumerable<string> upgradeNames)
        {
            foreach (var name in upgradeNames)
                UpgradeNames.Add(name);
        }
    }
}