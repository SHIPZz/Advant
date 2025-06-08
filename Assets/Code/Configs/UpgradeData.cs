using System;

namespace Code.Configs
{
    [Serializable]
    public class UpgradeData
    {
        public int Id;
        public int Cost;
        public float IncomeMultiplier;
        public bool Purchased;
    }
}