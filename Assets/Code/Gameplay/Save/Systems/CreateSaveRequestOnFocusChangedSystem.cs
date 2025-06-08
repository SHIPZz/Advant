using Code.Gameplay.Save.Components;
using Leopotam.EcsLite;
using UnityEngine;

namespace Code.Gameplay.Save.Systems
{
    public class CreateSaveRequestOnFocusChangedSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld _world;
        private bool _wasFocused = true;

        public void Init(IEcsSystems systems)
        {
            _world = systems.GetWorld();
        }

        public void Run(IEcsSystems systems)
        {
            bool isFocused = Application.isFocused;
            
            if (_wasFocused && !isFocused)
            {
                SendSaveRequest();
            }
            
            _wasFocused = isFocused;
        }

        private void SendSaveRequest()
        {
            int entity = _world.NewEntity();
            _world.GetPool<SaveRequestComponent>().Add(entity);
        }
    }
} 