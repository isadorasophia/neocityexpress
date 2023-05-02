using Bang;
using Bang.Contexts;
using Bang.Entities;
using Bang.StateMachines;
using LDGame.Assets;
using LDGame.Core.Sounds;
using Murder;
using Murder.Assets;
using Murder.Components;
using Murder.Core.Geometry;
using Murder.Utilities;

namespace LDGame.Services
{
    public static class LibraryServices
    {
        public static LibraryAsset GetRoadLibrary()
        {
            return Game.Data.GetAsset<LibraryAsset>(LDGame.Profile.Library);
        }
        
        internal static Entity DropItem(World world, Vector2 position, Vector2 velocity, PrefabAsset prefab)
        {
            var spawned = prefab.CreateAndFetch(world);
            //spawned.SetIgnoreUntil(Game.Now + 0.75f);
            spawned.SetIgnoreTriggersUntil(Game.Now + 0.75f);
            spawned.SetGlobalTransform(new PositionComponent(position));
            spawned.SetVelocity(velocity);
            return spawned;
        }

        public static UiSkinAsset GetUiSkin()
        {
            return Game.Data.GetAsset<UiSkinAsset>(GetRoadLibrary().UiSkin);
        }

        internal static void Explode(int explosionSize, World world, Vector2 position)
        {
            var explosion = Game.Data.GetPrefab(GetRoadLibrary().Explosions[explosionSize]).CreateAndFetch(world);
            explosion.SetGlobalPosition(position);

            LDGameSoundPlayer.Instance.PlayEvent(GetRoadLibrary().CarImpact, isLoop: false);
        }

        internal static PrefabAsset GetPausePrefab()
        {
            return Game.Data.GetAsset<PrefabAsset>(GetRoadLibrary().PausePrefab);
        }
    }
}
