using Bang.Components;

namespace LDGame.Components
{
    public readonly struct InteractOnButtonPressComponent : IComponent
    {
        public readonly int Priority = 0;
        public readonly bool HighlightOnRange = true;
        public InteractOnButtonPressComponent() { }

        public InteractOnButtonPressComponent(bool highlightOnRange)
        {
            HighlightOnRange = highlightOnRange;
        }

        public InteractOnButtonPressComponent(int priority, bool highlightOnRange)
        {
            Priority = priority;
            HighlightOnRange = highlightOnRange;
        }
    }
}
