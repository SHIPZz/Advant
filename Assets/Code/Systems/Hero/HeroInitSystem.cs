using Code.Common.Services;
using Code.Components;
using Leopotam.EcsLite;

namespace Code.Systems.Hero
{
    public class HeroInitSystem : IEcsInitSystem
    {
        private readonly IIdentifierService _identifierService;
        
        public HeroInitSystem(IIdentifierService identifierService)
        {
            _identifierService = identifierService;
        }

        public void Init(IEcsSystems systems)
        {
           var world = systems.GetWorld();
            
            int hero = world.NewEntity();

            world.GetPool<HeroComponent>()
                .Add(hero)
                .Value = true;

            world.GetPool<MoneyComponent>()
                .Add(hero)
                .Value = 10000;

            world.GetPool<IdComponent>()
                .Add(hero)
                .Value = _identifierService.Next();
        }
    }
}