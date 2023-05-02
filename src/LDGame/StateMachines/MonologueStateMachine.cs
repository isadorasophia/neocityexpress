using Bang.Components;
using Bang.Entities;
using Bang.StateMachines;
using LDGame.Services;
using Murder;
using Murder.Assets;
using Murder.Components;
using Murder.Core.Dialogs;
using Murder.Diagnostics;
using Murder.Messages;
using Murder.StateMachines;
using Murder.Utilities;
using System.Diagnostics;

namespace LDGame.StateMachines
{
    public enum MessageType
    {
        Monologue = 0,
        Cellphone = 1
    }

    public enum InputType
    {
        Time = 1,
        PauseGame = 2,
        Input = 3 // input without pause
    }

    public class MonologueStateMachine : DialogStateMachine
    {
        private readonly InputType _inputType = InputType.Time;

        private readonly MessageType _message = MessageType.Monologue;

        public MonologueStateMachine()
        {
            State(BeforeTalk);
        }

        public MonologueStateMachine(InputType inputType, MessageType message) : this() 
        {
            _inputType = inputType;
            _message = message;
        }

        public IEnumerator<Wait> BeforeTalk()
        {
            if (_inputType is InputType.PauseGame)
            {
                using PartiallyPauseGame freeze = new(World);

                yield return Wait.ForRoutine(TalkMonologue());
            }
            else
            {
                yield return Wait.ForRoutine(TalkMonologue());
            }
        }

        public IEnumerator<Wait> TalkMonologue()
        {
            Debug.Assert(_character is not null);

            while (true)
            {
                if (_character.NextLine(World, Entity) is not DialogLine dialogLine)
                {
                    Entity.RemoveTriggeredEventTracker();

                    // No line was ever added, destroy the dialog.
                    if (!Entity.HasMonologue() && !Entity.HasCellphoneLine())
                    {
                        Entity.RemoveCustomDraw();
                        Entity.RemoveStateMachine();

                        yield break;
                    }

                    Entity.RemoveMonologue();
                    Entity.RemoveCellphoneLine();

                    yield break;
                }

                if (dialogLine.Line is Line line)
                {
                    if (_message == MessageType.Monologue)
                    {
                        Entity.SetMonologue(line, _inputType);
                    }
                    else if (_message == MessageType.Cellphone)
                    {
                        string speaker = FetchSpeaker(line.Speaker);
                        Entity.SetCellphoneLine(line, Game.NowUnescaled, speaker);
                    }

                    if (line.IsText)
                    {
                        yield return Wait.NextFrame;

                        yield return Wait.ForMessage<NextDialogMessage>();
                    }
                    else if (line.Delay is float delay)
                    {
                        int ms = Calculator.RoundToInt(delay * 1000);
                        yield return Wait.ForMs(ms);
                    }
                }
                else if (dialogLine.Choice is ChoiceLine choice)
                {
                    if (_message == MessageType.Monologue)
                    {
                        Entity.SetMonologue(choice.Title, choice.Choices, _inputType);
                    }
                    else if (_message == MessageType.Cellphone)
                    {
                        string speaker = FetchSpeaker(dialogLine.Line?.Speaker);
                        Entity.SetCellphoneLine(choice.Title, choice.Choices, Game.NowUnescaled, speaker);
                    }

                    yield return Wait.NextFrame;
                    yield return Wait.ForMessage<PickChoiceMessage>();

                    if (_choice is not int choiceIndex)
                    {
                        GameLogger.Error("How do we not track a choice made by the player?");

                        Entity.Destroy();
                        yield break;
                    }

                    _character.DoChoice(choiceIndex, World, Entity);
                }
            }
        }

        private string FetchSpeaker(Guid? speakerGuid)
        {
            Debug.Assert(_character is not null);

            Guid driver = LibraryServices.GetRoadLibrary().DriverSpeaker;

            if (Entity.TryGetSituation() is SituationComponent situation &&
                            !string.IsNullOrEmpty(situation.Sender))
            {
                return situation.Sender;
            }
            else if (speakerGuid is not null && speakerGuid != driver &&
                Game.Data.TryGetAsset<SpeakerAsset>(speakerGuid.Value) is SpeakerAsset speakerAsset)
            {
                return speakerAsset.SpeakerName;
            }
            else if (Game.Data.TryGetAsset<SpeakerAsset>(_character.Owner) is SpeakerAsset character)
            {
                return character.SpeakerName;
            }

            return string.Empty;
        }

        protected override void OnMessage(IMessage message)
        {
            if (message is PickChoiceMessage pickChoiceMessage)
            {
                _choice = pickChoiceMessage.Choice;
            }
        }
    }
}
