namespace Code.Requests
{
    public readonly struct UpdateBusinessRequest
    {
        public readonly int Level;
        public readonly int Id;
        public readonly int LevelUpPrice;
        public readonly int Income;
        
        public UpdateBusinessRequest(int level, int levelUpPrice, int income, int id)
        {
            Level = level;
            LevelUpPrice = levelUpPrice;
            Income = income;
            Id = id;
        }
    }
}