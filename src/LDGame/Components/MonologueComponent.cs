using Bang.Components;
using LDGame.StateMachines;
using Murder.Attributes;
using Murder.Core.Dialogs;
using Murder.Utilities.Attributes;
using System.Collections.Immutable;

namespace LDGame.Components
{
    [RuntimeOnly]
    [DoNotPersistOnSave]
    public readonly struct MonologueComponent : IComponent
    {
        public readonly Line Line;

        public readonly InputType InputType = InputType.Time;

        public readonly ImmutableArray<string>? Choices = null;

        public MonologueComponent(Line line, InputType inputType)
        {
            Line = line;

            InputType = inputType;
        }

        public MonologueComponent(string line, ImmutableArray<string> choices, InputType inputType)
        {
            Line = new(line);
            Choices = choices;

            InputType = inputType;
        }
    }
}
