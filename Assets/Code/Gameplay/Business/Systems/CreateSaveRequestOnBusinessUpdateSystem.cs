using Code.Gameplay.Business.Components;
using Code.Gameplay.Save.Components;
using Leopotam.EcsLite;

namespace Code.Gameplay.Business.Systems
{
    public class CreateSaveRequestOnBusinessUpdateSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld _world;
        private EcsFilter _updateBusinessRequests;

        public void Init(IEcsSystems systems)
        {
            _world = systems.GetWorld();

           _updateBusinessRequests = _world
               .Filter<UpdateBusinessRequestComponent>()
               .End();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var updateBusinessRequest in _updateBusinessRequests) 
                SendSaveRequest();
        }

        private void SendSaveRequest()
        {
            int entity = _world.NewEntity();
            _world.GetPool<SaveRequestComponent>().Add(entity);
        }
    }
}