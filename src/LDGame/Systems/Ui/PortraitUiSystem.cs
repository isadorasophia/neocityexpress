using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using LDGame.Components;
using Murder.Assets.Graphics;
using Murder.Core;
using Murder.Core.Graphics;
using Murder.Services;
using Murder;
using Murder.Core.Geometry;
using LDGame.Services;

namespace LDGame.Systems.Ui
{
    [Filter(ContextAccessorFilter.AnyOf, typeof(GuestPortraitComponent), typeof(DriverPortraitComponent))]
    internal class PortraitUiSystem : IMonoRenderSystem
    {
        public void Draw(RenderContext render, Context context)
        {
            foreach (Entity e in context.Entities)
            {
                if (e.HasGuestPortrait())
                {
                    DrawRight(render, e.GetGuestPortrait().Portrait, e.HasPaused());
                }
                else if (e.HasDriverPortrait())
                {
                    DrawLeft(render, e.GetDriverPortrait().Portrait, e.HasPaused());
                }
            }
        }

        private void DrawLeft(RenderContext render, Portrait portrait, bool paused)
        {
            if (DialogueServices.GetSpriteAssetForSituation(portrait) is (SpriteAsset asset, string animation))
            {
                RenderServices.DrawSprite(
                    render.UiBatch,
                        new Vector2(8, 4),
                        0,
                        animation,
                        asset,
                        0,
                        Color.White,
                        paused? 0.3f : 0.90f,
                        false);
            }
        }

        private void DrawRight(RenderContext render, Portrait portrait, bool paused)
        {
            bool showGuest = SaveServices.GetOrCreateSave().GameplayBlackboard.ShowGranny;
            if (showGuest && DialogueServices.GetSpriteAssetForSituation(portrait) is (SpriteAsset asset, string animation))
            {
                RenderServices.DrawSprite(
                    render.UiBatch,
                        new Vector2(render.Camera.Width - 170, 4),
                        0,
                        animation,
                        asset,
                        0,
                        Color.White,
                        paused ? 0.3f : 0.90f,
                        false);
            }
        }
    }
}
