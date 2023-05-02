using Bang.Components;
using Murder.Core.Geometry;

namespace LDGame.Messages
{
    public readonly struct CarCollisionMessage : IMessage
    {
        public readonly int OtherCarId = -1;
        public readonly Vector2 Direction = Vector2.Zero;
        public readonly float Magnetude = 0f;
        public readonly Vector2 Center = Vector2.Zero;

        public CarCollisionMessage(int otherCarId, Vector2 direction, Vector2 center) : this()
        {
            Center = center;
            OtherCarId = otherCarId;
            Direction = direction;
            Magnetude = direction.Length();
        }
    }
}