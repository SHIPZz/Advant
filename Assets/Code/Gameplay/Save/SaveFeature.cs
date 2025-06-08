using Code.Common.Features;
using Code.Gameplay.Save.Systems;
using Leopotam.EcsLite;

namespace Code.Gameplay.Save
{
    public class SaveFeature : Feature
    {
        private readonly ISaveService _saveService;
        
        public SaveFeature(EcsWorld world, IEcsSystems systems, ISaveService saveService) : base(world, systems)
        {
            _saveService = saveService;
        }

        public override void RegisterSystems()
        {
            Systems
                .Add(new SaveRequestSystem())
                .Add(new SaveOnRequestSystem(_saveService));
        }
    }
} 