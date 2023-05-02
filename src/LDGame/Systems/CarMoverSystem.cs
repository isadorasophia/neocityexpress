using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using LDGame.Components;
using LDGame.Services;
using Murder;
using Murder.Components;
using Murder.Components.Agents;
using Murder.Core.Geometry;

namespace LDGame.Systems
{
    /// <summary>
    /// System that looks for AgentImpulse systems and translated them into 'Velocity' for the physics system.
    /// </summary>
    [Filter(typeof(CarComponent), typeof(AgentImpulseComponent))]
    [Filter(ContextAccessorFilter.NoneOf, typeof(DisableAgentComponent), typeof(CarEngineStoppedComponent) )]
    internal class CarMoverSystem : IFixedUpdateSystem
    {
        public void FixedUpdate(Context context)
        {
            var save = SaveServices.GetOrCreateSave();
            bool hasTheGoodShit = false;
            foreach (var item in save.Modifiers)
            {
                if (item.TheGoodShit)
                {
                    hasTheGoodShit = true;
                    break;
                }
            }

            foreach (var e in context.Entities)
            {
                var car = e.GetComponent<CarComponent>();
                var impulse = e.GetAgentImpulse();
                if (!impulse.Impulse.HasValue)
                {
                    continue;
                }
                
                Vector2 startVelocity = e.TryGetVelocity()?.Velocity ?? Vector2.Zero;

                // Use friction on any axis that's not receiving impulse or is receiving it in an opposing direction
                var result = GetVelocity(e, car, impulse, startVelocity, hasTheGoodShit);

                e.RemoveFriction();     // Remove friction to move
                e.SetVelocity(result); // Turn impulse into velocity
            }
        }

        private static Vector2 GetVelocity(Entity entity, CarComponent car, AgentImpulseComponent impulse, in Vector2 currentVelocity, bool hasTheGoodShit)
        {
            float multiplier = 1f;
            if (entity.TryGetAgentSpeedMultiplier() is AgentSpeedMultiplier speedMultiplier)
                multiplier = speedMultiplier.SpeedMultiplier;

            float speed, accel;
            if (entity.TryGetAgentSpeedOverride() is AgentSpeedOverride speedOverride)
            {
                speed = speedOverride.MaxSpeed;
                accel = speedOverride.Acceleration;
            }
            else
            {
                speed = car.Speed;
                accel = car.Acceleration;
            }

            if (hasTheGoodShit)
                multiplier += 0.5f;
            
            float xImpulse = impulse.Impulse.X * accel * multiplier * Game.FixedDeltaTime;
            if (Math.Sign(currentVelocity.X) != Math.Sign(impulse.Impulse.X))
            {
                xImpulse *= 2;
            }
            float yImpulse = impulse.Impulse.Y * accel * multiplier * Game.FixedDeltaTime;
            if (Math.Sign(currentVelocity.Y) != Math.Sign(impulse.Impulse.Y))
            {
                yImpulse *= 2;
            }

            Vector2 finalVelocity = currentVelocity + new Vector2(xImpulse, yImpulse);
            finalVelocity = finalVelocity.Clamp(speed * multiplier);

            return finalVelocity;
        }
    }
}
