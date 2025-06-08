using Code.Components;
using Code.Requests;
using Leopotam.EcsLite;

namespace Code.Gameplay.Money
{
    public class HeroMoneyService : IMoneyService
    {
        private readonly CurrencyModel _currencyModel;
        private readonly EcsWorld _ecsWorld;
        private EcsPool<MoneyUpdateRequestComponent> _moneyUpdateRequestPool;
        private EcsFilter _heroFilter;
        private EcsPool<IdComponent> _idPool;
        private EcsPool<MoneyComponent> _moneyPool;

        public HeroMoneyService(CurrencyModel currencyModel, EcsWorld ecsWorld)
        {
            _ecsWorld = ecsWorld;
            _currencyModel = currencyModel;
        }

        public void Initialize()
        {
            _heroFilter = _ecsWorld
                .Filter<HeroComponent>()
                .Inc<IdComponent>()
                .Inc<MoneyComponent>()
                .End();

            _moneyUpdateRequestPool = _ecsWorld.GetPool<MoneyUpdateRequestComponent>();
            _moneyPool = _ecsWorld.GetPool<MoneyComponent>();
            _idPool = _ecsWorld.GetPool<IdComponent>();
        }

        public void Set(int money)
        {
            _currencyModel.Money.Value = money;
        }

        public bool TryPurchase(int cost)
        {
            foreach (int hero in _heroFilter)
            {
                int heroId = _idPool.Get(hero).Value;
                int money = _moneyPool.Get(hero).Value;

                if (money < cost)
                    return false;

                CreateUpdateMoneyRequest(-cost, heroId);
            }

            return true;
        }

        private void CreateUpdateMoneyRequest(int cost, int heroId)
        {
            var request = _ecsWorld.NewEntity();

            _moneyUpdateRequestPool
                .Add(request)
                .Value = new MoneyUpdateRequest(heroId, cost);
        }
    }
}