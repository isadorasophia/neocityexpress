using Bang;
using Bang.Components;
using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using LDGame.Components;
using Murder;
using Murder.Utilities;
using System.Collections.Immutable;

namespace LDGame.Systems.Cars
{
    [Filter(typeof(ITransformComponent), typeof(SpawnOnDeathComponent))]
    [Watch(typeof(SpawnOnDeathComponent))]
    public class DestroyAndSpawnSystem : IFixedUpdateSystem, IReactiveSystem
    {
        public void FixedUpdate(Context context)
        {
            foreach (var e in context.Entities)
            {
                var spawnOnDeath = e.GetSpawnOnDeath();
                if (spawnOnDeath.DestroyTime< Game.Now)
                {
                    e.Destroy();
                }
            }
        }

        public void OnAdded(World world, ImmutableArray<Entity> entities)
        {
        }

        public void OnModified(World world, ImmutableArray<Entity> entities)
        {
        }

        public void OnRemoved(World world, ImmutableArray<Entity> entities)
        {
            foreach (var e in entities)
            {
                var spawnOnDeath = e.GetSpawnOnDeath();
                spawnOnDeath.OnDeath?.Invoke();
            }
        }
    }
}