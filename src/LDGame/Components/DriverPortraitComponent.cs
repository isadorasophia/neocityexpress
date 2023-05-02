using Bang.Components;
using Murder.Core;

namespace LDGame.Components
{
    internal readonly struct DriverPortraitComponent : IComponent
    {
        public readonly Portrait Portrait;

        public DriverPortraitComponent() { }

        public DriverPortraitComponent(Portrait portrait) => Portrait = portrait;
    }
}
