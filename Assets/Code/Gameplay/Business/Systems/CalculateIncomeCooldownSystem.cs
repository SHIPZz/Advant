using Code.Common.Components;
using Code.Common.Services;
using Code.Gameplay.Business.Components;
using Leopotam.EcsLite;

namespace Code.Gameplay.Business.Systems
{
    public class CalculateIncomeCooldownSystem : IEcsRunSystem, IEcsInitSystem
    {
        private readonly ITimeService _timeService;
        
        private EcsFilter _incomeCooldowns;
        private EcsWorld _world;
        private EcsPool<IncomeСooldownLeftComponent> _cooldownLeftPool;
        private EcsPool<IncomeСooldownUpComponent> _cooldownUpPool;
        private EcsPool<IncomeСooldownComponent> _incomeCooldownPool;
        
        public CalculateIncomeCooldownSystem(ITimeService timeService)
        {
            _timeService = timeService;
        }

        public void Init(IEcsSystems systems)
        {
            _world = systems.GetWorld();

            _incomeCooldowns = _world
                .Filter<IncomeComponent>()
                .Inc<IncomeСooldownComponent>()
                .Inc<IncomeСooldownAvailableComponent>()
                .Inc<IncomeСooldownLeftComponent>()
                .Inc<IdComponent>()
                .End();

            _cooldownLeftPool = _world.GetPool<IncomeСooldownLeftComponent>();
            _incomeCooldownPool = _world.GetPool<IncomeСooldownComponent>();
            _cooldownUpPool = _world.GetPool<IncomeСooldownUpComponent>();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (int incomeCooldown in _incomeCooldowns)
            {
               ref var currentCooldown = ref _cooldownLeftPool.Get(incomeCooldown).Value;
               ref var cooldownUp = ref _cooldownUpPool.Get(incomeCooldown).Value;

                currentCooldown -= _timeService.DeltaTime; 

                if (currentCooldown <= 0)
                {
                    if (!cooldownUp)
                        cooldownUp = true;
                    
                    currentCooldown = _incomeCooldownPool.Get(incomeCooldown).Value;
                }
                else
                {
                    if (cooldownUp)
                        cooldownUp = false;
                }
            }
        }
    }
}
