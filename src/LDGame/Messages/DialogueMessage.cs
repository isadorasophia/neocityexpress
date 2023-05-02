using Bang.Components;
using LDGame.Components;

namespace LDGame.Messages
{
    internal enum SpeakerKind
    {
        Driver,
        Granny,
        Passenger,
        Car
    }

    internal readonly struct DialogueMessage : IMessage
    {
        public readonly MonologueComponent Monologue { get; init; } = new();

        public readonly SpeakerKind SpeakerKind { get; init; } = SpeakerKind.Driver;

        public readonly bool Clear { get; init; } = false;

        public DialogueMessage(MonologueComponent monologue, SpeakerKind speaker) => (Monologue, SpeakerKind) = (monologue, speaker);

        public static DialogueMessage CreateClear() => new() { Clear = true };
    }
}
