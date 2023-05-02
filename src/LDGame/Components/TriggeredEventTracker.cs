using Bang.Components;
using Murder.Utilities.Attributes;

namespace LDGame.Components
{
    public enum TriggeredEventTrackerKind
    {
        Monologue,
        Text,
        Tinder
    }

    [RuntimeOnly]
    public readonly struct TriggeredEventTrackerComponent : IComponent
    {
        public readonly TriggeredEventTrackerKind Kind;

        public readonly float Limit;

        public TriggeredEventTrackerComponent() { }

        public TriggeredEventTrackerComponent(TriggeredEventTrackerKind kind, float limit = 1) => (Kind, Limit) = (kind, limit);
    }
}
