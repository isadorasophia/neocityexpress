using Bang.Components;
using Murder.Core.Geometry;

namespace LDGame.Components
{
    readonly struct SpawnOnDeathComponent : IComponent
    {
        public readonly Action OnDeath;
        public readonly float DestroyTime = 0;
        
        public SpawnOnDeathComponent(Action onDeath, float destroyTime)
        {
            OnDeath = onDeath;
            DestroyTime = destroyTime;
        }
    }
}