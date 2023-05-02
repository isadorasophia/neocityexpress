using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using LDGame.Components;
using LDGame.Systems.Cars;

namespace LDGame.Systems
{
    [Filter(typeof(RelativeVelocityComponent))]
    public class RelativeVelocitySystem : IFixedUpdateSystem
    {
        public void FixedUpdate(Context context)
        {
            if (context.World.TryGetUnique<PlayerSpeedComponent>() is not PlayerSpeedComponent playerSpeed)
                return;

            foreach (var e in context.Entities)
            {
                RelativeVelocityComponent relative = e.GetRelativeVelocity();

                e.SetVelocity(relative.Velocity.X, relative.Velocity.Y + playerSpeed.Speed * 100f);
            }
        }
    }
}