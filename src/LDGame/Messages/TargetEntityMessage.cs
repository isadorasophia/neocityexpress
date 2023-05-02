using Bang.Components;
using Bang.Entities;

namespace LDGame.Messages
{
    internal readonly struct TargetEntityMessage : IMessage
    {
        public readonly Entity Entity { get; init; }
    }
}
