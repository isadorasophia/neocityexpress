using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using LDGame.Components;
using LDGame.Services;
using Murder;
using Murder.Core.Geometry;

namespace LDGame.Systems;

[Filter(typeof(CarEngineComponent),typeof(CarComponent))]
[Filter(ContextAccessorFilter.NoneOf, typeof(CarEngineStoppedComponent))]
public class CarEngineSystem : IFixedUpdateSystem
{
    public void FixedUpdate(Context context)
    {
        var save = SaveServices.GetOrCreateSave();
        float multiplier = 1f;
        foreach (var mod in save.Modifiers)
        {
            multiplier *= mod.OtherCarsSpeedMultiplier;
        }
        foreach (var e in context.Entities)
        {

            var engine = e.GetComponent<CarEngineComponent>();
            var car = e.GetComponent<CarComponent>();
            var velocity = e.TryGetRelativeVelocity()?.Velocity ?? Vector2.Zero;
            var targetVelocity = engine.DesiredSpeed * car.Speed * multiplier;

            if (e.TryGetIncomingCollision() is IncomingCollisionComponent incomingCollision)
            {
                if (incomingCollision.When < Game.Now)
                {
                    e.RemoveIncomingCollision();
                }
                else
                {
                    targetVelocity = new Vector2 (-200, targetVelocity.Y * 0.25f);
                }
            }
            
            e.SetRelativeVelocity(Vector2.LerpSnap(velocity, targetVelocity, engine.LerpAmount));
        }
    }
}