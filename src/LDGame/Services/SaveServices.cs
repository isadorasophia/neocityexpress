using Bang;
using LDGame.Assets;
using LDGame.Core;
using Murder;
using Murder.Core;
using Murder.Core.Dialogs;
using Murder.Diagnostics;
using Murder.Services;

namespace LDGame.Services
{
    internal static class SaveServices
    {
        public static LdSaveData GetOrCreateSave()
        {
#if DEBUG
            if (Game.Instance.ActiveScene is not GameScene && Game.Data.TryGetActiveSaveData() is null)
            {
                GameLogger.Warning("Creating a save out of the game!");
            }
#endif

            if (Game.Data.TryGetActiveSaveData() is not LdSaveData save)
            {
                // Right now, we are creating a new save if one is already not here.
                save = (LdSaveData)Game.Data.CreateSave("_default");
            }

            return save;
        }

        public static LdSaveData? TryGetSave() => Game.Data.TryGetActiveSaveData() as LdSaveData;

        public static GameplayBlackboard GetGameplay()
        {
            return GetOrCreateSave().GameplayBlackboard;
        }
        public static void SetGameplayValue(string fieldName, bool value)
        {
            GetOrCreateSave().BlackboardTracker.SetBool(GameplayBlackboard.Name, fieldName, value);
        }

        public static void SetGameplayValue(string fieldName, int value)
        {
            GetOrCreateSave().BlackboardTracker.SetInt(GameplayBlackboard.Name, fieldName, BlackboardActionKind.Set, value);
        }

        public static void SetGameplayValue(string fieldName, string value)
        {
            GetOrCreateSave().BlackboardTracker.SetString(GameplayBlackboard.Name, fieldName, value);
        }

        internal static void AddGameplayValue(string fieldName, int value)
        {
            GetOrCreateSave().BlackboardTracker.SetInt(
                GameplayBlackboard.Name, fieldName, BlackboardActionKind.Add, value);
        }

        public static void QuickSave() => Game.Data.QuickSave();

        public static void OnDeath(World world, int seconds)
        {
            LdSaveData save = GetOrCreateSave();
            save.NextLevelCutscene = null;

            if (seconds == 0)
            {
                LevelServices.SwitchScene(LibraryServices.GetRoadLibrary().DeathWorld);
            }
            else
            {
                LevelServices.SwitchSceneOnSeconds(world, LibraryServices.GetRoadLibrary().DeathWorld, seconds);
            }
        }

        public static void OnDeath()
        {
            LevelServices.SwitchScene(LibraryServices.GetRoadLibrary().DeathWorld);
        }
    }
}
