using Code.Gameplay.Business.Components;
using Leopotam.EcsLite;

namespace Code.Gameplay.Business.Systems
{
    public class CleanupBusinessRequestsSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld _world;
        private EcsFilter _updateRequest;
        private EcsPool<UpdateBusinessRequestComponent> _businessUpdateRequestPool;

        public void Init(IEcsSystems systems)
        {
            _world = systems.GetWorld();
            _updateRequest = _world.Filter<UpdateBusinessRequestComponent>().End();
            _businessUpdateRequestPool = _world.GetPool<UpdateBusinessRequestComponent>();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (int updateRequest in _updateRequest)
            {
                _businessUpdateRequestPool.Del(updateRequest);
            }
        }
    }
}