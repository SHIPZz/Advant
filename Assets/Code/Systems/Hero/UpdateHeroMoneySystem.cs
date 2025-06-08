using Code.Components;
using Code.Gameplay.Money;
using Leopotam.EcsLite;

namespace Code.Systems.Hero
{
    public class UpdateHeroMoneySystem : IEcsInitSystem, IEcsPostRunSystem
    {
        private readonly IMoneyService _heroMoneyService;

        private EcsWorld _world;
        private EcsFilter _hero;
        private EcsPool<MoneyComponent> _balancePool;

        public UpdateHeroMoneySystem(IMoneyService heroMoneyService)
        {
            _heroMoneyService = heroMoneyService;
        }

        public void Init(IEcsSystems systems)
        {
            _world = systems.GetWorld();

            _hero = _world.Filter<MoneyComponent>()
                .Inc<IdComponent>()
                .Inc<HeroComponent>()
                .End();

            _balancePool = _world.GetPool<MoneyComponent>();
        }

        public void PostRun(IEcsSystems systems)
        {
            foreach (int hero in _hero)
            {
                int targetBalance = _balancePool.Get(hero).Value;
                 
                _heroMoneyService.Set(targetBalance);
            }
        }
    }
}