using Bang.Components;

namespace LDGame.Components
{
    public readonly struct SuddenStopComponent : IComponent
    {
        public readonly float When = 0f;

        public SuddenStopComponent(float when)
        {
            When = when;
        }
    }
}