using Bang.Components;
using Murder.Attributes;
using Murder.Core.Dialogs;
using Murder.Utilities.Attributes;
using System.Collections.Immutable;

namespace LDGame.Components
{
    [RuntimeOnly]
    [DoNotPersistOnSave]
    public readonly struct CellphoneLineComponent : IComponent
    {
        public readonly Line Line;

        public readonly float Start = 0;

        /// <summary>
        /// Person who originally messaged.
        /// </summary>
        public readonly string Owner = string.Empty;

        public readonly ImmutableArray<string>? Choices = null;

        public CellphoneLineComponent(Line line, float start)
        {
            Line = line;
            Start = start;
        }

        public CellphoneLineComponent(Line line, float start, string owner)
        {
            Line = line;
            Start = start;

            Owner = owner;
        }

        public CellphoneLineComponent(string line, ImmutableArray<string> choices, float start, string owner)
        {
            Line = new(line);
            Start = start;
            Choices = choices;

            Owner = owner;
        }
    }
}
