using Code.Common.Components;
using Code.Gameplay.Business.Components;
using Leopotam.EcsLite;

namespace Code.Gameplay.Business.Systems
{
    public class CalculateBusinessProgressSystem : IEcsRunSystem, IEcsInitSystem
    {
        private readonly BusinessService _businessService;

        private EcsFilter _businesses;
        private EcsWorld _world;
        private EcsPool<IncomeСooldownComponent> _cooldownPool;
        private EcsPool<IncomeСooldownLeftComponent> _cooldownLeftPool;
        private EcsPool<BusinessIdComponent> _businessIdPool;
        private EcsPool<ProgressComponent> _progressPool;

        public CalculateBusinessProgressSystem(BusinessService businessService)
        {
            _businessService = businessService;
        }

        public void Init(IEcsSystems systems)
        {
            _world = systems.GetWorld();

            _businesses = _world
                .Filter<IncomeComponent>()
                .Inc<IncomeСooldownComponent>()
                .Inc<BusinessIdComponent>()
                .Inc<BusinessComponent>()
                .Inc<ProgressComponent>()
                .Inc<PurchasedComponent>()
                .Inc<IncomeСooldownLeftComponent>()
                .Inc<IdComponent>()
                .End();

            _cooldownPool = _world.GetPool<IncomeСooldownComponent>();
            _cooldownLeftPool = _world.GetPool<IncomeСooldownLeftComponent>();
            _businessIdPool = _world.GetPool<BusinessIdComponent>();
            _progressPool = _world.GetPool<ProgressComponent>();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (int business in _businesses)
            {
                float totalCooldown = _cooldownPool.Get(business).Value;
                float currentCooldown = _cooldownLeftPool.Get(business).Value;
                int businessId = _businessIdPool.Get(business).Value;

                float progress = 1f - (currentCooldown / totalCooldown);

                _progressPool.Get(business).Value = progress;

                _businessService.UpdateBusinessProgress(businessId, progress);
            }
        }
    }
}