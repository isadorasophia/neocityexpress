using Bang.Entities;
using Bang.StateMachines;
using LDGame.Assets;
using LDGame.Core;
using LDGame.Services;
using Murder;
using Murder.Assets;
using Murder.Assets.Graphics;
using Murder.Attributes;
using Murder.Core.Geometry;
using Murder.Core.Graphics;
using Murder.Services;
using Newtonsoft.Json;

namespace LDGame.StateMachines
{
    internal class SlideShowStateMachine : StateMachine
    {
        private bool _showFmod = true;
        private bool _showCityLogo = false;

        [JsonProperty]
        [GameAssetId<WorldAsset>]
        private Guid _nextWorld = Guid.Empty;

        public SlideShowStateMachine()
        {
            State(Main);
        }

        private float _lastTransition = 0;

        private IEnumerator<Wait> Main()
        {
            Entity.SetCustomDraw(Draw);

            Entity.SetFadeScreen(Murder.Components.FadeType.Out, Game.Now, .5f, Palette.Colors[24], destroyAfterFinished: false, customTexture: string.Empty /* unused */);

            int duration = 2;

            _lastTransition = Game.Now;
            while (Game.Now - _lastTransition < duration && !Game.Input.PressedAndConsume(InputButtons.Submit) && !Game.Input.PressedAndConsume(InputButtons.Interact))
            {
                yield return Wait.NextFrame;
            }

            Entity.SetFadeScreen(Murder.Components.FadeType.In, Game.Now, .5f, Palette.Colors[24], destroyAfterFinished: false, customTexture: string.Empty /* unused */);
            yield return Wait.ForSeconds(.5f);

            _showFmod = false;
            _showCityLogo = true;

            Entity.SetFadeScreen(Murder.Components.FadeType.Out, Game.Now, .5f, Palette.Colors[24], destroyAfterFinished: false, customTexture: string.Empty /* unused */);

            _lastTransition = Game.Now;
            while (Game.Now - _lastTransition < duration && !Game.Input.PressedAndConsume(InputButtons.Submit) && !Game.Input.PressedAndConsume(InputButtons.Interact))
            {
                yield return Wait.NextFrame;
            }

            Entity.SetFadeScreen(Murder.Components.FadeType.In, Game.Now, .5f, Palette.Colors[24], destroyAfterFinished: false, customTexture: string.Empty /* unused */);
            yield return Wait.ForSeconds(1);

            LevelServices.SwitchScene(_nextWorld);
        }

        private void Draw(RenderContext render)
        {
            LibraryAsset library = LibraryServices.GetRoadLibrary();

            Vector2 position = new Vector2(74, 0);
            if (_showFmod)
            {
                if (DialogueServices.GetSpriteAssetForSituation(library.FmodImage) is (SpriteAsset asset, string animation))
                {
                    RenderServices.DrawSprite(
                        render.UiBatch,
                            position,
                            0,
                            animation,
                            asset,
                            0,
                            Color.White,
                            .3f,
                            false);
                }
            }
            else if (_showCityLogo)
            {
                if (DialogueServices.GetSpriteAssetForSituation(library.CityImage) is (SpriteAsset asset, string animation))
                {
                    RenderServices.DrawSprite(
                        render.UiBatch,
                            position,
                            0,
                            animation,
                            asset,
                            0,
                            Color.White,
                            .3f,
                            false);
                }
            }
        }
    }
}
