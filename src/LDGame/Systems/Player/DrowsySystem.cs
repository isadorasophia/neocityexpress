using Bang.Contexts;
using Bang.Systems;
using LDGame.Components;
using LDGame.Core;
using LDGame.Core.Sounds;
using LDGame.Services;
using Murder;
using Murder.Components;
using Murder.Core.Geometry;
using Murder.Core.Graphics;
using Murder.Utilities;

namespace LDGame.Systems.Player
{
    public class DrowsySystem : IMonoRenderSystem, IFixedUpdateSystem
    {
        private float _currentSleep = 0;
        private Vector2[] _vertices;

        private Vector2 _lastCursorPosition = Vector2.Zero;

        public void Draw(RenderContext render, Context context)
        {
            if (_currentSleep <= 0)
                return;

            int divisions = 8;
            var screenSize = render.Camera.Size;
            var step = screenSize.X / (float)divisions * 0.67f;

            _vertices = new Vector2[4];
            var drawInfo = new DrawInfo() { Color = Color.Black };

            float eyeOpen = 100 * (1 - Calculator.Clamp01(_currentSleep));
            UpperLid(render, divisions, screenSize, step, _vertices, drawInfo, eyeOpen);
            LowerLid(render, divisions, screenSize, step, _vertices, drawInfo, eyeOpen);

            float alert = Calculator.Remap(Calculator.Clamp01(_currentSleep), 0.3f, 1, 0, 1f);
            if (alert > 0f)
            {
                Game.Data.PixelFont.Draw(render.UiBatch, "move your mouse to shake your head", 1, new Vector2(screenSize.X / 2, screenSize.Y - 50 * alert), Vector2.Center, 0.09f, Palette.Colors[19] * _currentSleep);
            }

            LDGameSoundPlayer.Instance.SetGlobalParameter(LibraryServices.GetRoadLibrary().EyesClosedParameter, eyeOpen);
        }

        private static void UpperLid(RenderContext render, int divisions, Point screenSize, float step, Vector2[] vertices, DrawInfo drawInfo, float eyeOpen)
        {
            for (int i = 0; i < divisions; i++)
            {
                var delta = Ease.CubeOut(i / (float)divisions);
                var deltaNext = Ease.CubeOut((i + 1) / (float)divisions);

                vertices[0] = new Vector2(i * step, 0);
                vertices[1] = new Vector2((i + 1) * step, 0);
                vertices[2] = new Vector2((i + 1) * step, screenSize.Y / 2f - deltaNext * eyeOpen - eyeOpen*2);
                vertices[3] = new Vector2(i * step, screenSize.Y / 2f - delta * eyeOpen - eyeOpen*2);

                render.UiBatch.DrawPolygon(SharedResources.GetOrCreatePixel(render.UiBatch), vertices, drawInfo);

                int j = i + divisions / 2 + 1;
                delta = 1 - Ease.CubeIn(i / (float)divisions);
                deltaNext = 1 - Ease.CubeIn((i + 1) / (float)divisions);
                vertices[0] = new Vector2(j * step, 0);
                vertices[1] = new Vector2((j + 1) * step, 0);
                vertices[2] = new Vector2((j + 1) * step, screenSize.Y / 2f - deltaNext * eyeOpen - eyeOpen * 2);
                vertices[3] = new Vector2(j * step, screenSize.Y / 2f - delta * eyeOpen - eyeOpen * 2);

                render.UiBatch.DrawPolygon(SharedResources.GetOrCreatePixel(render.UiBatch), vertices, drawInfo);
            }
        }
        private static void LowerLid(RenderContext render, int divisions, Point screenSize, float step, Vector2[] vertices, DrawInfo drawInfo, float eyeOpen)
        {
            for (int i = 0; i < divisions; i++)
            {
                var delta = Ease.CubeOut(i / (float)divisions);
                var deltaNext = Ease.CubeOut((i + 1) / (float)divisions);

                vertices[0] = new Vector2(i * step, screenSize.Y);
                vertices[1] = new Vector2((i + 1) * step, screenSize.Y);
                vertices[2] = new Vector2((i + 1) * step, screenSize.Y / 2f + deltaNext * eyeOpen + eyeOpen * 2);
                vertices[3] = new Vector2(i * step, screenSize.Y / 2f+ delta * eyeOpen + eyeOpen * 2);

                render.UiBatch.DrawPolygon(SharedResources.GetOrCreatePixel(render.UiBatch), vertices, drawInfo);

                int j = i + divisions / 2 + 1;
                delta = 1 - Ease.CubeIn(i / (float)divisions);
                deltaNext = 1 - Ease.CubeIn((i + 1) / (float)divisions);
                vertices[0] = new Vector2(j * step, screenSize.Y);
                vertices[1] = new Vector2((j + 1) * step, screenSize.Y);
                vertices[2] = new Vector2((j + 1) * step, screenSize.Y / 2f+ deltaNext * eyeOpen + eyeOpen * 2);
                vertices[3] = new Vector2(j * step, screenSize.Y / 2f + delta * eyeOpen + eyeOpen * 2);

                render.UiBatch.DrawPolygon(SharedResources.GetOrCreatePixel(render.UiBatch), vertices, drawInfo);
            }
        }

        public void FixedUpdate(Context context)
        {
            if (context.World.GetEntitiesWith(typeof(FreezeWorldComponent)).Length != 0)
            {
                // we are not really running anymore!
                return;
            }

            var delta = (Game.Input.CursorPosition - _lastCursorPosition).Manhattan();

            if (delta > 5)
            {
                _currentSleep -= Game.FixedDeltaTime * 0.5f;
            }
            else
            {
                _currentSleep += Game.FixedDeltaTime * 0.1f;
            }

            _currentSleep = Math.Clamp(_currentSleep, -1, 1);

            _lastCursorPosition = Game.Input.CursorPosition;
        }
    }
}