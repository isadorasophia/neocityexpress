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
            LDGameSoundPlayer.Instance.Stop(LibraryServices.GetRoadLibrary().CarLoop, false);
        }

        public void FixedUpdate(Context context)
        {
            foreach (var e in context.Entities)
            {
                float pitch = Calculator.Clamp01((Game.Profile.GameHeight - e.GetGlobalTransform().Vector2.Y) / (float)Game.Profile.GameHeight) * 100f;
                
                // TODO: Fix
                // LDGameSoundPlayer.Get.SetParameter(sound, "Car_Pitch", pitch);
            }
        }

        public void OnAdded(World world, ImmutableArray<Entity> entities)
        {
            //LDGameSoundPlayer.Instance.PlayEvent(LibraryServices.GetRoadLibrary().CarLoop, isLoop: false);
        }

        public void OnModified(World world, ImmutableArray<Entity> entities)
        {
        }

        public void OnRemoved(World world, ImmutableArray<Entity> entities)
        {
            LDGameSoundPlayer.Instance.Stop(LibraryServices.GetRoadLibrary().CarLoop, false);
        }
        
    }
}