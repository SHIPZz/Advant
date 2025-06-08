using Code.Gameplay.Save.Components;
using Leopotam.EcsLite;
using UnityEngine;

namespace Code.Gameplay.Save.Systems
{
    public class CreateSaveRequestOnFocusChangedSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld _world;
        private EcsPool<SaveRequestComponent> _saveRequestPool;

        public void Init(IEcsSystems systems)
        {
            _world = systems.GetWorld();
            _saveRequestPool = _world.GetPool<SaveRequestComponent>();
        }

        public void Run(IEcsSystems systems)
        {
            if (!Application.isFocused)
            {
                SendSaveRequest();
            }
        }

        private void SendSaveRequest()
        {
            int entity = _world.NewEntity();
            _saveRequestPool.Add(entity);
        }
    }
} 