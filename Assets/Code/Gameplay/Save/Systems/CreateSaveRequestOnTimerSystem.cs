using Code.Gameplay.Save.Components;
using Leopotam.EcsLite;
using UnityEngine;

namespace Code.Gameplay.Save.Systems
{
    public class CreateSaveRequestOnTimerSystem : IEcsInitSystem, IEcsRunSystem
    {
        private const float SaveInterval = 5f;
        
        private EcsWorld _world;
        private float _timeSinceLastSave;

        public void Init(IEcsSystems systems)
        {
            _world = systems.GetWorld();
            _timeSinceLastSave = 0f;
        }

        public void Run(IEcsSystems systems)
        {
            _timeSinceLastSave += Time.deltaTime;
            
            if (_timeSinceLastSave >= SaveInterval)
            {
                 SendSaveRequest();
                _timeSinceLastSave = 0f;
            }
        }

        private void SendSaveRequest()
        {
            int entity = _world.NewEntity();
            _world.GetPool<SaveRequestComponent>().Add(entity);
        }
    }
} 