using Bang.Components;
using Murder.Attributes;

namespace LDGame.Components
{
    internal readonly struct CarComponent : IComponent
    {
        public readonly float Speed = 100f;
        public readonly float Acceleration = 100f;

        [Slider(1,20f)]
        public readonly float Mass = 1;

        [Slider(0, 1)]
        public readonly float Friction = 0f;

        public CarComponent()
        {
        }
    }
}