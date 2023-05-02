using Bang.Entities;
using Bang.StateMachines;
using LDGame.Assets;
using LDGame.Components;
using LDGame.Core;
using LDGame.Services;
using LDGame.Systems;
using LDGame.Systems.Player;
using Murder;
using Murder.Assets;
using Murder.Attributes;
using Murder.Core.Geometry;
using Murder.Services;
using Murder.Utilities;
using Newtonsoft.Json;

namespace LDGame.StateMachines
{
    internal class RaceEndedCutscene : StateMachine
    {
        [JsonProperty]
        [GameAssetId<WorldAsset>]
        private readonly Guid _endWorld = new();

        public RaceEndedCutscene()
        {
            State(Start);
        }

        public IEnumerator<Wait> Start()
        {
            // reset.
            SaveServices.SetGameplayValue(nameof(GameplayBlackboard.FinishedRaceOfDay), false);
            SaveServices.SetGameplayValue(nameof(GameplayBlackboard.ScaredByAlmostAccident), false);

            // stop being drowsy, if we are.
            World.DeactivateSystem<DrowsySystem>();

            // Stop spawning cars!
            World.TryGetUniqueEntity<CarLevelManagerComponent>()?.Destroy();

            // Let the cars go away...
            yield return Wait.ForSeconds(1);
            
            World.DeactivateSystem<PlayerInputSystem>();

            Entity? car = World.TryGetUniqueEntity<PlayerComponent>();
            if (car is null)
            {
                SaveServices.OnDeath();
                yield break;
            }

            IntRectangle roadBounds = LibraryServices.GetRoadLibrary().Bounds;
            car.SetMoveToPerfect(roadBounds.TopRight - new Vector2(x: 30, y: -180), 5, EaseKind.CubeOut);
            car.SetCarEngineStopped();

            while (car.HasMoveToPerfect())
            {
                car.SetPlayerSpeed(car.GetPlayerSpeed().Brake(Game.DeltaTime));
                yield return Wait.NextFrame;
            }

            // =====================
            // Do any pending events
            // =====================
            Guid? dayCycle = World.TryGetUnique<DayCycleComponent>()?.DayCycle;

            DayCycle? day = dayCycle is null ? null : Game.Data.TryGetAsset<DayCycleAsset>(dayCycle.Value)?.Day;
            if (day is not null)
            {
                // Iterate over all the events before finishing to drive.
                for (int i = 0; i < day.Value.AfterDriving.Length; ++i)
                {
                    DriverlessDialogueEvent @event = day.Value.AfterDriving[i];

                    TriggeredEventTrackerComponent? tracker = new(
                        @event.Type == MessageType.Monologue ? TriggeredEventTrackerKind.Monologue :
                        TriggeredEventTrackerKind.Text);

                    DialogueServices.TriggerDialogue(World, @event.Situation, InputType.Input, @event.Type, tracker);

                    while (World.TryGetUniqueEntity<TriggeredEventTrackerComponent>() is not null)
                    {
                        yield return Wait.NextFrame;
                    }
                }
            }

            // TODO: Do animation here.
            yield return Wait.ForSeconds(1);

            LevelServices.SwitchScene(_endWorld);
        }
    }
}
