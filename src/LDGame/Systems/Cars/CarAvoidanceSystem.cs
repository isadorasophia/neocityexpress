using Bang;
using Bang.Components;
using Bang.Entities;
using Bang.Systems;
using LDGame.Components;
using LDGame.Messages;
using Murder;
using Murder.Messages.Physics;

namespace LDGame.Systems.Cars
{
    [Messager(typeof(OnActorEnteredOrExitedMessage))]
    internal class CarAvoidanceSystem : IMessagerSystem
    {
        public void OnMessage(World world, Entity entity, IMessage message)
        {
            var msg = (OnActorEnteredOrExitedMessage)message;

            if (world.TryGetEntity(msg.EntityId) is Entity other)
            {
                if (other.HasCar() && !other.HasPlayer())
                {
                    if (entity.TryFetchParent() is Entity parent)
                    {
                        parent.SetIncomingCollision(Game.Now + 0.1f);
                    }
                }
            }
        }
    }
}