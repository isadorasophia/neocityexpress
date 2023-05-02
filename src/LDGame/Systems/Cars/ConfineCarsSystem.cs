using Bang.Components;
using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using LDGame.Components;
using LDGame.Services;
using Murder;
using Murder.Core.Geometry;
using Murder.Utilities;

namespace LDGame.Systems;

[Filter(typeof(CarComponent),typeof(ITransformComponent))]
public class ConfineCarsSystem : IFixedUpdateSystem
{
    public void FixedUpdate(Context context)
    {
        foreach (var e in context.Entities)
        {
            var position = e.GetGlobalTransform().Vector2;

            var bounds = LibraryServices.GetRoadLibrary().Bounds;
            (int minX, int maxX) = (bounds.Left, bounds.Right);
            (int minY, int maxY) = (bounds.Top, bounds.Bottom);

            //            (int minX, int maxX) = (220, 450);
            //            (int minY, int maxY) = (50, 345);

            if (e.HasPlayer())
            {
                var currentVelocity = e.TryGetVelocity()?.Velocity ?? Vector2.Zero;
                if (position.Y < minY || position.Y > maxY)
                {
                    currentVelocity.Y = 0;
                }

                e.SetGlobalPosition(new Vector2(
                    Math.Clamp(position.X, minX, maxX),
                    Math.Clamp(position.Y, minY, maxY)
                    ));

                e.SetVelocity(currentVelocity);
            }
            else if (e.HasEnemy())
            {
                // enemy, explode on sides
                var currentVelocity = e.TryGetRelativeVelocity()?.Velocity ?? Vector2.Zero;
                if (position.X < minX || position.X > maxX)
                {
                    currentVelocity.X = -currentVelocity.X;
                    if (Math.Abs(currentVelocity.X) < 2f)
                    {
                        if (position.X < minX)
                            currentVelocity.X = 20;
                        else
                            currentVelocity.X = -20;
                    }
                    LibraryServices.Explode(1, context.World, e.GetGlobalTransform().Vector2);
                }

                // enemy removes on top, screw the GC for now [PERF]
                if (position.Y<minY - 190 || position.Y > maxY + 190)
                {
                    e.Destroy();
                }
                
                e.SetRelativeVelocity(currentVelocity);
            }
        }
    }
}