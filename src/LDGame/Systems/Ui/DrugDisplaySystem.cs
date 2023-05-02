using Bang.Contexts;
using LDGame.Assets;
using LDGame.Components;
using LDGame.Core;
using LDGame.Services;
using Murder;
using Murder.Core.Geometry;
using Murder.Core.Graphics;
using Murder.Services;
using Murder.Utilities;

namespace LDGame.Systems;

public class DrugDisplaySystem : IMonoRenderSystem
{
    float _time = -1;
    float _currentYOffset = 0;
    float _offsetStarTime = 0;
    public void Draw(RenderContext render, Context context)
    {
        if (context.World.GetEntitiesWith(typeof(CellphoneLineComponent)).Any() )
        {
            if (_currentYOffset !=0) {
                _offsetStarTime = Game.Now;
            }
            _currentYOffset = 0;
        }
        else
        {
            if (_currentYOffset != 1)
            {
                _offsetStarTime = Game.Now;
            }
            _currentYOffset = 1;
        }

        var skin = LibraryServices.GetUiSkin();
        var save = SaveServices.GetOrCreateSave();
        float sway = save.SwayDirection.X;
        if (!save.HasSway || !save.GameplayBlackboard.HyperXEnabled)
            return;

        if (_time == -1)
            _time = Game.Now;

        var delta = 1 - Ease.BackOut(Calculator.ClampTime(Game.Now - _time, 1.9f));

        float offset = (_currentYOffset - Ease.BackInOut(Calculator.ClampTime(Game.Now - _offsetStarTime , 0.8f)) ) * 195;
        var position = new Vector2(render.Camera.Width - 90 + 180 * delta, 200 + offset);
        RenderServices.DrawSprite(render.UiBatch, skin.DrugSlider, position.X, position.Y, "", new DrawInfo(0.818f));

        float pointer = sway * 120;
        RenderServices.DrawSprite(render.UiBatch, skin.DrugPointer, position.X + pointer, position.Y + 12, "", new DrawInfo(0.81f));

        Game.Data.PixelFont.Draw(render.UiBatch, "DIRECTION SWAY DETECTED", 1,
            position + new Vector2(0, 40), Vector2.Center, 0.8f, Palette.Colors[14]);

        if (Game.Input.Down(InputButtons.Space))
        {
            RenderServices.Draw9Slice(render.UiBatch, skin.DrugButtonDown, new Rectangle(position.X - 60, position.Y + 44, 120, 42), new DrawInfo(0.875f));
            Game.Data.PixelFont.Draw(render.UiBatch, "hold SPACE to", 1, position + new Vector2(0, 55 + 3), Vector2.Center, 0.87f, Palette.Colors[4]);
            Game.Data.PixelFont.Draw(render.UiBatch, "inject k-HyperX", 1, position + new Vector2(0, 65 + 3), Vector2.Center, 0.87f, Palette.Colors[4]);
        }
        else
        {
            RenderServices.Draw9Slice(render.UiBatch, skin.DrugButton, new Rectangle(position.X - 60, position.Y + 44, 120, 42), new DrawInfo(0.875f));
            Game.Data.PixelFont.Draw(render.UiBatch, "hold SPACE to", 1, position + new Vector2(0, 55), Vector2.Center, 0.87f, Palette.Colors[5]);
            Game.Data.PixelFont.Draw(render.UiBatch, "inject k-HyperX", 1, position + new Vector2(0, 65), Vector2.Center, 0.87f, Palette.Colors[5]);
        }
    }
}