using Bang;
using Bang.StateMachines;
using LDGame.Assets;
using LDGame.Components;
using LDGame.Core.Sounds;
using LDGame.Services;
using Murder.Diagnostics;
using Murder.Utilities.Attributes;

namespace LDGame.StateMachines
{
    [RuntimeOnly]
    internal class DispatchEventsBeforeDrivingStateMachine : StateMachine
    {
        private readonly DayCycle? _day = null;

        public DispatchEventsBeforeDrivingStateMachine(DayCycle day) : this()
        {
            _day = day;
        }

        public DispatchEventsBeforeDrivingStateMachine()
        {
            State(PrepareForDrive);
        }

        private IEnumerator<Wait> PrepareForDrive()
        {
            if (_day is null)
            {
                GameLogger.Fail("Unable to fetch the day cycle asset?");
                yield break;
            }

            using StopMovingCar stopMovingCar = new(World);

            // Iterate over all the events before starting to drive.
            DayCycle day = _day.Value;
            for (int i = 0; i < day.BeforeDriving.Length; ++i)
            {
                DriverlessDialogueEvent @event = day.BeforeDriving[i];

                TriggeredEventTrackerComponent? tracker = new(
                    @event.Type == MessageType.Monologue ? TriggeredEventTrackerKind.Monologue : 
                    TriggeredEventTrackerKind.Text);

                DialogueServices.TriggerDialogue(World, @event.Situation, InputType.Input, @event.Type, tracker);

                while (World.TryGetUniqueEntity<TriggeredEventTrackerComponent>() is not null)
                {
                    yield return Wait.NextFrame;
                }

                // Wait slightly longer to put cellphone dpwm.
                if (@event.Type == MessageType.Cellphone)
                {
                    yield return Wait.ForSeconds(1.5f);
                }
            }

            LDGameSoundPlayer.Instance.PlayEvent(LibraryServices.GetRoadLibrary().LevelMusic, isLoop: true);
            LDGameSoundPlayer.Instance.SetParameter(LibraryServices.GetRoadLibrary().LevelMusic, LibraryServices.GetRoadLibrary().EyesClosedParameter, 100);

            LDGameSoundPlayer.Instance.PlayEvent(LibraryServices.GetRoadLibrary().CarLoop, isLoop: true, stopLastMusic: false);
        }
    }
}
