using System;

namespace Code.Gameplay.Business.Components
{
    [Serializable]
    public struct BusinessSaveComponent
    {
        public int BusinessId;
        public int Level;
        public float Progress;
        public float Cooldown;
    }
} 