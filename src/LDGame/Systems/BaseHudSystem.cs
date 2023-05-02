using Bang.Contexts;
using LDGame.Services;
using Murder.Core.Graphics;
using Murder.Services;

namespace LDGame.Systems
{
    internal class BaseHudSystem : IMonoRenderSystem
    {
        public void Draw(RenderContext render, Context context)
        {
            var skin = LibraryServices.GetUiSkin();

            RenderServices.DrawSprite(render.UiBatch, skin.LeftHudBase, 0, 0, "", new DrawInfo(0.984f));
            RenderServices.DrawSprite(render.UiBatch, skin.RightHudBase, render.Camera.Width, 0, "", new DrawInfo(0.984f));

            var save = SaveServices.GetOrCreateSave();
            RenderServices.DrawSprite(render.UiBatch, skin.HealthBar, -12, render.Camera.Height + 8, Math.Clamp(save.Health, 0, 5).ToString(), new DrawInfo(0.983f));
        }
    }
}