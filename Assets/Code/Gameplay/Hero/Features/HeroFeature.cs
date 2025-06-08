using Code.Common.Features;
using Code.Common.Services;
using Code.Gameplay.Hero.Systems;
using Leopotam.EcsLite;

namespace Code.Gameplay.Hero.Features
{
    public class HeroFeature : Feature
    {
        private readonly IIdentifierService _identifierService;

        public HeroFeature(EcsWorld world, IEcsSystems systems, IIdentifierService identifierService) 
            : base(world, systems)
        {
            _identifierService = identifierService;
        }

        public override void RegisterSystems()
        {
            Systems
                .Add(new HeroInitSystem(_identifierService));
        }
    }
} 