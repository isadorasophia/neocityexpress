using Bang.Components;
using Bang.Contexts;
using Bang.Systems;
using LDGame.Components;
using LDGame.Services;
using Murder;
using Murder.Assets.Graphics;
using Murder.Components;
using Murder.Core.Geometry;
using Murder.Core.Graphics;
using Murder.Services;
using Murder.Utilities;

namespace LDGame.Systems
{
    [Filter(typeof(ScrollingSprite), typeof(ITransformComponent))]
    internal class ScrollingImageSystem : IMonoRenderSystem, IUpdateSystem
    {
        float _distance = 0;
        public void Draw(RenderContext render, Context context)
        {
            foreach (var e in context.Entities)
            {
                var position = e.GetGlobalTransform().Vector2;
                var scroll = e.GetComponent<ScrollingSprite>();

                if (Game.Data.TryGetAsset<SpriteAsset>(scroll.Sprite) is SpriteAsset sprite)
                {
                    var size = sprite.Size;
                    var totalSize = size * scroll.Size;
                    var batch = render.GetSpriteBatch(scroll.targetSpriteBatch);


                    Vector2 offset = new Vector2((Game.Now * scroll.Speed.X) % size.X, (_distance*100 * scroll.Speed.Y) % size.Y);
                    
                    for (int x = 0; x < scroll.Size.X; x++)
                    {
                        for (int y = 0; y < scroll.Size.Y; y++)
                        {
                            RenderServices.DrawSprite(batch, scroll.Sprite, position + size * new Point(x, y) - totalSize / 2f + offset, "base", 0, new DrawInfo(scroll.Sort)
                            {
                                FlippedHorizontal = scroll.Flip
                            });
                        }
                    }
                }
            }
        }

        public void Update(Context context)
        {
            var playerSpeed = context.World.TryGetUnique<PlayerSpeedComponent>()?.Speed ?? 0;
            _distance += Game.DeltaTime * playerSpeed;
        }
    }
}