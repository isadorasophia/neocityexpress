using Bang.Components;
using Murder.Core.Geometry;

namespace LDGame.Systems.Cars
{
    readonly struct RelativeVelocityComponent : IComponent
    {
        public readonly Vector2 Velocity;

        public RelativeVelocityComponent(Vector2 velocity) =>
            Velocity = velocity;

        public RelativeVelocityComponent(float x, float y) =>
            Velocity = new Vector2(x, y);
    }
}