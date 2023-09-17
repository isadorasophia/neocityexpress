using Bang.Entities;
using Bang.StateMachines;
using Bang;
using Murder.Core.Graphics;
using Murder.Services;
using Murder.Core.Input;
using LDGame.Core;
using Murder;
using Murder.Assets;
using Murder.Attributes;
using Newtonsoft.Json;
using Murder.Core.Geometry;
using System.Diagnostics;
using LDGame.Core.Sounds;
using LDGame.Services;

namespace LDGame.StateMachines.Menu
{
    internal class PauseMenuStateMachine : StateMachine
    {
        [JsonProperty, GameAssetId(typeof(WorldAsset))]
        private readonly Guid _mainMenuWorld = Guid.Empty;

        private OptionsInfo _options =
            new OptionsInfo(options: new MenuOption[] { new("Resume"), new("Quit") });

        private MenuInfo _menuInfo = new();

        private float _previousFocusMusicValue = 0;
        private bool _hadCarLoop = false;

        public PauseMenuStateMachine()
        {
            State(StartPause);
        }

        private IEnumerator<Wait> StartPause()
        {
            World.Pause();

            Entity.SetCustomDraw(DrawPauseMenu);
            yield return GoTo(Main);
        }

        private IEnumerator<Wait> Main()
        {
            _previousFocusMusicValue = 
                LDGameSoundPlayer.Instance.GetGlobalParameterValue(LibraryServices.GetRoadLibrary().MusicFocusParameter) ?? 0;

            LDGameSoundPlayer.Instance.SetGlobalParameter(LibraryServices.GetRoadLibrary().MusicFocusParameter, 1);
            _hadCarLoop = LDGameSoundPlayer.Instance.Stop(LibraryServices.GetRoadLibrary().CarLoop, fadeOut: true);

            while (true)
            {
                if (Game.Input.VerticalMenu(ref _menuInfo, _options))
                {
                    LDGameSoundPlayer.Instance.PlayEvent(LibraryServices.GetRoadLibrary().UiConfirm, isLoop: false);

                    switch (_menuInfo.Selection)
                    {
                        case 0: //  Resume
                            World.Resume();
                            Entity.Destroy();

                            break;

                        case 1: //  Quit
                            LDGameSoundPlayer.Instance.Stop(fadeOut: true);

                            Game.Instance.QueueWorldTransition(_mainMenuWorld);
                            break;

                        default:
                            break;
                    }
                }
                else if (Game.Input.Pressed(InputButtons.Cancel))
                {
                    World.Resume();
                    Entity.Destroy();

                    break;
                }

                yield return Wait.NextFrame;
            }
        }

        private void DrawPauseMenu(RenderContext render)
        {
            Debug.Assert(_options.Options is not null);

            // BG fade
            RenderServices.DrawRectangle(render.UiBatch, new(0, 0, render.Camera.Width, render.Camera.Height), Palette.Colors[11] * 0.8f, .11f);

            Point cameraHalfSize = render.Camera.Size / 2f - new Point(0, _options.Length * 7);

            RenderServices.DrawVerticalMenu(render, cameraHalfSize, new Vector2(.5f, .5f), Game.Data.MediumFont, selectedColor: Palette.Colors[7],
                color: Palette.Colors[5], shadow: Palette.Colors[1], _menuInfo.Selection,
                out _, _options.Options);
        }

        public override void OnDestroyed()
        {
            base.OnDestroyed();

            LDGameSoundPlayer.Instance.SetGlobalParameter(LibraryServices.GetRoadLibrary().MusicFocusParameter, _previousFocusMusicValue);

            if (_hadCarLoop)
            {
                LDGameSoundPlayer.Instance.PlayEvent(LibraryServices.GetRoadLibrary().CarLoop, isLoop: true, stopLastMusic: false);
            }
        }
    }
}
