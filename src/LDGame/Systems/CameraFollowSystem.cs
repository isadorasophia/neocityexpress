using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using Murder.Core.Graphics;
using Murder.Core.Geometry;
using Murder.Core;
using Murder.Components;
using Murder.Utilities;
using Murder;
using LDGame.Components;

namespace LDGame.Systems
{
    [Filter(kind: ContextAccessorKind.Read, typeof(CameraFollowComponent), typeof(IMurderTransformComponent))]
    internal class CameraFollowSystem : IFixedUpdateSystem
    {
        bool _firstStarted = false;

        public void FixedUpdate(Context context)
        {
            if (!context.HasAnyEntity || context.Entity is not Entity cameraman)
            {
                // Unable to find any entity to follow? Do not update the camera position in that case.
                return;
            }

            var cameraFollow = context.Entity.GetCameraFollow();
            if (!cameraFollow.Enabled)
                return;

            Camera2D camera = ((MonoWorld)context.World).Camera;

            Vector2 cameramanPosition = cameraman.GetGlobalTransform().Vector2;

            Entity? trackedEntity;
            if (context.Entity.HasIdTarget())
            {
                trackedEntity = context.World.TryGetEntity(context.Entity.GetIdTarget().Target);
            }
            else
            {
                trackedEntity = context.World.TryGetUniqueEntity<PlayerComponent>();
            }

            if (trackedEntity is not null)
            {
                Vector2 targetPosition = trackedEntity.GetGlobalTransform().Vector2;

                if (cameraFollow.SecondaryTarget is not null)
                {
                    if (cameraFollow.SecondaryTarget.IsDestroyed)
                    {
                        cameraFollow = new CameraFollowComponent(true);
                        context.Entity.SetCameraFollow(cameraFollow);
                    }
                    else
                    { 
                        targetPosition = Vector2.Lerp(targetPosition, cameraFollow.SecondaryTarget.GetGlobalTransform().Vector2, 0.35f);
                    }
                }
                
                if (!_firstStarted)
                {
                    _firstStarted = true;
                    cameramanPosition = targetPosition;
                    cameraman.SetGlobalTransform(new PositionComponent(cameramanPosition));
                }
                else
                {
                    if (cameraFollow.Style == CameraStyle.Perfect)
                    {
                        cameraman.SetTransform(new PositionComponent(targetPosition));
                        cameramanPosition = targetPosition;
                    }
                    else
                    {
                        Point deadzone = cameraFollow.Style == CameraStyle.DeadZone ? new(24, 24) : Point.Zero;
                        var delta = (cameramanPosition - targetPosition);
                        float lerpAmount = 1 - MathF.Pow(0.1f, Game.FixedDeltaTime);
                        float lerpedX = cameramanPosition.X;
                        float lerpedY = cameramanPosition.Y;
                        if (MathF.Abs(delta.X) > deadzone.X)
                        {
                            lerpedX = Calculator.LerpSnap(cameramanPosition.X, targetPosition.X + deadzone.X * MathF.Sign(delta.X), lerpAmount, 0.2f);
                        }
                        if (MathF.Abs(delta.Y) > deadzone.Y)
                        {
                            lerpedY = Calculator.LerpSnap(cameramanPosition.Y, targetPosition.Y + deadzone.Y * MathF.Sign(delta.Y), lerpAmount, 0.2f);
                        }
                        cameramanPosition = new Vector2(lerpedX, lerpedY);
                        cameraman.SetTransform(new PositionComponent(cameramanPosition));
                    }
                }
            }

            Vector2 finalPosition = cameramanPosition - new Vector2(camera.Width, camera.Height) / 2f;

            // Make sure that the camera stay in the dungeon limits.
            if (context.World.TryGetUnique<MapComponent>() is MapComponent map && map.Map != null)
            {
                finalPosition = ClampBounds(map.Width, map.Height, camera, finalPosition);
            }

            camera.Position = finalPosition;
        }

        private Vector2 ClampBounds(int width, int height, Camera2D camera, Vector2 position)
        {
            if (position.X < 0) position.X = 0;
            if (position.Y < 0) position.Y = 0;

            var maxWidth = width * Grid.CellSize;
            var maxHeight = height * Grid.CellSize;

            if (position.X + camera.Bounds.Width > maxWidth) position.X = maxWidth - camera.Bounds.Width;
            if (position.Y + camera.Bounds.Height > maxHeight) position.Y = maxHeight - camera.Bounds.Height;

            return position;
        }
    }
}
