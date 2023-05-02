using Bang.Components;
using Murder.Core;

namespace LDGame.Components
{
    internal readonly struct GuestPortraitComponent : IComponent
    {
        public readonly Portrait Portrait;

        public GuestPortraitComponent() { }

        public GuestPortraitComponent(Portrait portrait)
        {
            Portrait = portrait;
        }
    }
}
