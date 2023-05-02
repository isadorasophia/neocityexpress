using Bang;
using Bang.Components;
using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using LDGame.Components;
using LDGame.Messages;
using Murder;
using Murder.Components;
using Murder.Core;
using Murder.Core.Dialogs;
using Murder.Core.Geometry;
using Murder.Core.Physics;
using Murder.Diagnostics;
using Murder.Messages;
using Murder.Services;
using Murder.Utilities;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Threading.Tasks;

namespace LDGame.Systems;

[Filter(typeof(CarComponent))]
[Messager(typeof(CollidedWithMessage))]
public class CarPhysicsSystem : IMessagerSystem
{
    public void OnMessage(World world, Entity entity, IMessage message)
    {
        var msg = (CollidedWithMessage)message;
        var other = world.GetEntity(msg.EntityId);

        float relative = 0;
        if (world.TryGetUnique<PlayerSpeedComponent>() is PlayerSpeedComponent playerSpeed)
        {
            relative = playerSpeed.Speed;
        }
        
        HandleCollision(entity, other, msg.Pushout, relative);
    }

    public void HandleCollision(Entity entity1, Entity entity2, Vector2 pushAwayVector, float worldSpeed)
    {
        // Calculate the collision normal
        Vector2 collisionNormal = pushAwayVector.Normalized();

        Vector2 velocity1 = entity1.TryGetVelocity()?.Velocity ?? Vector2.Zero;
        Vector2 velocity2 = entity2.TryGetVelocity()?.Velocity ?? Vector2.Zero;

        // Compute relative velocity
        Vector2 relativeVelocity = velocity1 - velocity2;

        // Calculate the impulse along the collision normal
        float impulse = Vector2.Dot(relativeVelocity, collisionNormal);

        // If the entities are moving away from each other, no need to handle the collision
        if (impulse <= 0)
        {
            return;
        }

        // Calculate the mass of the entities (assuming equal mass for both)
        float mass1 = entity1.GetCar().Mass;
        float mass2 = entity2.GetCar().Mass;

        // Compute the impulse scalar
        float impulseScalar = -(1 + 0.8f /*restitution*/) * impulse / (mass1 + mass2);

        // Calculate the impulse vectors for both entities
        Vector2 impulseVector1 = impulseScalar * collisionNormal * 2; // Times 2 because we want a stronger impact
        Vector2 impulseVector2 = -impulseScalar * collisionNormal * 2; // Times 2 because we want a stronger impact

        // Update the velocities of the entities
        if (entity1.HasPlayer())
        {
            entity1.SetVelocity(velocity1 + impulseVector1 * 1.3f);

            Vector2 realVelocity = velocity2 + impulseVector2;
            entity2.SetRelativeVelocity(realVelocity.X, realVelocity.Y - worldSpeed * 100);
        }
        else
        {
            entity2.SetVelocity(velocity2 + impulseVector2 * 1.3f);
            
            Vector2 realVelocity = velocity1 + impulseVector1;
            entity1.SetRelativeVelocity(realVelocity.X, realVelocity.Y - worldSpeed * 100);

        }

        Vector2 center = Vector2.Lerp(entity1.GetGlobalTransform().Vector2, entity2.GetGlobalTransform().Vector2, 0.5f);

        entity1.SendMessage(new CarCollisionMessage(entity2.EntityId, pushAwayVector, center));
        entity2.SendMessage(new CarCollisionMessage(entity2.EntityId, -pushAwayVector, center));
    }

}
