using System;

namespace Code.Gameplay.Business.Configs
{
    [Serializable]
    public class BusinessData
    {
        public float IncomeDelay;
        public int BaseCost;
        public int BaseIncome;
        public UpgradeData[] Upgrades;
    }
}