using Code.Gameplay.Business.Configs;

namespace Code.Gameplay.Requests
{
    public readonly struct UpdateBusinessRequest
    {
        public readonly int Level;
        public readonly int Id;
        public readonly int LevelUpPrice;
        public readonly int Income;
        public readonly UpdateModifierData UpdateModifierData;
        
        public UpdateBusinessRequest(int level, int levelUpPrice, int income, int id, UpdateModifierData updateModifierData)
        {
            Level = level;
            LevelUpPrice = levelUpPrice;
            Income = income;
            Id = id;
            UpdateModifierData = updateModifierData;
        }
    }
}