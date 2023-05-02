using Bang.Contexts;
using Bang.Systems;
using LDGame.Assets;
using LDGame.Components;
using LDGame.Core;
using LDGame.Services;
using Murder;
using Murder.Core.Geometry;
using Murder.Core.Graphics;
using Murder.Services;
using Murder.Utilities;

namespace LDGame.Systems.Ui
{
    [Filter(typeof(PlayerComponent))]
    internal class CarProgressUiSystem : IMonoRenderSystem
    {
        public void Draw(RenderContext render, Context context)
        {
            if (DayCycle.TryGetCurrentDay(context.World) is DayCycle day) {
                Vector2 position = new Vector2(0, 136);
                
                var skin = LibraryServices.GetUiSkin();
                RenderServices.DrawSprite(render.UiBatch, skin.MapLine, position, "", 0, new DrawInfo(0.8f));

                // Updated in CarProgressSystem.
                LdSaveData save = SaveServices.GetOrCreateSave();
                float progress = Calculator.Clamp01(save.TraveledDistance / day.Distance);

                float xProgress = 118 * progress;

                RenderServices.DrawSprite(render.UiBatch, skin.MapPoint, position + new Vector2(12 + xProgress,0), "", 0, new DrawInfo(0.78f));

                if (progress >= 1)
                {
                    Game.Data.PixelFont.Draw(render.UiBatch, "ALMOST THERE!!!", 1, position + new Vector2(40, 5 + MathF.Sin(Game.Now*5)*5),
                        0.7f, Palette.Colors[Calculator.Blink(10,true)?5:6], Palette.Colors[1]);
                }
            } }
    }
}
