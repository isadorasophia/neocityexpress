using LDGame.Components;
using LDGame.StateMachines;
using Murder;
using Murder.Assets;
using Murder.Attributes;
using Murder.Components;
using Murder.Core;
using Murder.Utilities.Attributes;
using System.Collections.Immutable;

namespace LDGame.Assets
{
    public readonly struct DriverlessDialogueEvent
    {
        [ShowSenderOfSituation]
        public readonly SituationComponent Situation = new();

        public readonly MessageType Type = MessageType.Monologue;

        public DriverlessDialogueEvent() { }
    }

    public readonly struct DialogueEvent
    {
        [Tooltip("Slice of the journey that this dialogue will be randomly triggered.")]
        public readonly float Time = new();

        [Tooltip("Set when the event is forced on the player to resolve (progress is frozen).")]
        public readonly float? ForceEventBy;

        public readonly bool Pause = false;

        [ShowSenderOfSituation]
        public readonly SituationComponent Situation = new();

        public DialogueEvent() { }
    }

    public readonly struct TinderEvent
    {
        [Tooltip("Slice of the journey that this dialogue will be randomly triggered.")]
        public readonly float Time = new();

        [GameAssetId<DatingProfileAsset>]
        public readonly Guid Profiles = new();

        public TinderEvent() { }
    }

    public readonly struct DayCycle
    {
        public readonly ImmutableArray<DriverlessDialogueEvent> BeforeDriving = ImmutableArray<DriverlessDialogueEvent>.Empty;

        public readonly ImmutableArray<DialogueEvent> Monologues = ImmutableArray<DialogueEvent>.Empty;

        public readonly ImmutableArray<DialogueEvent> CellphoneTexts = ImmutableArray<DialogueEvent>.Empty;

        public readonly ImmutableArray<TinderEvent> TinderEvents = ImmutableArray<TinderEvent>.Empty;

        public readonly ImmutableArray<DriverlessDialogueEvent> AfterDriving = ImmutableArray<DriverlessDialogueEvent>.Empty;

        [GameAssetId<RoadCarsAsset>]
        public readonly ImmutableArray<Guid> PossibleCars = ImmutableArray<Guid>.Empty;
        
        [GameAssetId<ModifierAsset>]
        public readonly ImmutableArray<Guid> StartingModifiers = ImmutableArray<Guid>.Empty;

        public readonly int StartingHealth = 5;
        
        public readonly float Distance = 0;

        [Tooltip("Total of the distance which the car will travel for this day.")]
        public DayCycle() { }

        internal static DayCycle? TryGetCurrentDay(Bang.World world)
        {
            if (world.TryGetUnique<DayCycleComponent>() is DayCycleComponent dayCycle)
            {
                return Game.Data.TryGetAsset<DayCycleAsset>(dayCycle.DayCycle)?.Day;
            }

            return null;
        }
    }

    public readonly struct DeliveryStats
    {
        public readonly int Day = 1;
        public readonly string Content = string.Empty;
        public readonly string Destination = string.Empty;
        public readonly Portrait Image = new();

        public DeliveryStats() { }
    }

    internal class DayCycleAsset : GameAsset
    {
        public override string EditorFolder => "#\uf1b9Day";

        public override char Icon => '\uf1b9';

        [Tooltip("Events that will be triggered throughout the day.")]
        public readonly DayCycle Day = new();

        [GameAssetId<ConsequencesAsset>]
        [Tooltip("Consequences that will affect this day.")]
        public readonly ImmutableArray<Guid> Consequences = ImmutableArray<Guid>.Empty;

        [GameAssetId<DayCycleAsset>]
        [Tooltip("Next day which will follow this one.")]
        public Guid NextDay = Guid.Empty;

        [Tooltip("Content of the upcoming delivery.")]
        public DeliveryStats DeliveryContent = new();

        [Tooltip("Cutscene triggered when the delivery succeeded.")]
        public SituationComponent DeliveryEnded = new();
    }
}
