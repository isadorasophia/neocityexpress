using Bang;
using Bang.Components;
using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using LDGame.Assets;
using LDGame.Components;
using LDGame.Core;
using LDGame.Services;
using Murder;
using System.Collections.Immutable;

namespace LDGame.Systems
{
    [Requires(typeof(DayCycleSystem))]
    [Filter(typeof(TriggeredEventTrackerComponent))]
    internal class CarProgressSystem : IStartupSystem, IFixedUpdateSystem
    {
        public void Start(Context context)
        {
            LdSaveData save = SaveServices.GetOrCreateSave();
            save.TraveledDistance = 0;
        }

        private bool _reachedEnd = false;

        private bool _stoppedProgress = false;

        public void FixedUpdate(Context context)
        {
            if (DayCycle.TryGetCurrentDay(context.World) is not DayCycle dayCycle)
            {
                return;
            }

            if (context.World.TryGetUnique<PlayerSpeedComponent>() is not PlayerSpeedComponent playerSpeed)
            {
                return;
            }

            LdSaveData save = SaveServices.GetOrCreateSave();

            float totalProgress = save.TraveledDistance / dayCycle.Distance;
            // CanProgressCar(context.Entities, totalProgress);

            // We block progress on any event that trigger this block.
            bool canProgress = context.Entities.Length == 0; 
            if (!canProgress)
            {
                if (!_stoppedProgress)
                {
                    SaveServices.SetGameplayValue(nameof(GameplayBlackboard.PanickedAndCantReply), true);
                    _stoppedProgress = true;
                }

                // We are having a bad time until we address whatever is happening here.
                return;
            }

            if (_stoppedProgress)
            {
                SaveServices.SetGameplayValue(nameof(GameplayBlackboard.PanickedAndCantReply), false);
                _stoppedProgress = false;
            }

            // Update time!
            save.TraveledDistance += Game.FixedDeltaTime * playerSpeed.Speed;
            totalProgress = save.TraveledDistance / dayCycle.Distance;

            if (totalProgress >= .95f && !_reachedEnd
                && !context.World.GetEntitiesWith(typeof(EnemyComponent)).Any())
            {
                // Only make this once.
                _reachedEnd = true;

                SaveServices.SetGameplayValue(nameof(GameplayBlackboard.FinishedRaceOfDay), true);
            }
        }

        private bool CanProgressCar(ImmutableArray<Entity> entities, float totalProgress)
        {
            foreach (Entity e in entities)
            {
                TriggeredEventTrackerComponent tracker = e.GetTriggeredEventTracker();
                if (tracker.Limit > totalProgress)
                {
                    // Ops! This is bad. Stop things immediately.
                    return false;
                }
            }

            return true;
        }
    }
}
