using Code.Gameplay.Save.Components;
using Leopotam.EcsLite;
using UnityEngine;

namespace Code.Gameplay.Save.Systems
{
    public class CreateSaveRequestOnFocusChangedSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld _world;
        private bool _isPaused;

        public void Init(IEcsSystems systems)
        {
            _world = systems.GetWorld();
            _isPaused = false;
        }

        public void Run(IEcsSystems systems)
        {
            if (Application.isFocused && _isPaused)
            {
                SendSaveRequest();
                _isPaused = false;
            }
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                SendSaveRequest();
                _isPaused = true;
            }
        }

        private void SendSaveRequest()
        {
            int entity = _world.NewEntity();
            _world.GetPool<SaveRequestComponent>().Add(entity);
        }
    }
} 