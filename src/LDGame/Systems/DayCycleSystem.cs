using Bang;
using Bang.Contexts;
using Bang.Entities;
using Bang.Interactions;
using Bang.StateMachines;
using Bang.Systems;
using LDGame.Assets;
using LDGame.Components;
using LDGame.Core.Sounds;
using LDGame.Services;
using LDGame.StateMachines;
using LDGame.Systems.Player;
using Murder;
using Murder.Assets;
using Murder.Diagnostics;
using Murder.Interactions;
using System.Collections.Immutable;
using System.Numerics;

namespace LDGame.Systems
{
    [Filter(typeof(DayCycleComponent))]
    internal class DayCycleSystem : IStartupSystem, IUpdateSystem
    {
        private DayCycle? _day;

        private readonly HashSet<int> _triggeredMonologues = new();
        private readonly HashSet<int> _triggeredCellphoneTexts = new();
        private readonly HashSet<int> _triggeredTinders = new();

        public void Start(Context context)
        {
            LdSaveData save = SaveServices.GetOrCreateSave();
            if (save.NextLevel is Guid nextLevel && nextLevel != Guid.Empty)
            {
                context.Entity.SetDayCycle(nextLevel);
            }

            LDGameSoundPlayer.Instance.Stop(fadeOut: true);

            Guid guid = context.Entity.GetDayCycle().DayCycle;

            DayCycleAsset? asset = Game.Data.TryGetAsset<DayCycleAsset>(guid);
            if (asset is null)
            {
                GameLogger.Fail("Unable to fetch the day cycle asset?");
                return;
            }

            _day = asset.Day;

            foreach (Guid consequence in asset.Consequences)
            {
                ConsequencesAsset? consequencesAsset = Game.Data.TryGetAsset<ConsequencesAsset>(consequence);
                if (consequencesAsset is null)
                {
                    continue;
                }

                // Add all the consequences in game.
                foreach (Consequence trigger in consequencesAsset.Consequences)
                {
                    context.World.AddEntity(
                        trigger.Triggers, 
                        new InteractiveComponent<InteractionCollection>(trigger.Effects)
                        );
                }
            }

            DoModifiers(context.World, save, asset.Day.StartingModifiers);

            // Now, save the upcoming level.
            save.NextLevel = asset.NextDay;
            save.NextLevelCutscene = asset.DeliveryEnded;

            save.Health = asset.Day.StartingHealth;
            var levelManager = new StateMachineComponent<CarsLevelManager>(new CarsLevelManager(asset.Day.PossibleCars));
            context.World.AddEntity(levelManager);

            context.World.AddEntity(new StateMachineComponent<DispatchEventsBeforeDrivingStateMachine>(new(_day.Value)));
        }

        private void DoModifiers(World world, LdSaveData save, ImmutableArray<Guid> startingDayModifiers)
        {
            save.HasSway = false;
            save.SwayDirection = Vector2.Zero;

            save.Modifiers.Clear();
            foreach (Guid mod in startingDayModifiers)
            {
                ModifierAsset modifierAsset = Game.Data.GetAsset<ModifierAsset>(mod);
                save.Modifiers.Add(modifierAsset.Modifier);

                if (modifierAsset.Modifier.ConstantInput.HasValue)
                {
                    save.HasSway = true;
                }

                if (modifierAsset.Sleepy)
                {
                    save.Sleepy = true;
                    world.ActivateSystem<DrowsySystem>();
                }
            }
        }
        
        public void Update(Context context)
        {
            if (context.Entities.Length == 0 || _day is null)
            {
                return;
            }

            LdSaveData save = SaveServices.GetOrCreateSave();
            float progressPercentile = save.TraveledDistance / _day.Value.Distance;

            DispatchPendingEvents(context.World, progressPercentile);
        }

        private void DispatchPendingEvents(World world, float progressPercentile)
        {
            if (_day is null)
            {
                return;
            }

            // Check if any monologue was triggered.
            for (int i = 0; i < _day.Value.Monologues.Length; ++i)
            {
                if (_triggeredMonologues.Contains(i))
                {
                    continue;
                }

                DialogueEvent @event = _day.Value.Monologues[i];
                if (@event.Time < progressPercentile)
                {
                    if (world.TryGetUniqueEntity<MonologueComponent>() is not null)
                    {
                        // Dialogue is already in place, wait for this to finish.
                        continue;
                    }

                    TriggeredEventTrackerComponent? tracker = null;
                    if (@event.ForceEventBy is float limit)
                    {
                        tracker = new(TriggeredEventTrackerKind.Monologue, limit);
                    }

                    // TODO: Make this random?
                    DialogueServices.TriggerDialogue(world, @event.Situation, @event.Pause ? InputType.PauseGame : InputType.Time, MessageType.Monologue, tracker);

                    _triggeredMonologues.Add(i);
                    break;
                }
            }

            // Check if any cellphone text was triggered.
            for (int i = 0; i < _day.Value.CellphoneTexts.Length; ++i)
            {
                if (_triggeredCellphoneTexts.Contains(i))
                {
                    continue;
                }

                DialogueEvent @event = _day.Value.CellphoneTexts[i];
                if (@event.Time < progressPercentile)
                {
                    // Always track cellphone events.
                    TriggeredEventTrackerComponent? tracker = new(TriggeredEventTrackerKind.Monologue);

                    //TriggeredEventTrackerComponent? tracker = null;
                    //if (@event.ForceEventBy is float limit)
                    //{
                    //    tracker = new(TriggeredEventTrackerKind.Monologue);
                    //}

                    // TODO: Make this random?
                    DialogueServices.TriggerDialogue(world, @event.Situation, @event.Pause ? InputType.PauseGame : InputType.Time, MessageType.Cellphone, tracker);

                    _triggeredCellphoneTexts.Add(i);
                }
            }

            // Check if any cellphone text was triggered.
            for (int i = 0; i < _day.Value.TinderEvents.Length; ++i)
            {
                if (_triggeredTinders.Contains(i))
                {
                    continue;
                }

                TinderEvent @event = _day.Value.TinderEvents[i];
                if (@event.Time < progressPercentile)
                {
                    // Always track tinder events.
                    TriggeredEventTrackerComponent? tracker = new(TriggeredEventTrackerKind.Tinder);

                    //TriggeredEventTrackerComponent? tracker = null;
                    // if (@event.ForceEventBy is float limit)
                    //{
                    //    tracker = new(TriggeredEventTrackerKind.Monologue, limit);
                    //}

                    DialogueServices.TriggerDating(world, @event.Profiles, tracker);

                    _triggeredTinders.Add(i);
                }
            }
        }
    }
}
