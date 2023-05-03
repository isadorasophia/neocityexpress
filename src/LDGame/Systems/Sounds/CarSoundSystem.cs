using Bang;
using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using LDGame.Components;
using LDGame.Core.Sounds;
using LDGame.Services;
using Murder;
using Murder.Utilities;
using System.Collections.Immutable;

namespace LDGame.Systems
{
    [Filter(typeof(PlayerComponent))]
    [Watch(typeof(PlayerComponent))]
    internal class CarSoundSystem : IFixedUpdateSystem, IReactiveSystem, IExitSystem
    {
        public void Exit(Context context)
        {
            LDGameSoundPlayer.Instance.Stop(LibraryServices.GetRoadLibrary().CarLoop, fadeOut: true);
        }

        public void FixedUpdate(Context context)
        {
            foreach (var e in context.Entities)
            {
                float pitch = Calculator.Clamp01((Game.Profile.GameHeight - e.GetGlobalTransform().Vector2.Y) / (float)Game.Profile.GameHeight) * 100f;

                LDGameSoundPlayer.Instance.SetGlobalParameter(LibraryServices.GetRoadLibrary().CarPitchParameter, pitch);
            }
        }

        public void OnAdded(World world, ImmutableArray<Entity> entities)
        {
        }

        public void OnModified(World world, ImmutableArray<Entity> entities) { }

        public void OnRemoved(World world, ImmutableArray<Entity> entities)
        {
            LDGameSoundPlayer.Instance.Stop(LibraryServices.GetRoadLibrary().CarLoop, fadeOut: true);
        }
    }
}