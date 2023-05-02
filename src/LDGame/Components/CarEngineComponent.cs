using Bang.Components;
using Murder.Attributes;
using Murder.Core.Geometry;

namespace LDGame.Components
{
    internal readonly struct CarEngineComponent : IComponent
    {
        public readonly Vector2 DesiredSpeed = Vector2.Zero;
        
        [Slider(0,1f)]
        public readonly float LerpAmount = 0.2f;

        public CarEngineComponent(Vector2 desiredSpeed, float lerpAmount)
        {
            DesiredSpeed = desiredSpeed;
            LerpAmount = lerpAmount;
        }
    }
}