using Bang.Entities;
using Bang.StateMachines;
using Bang.Systems;
using LDGame.Core;
using LDGame.Core.Sounds;
using LDGame.Services;
using Murder;
using Murder.Assets;
using Murder.Attributes;
using Murder.Core.Geometry;
using Murder.Core.Graphics;
using Murder.Core.Input;
using Murder.Services;
using Newtonsoft.Json;
using System.Diagnostics;

namespace LDGame.StateMachines
{
    internal class MainMenuStateMachine : StateMachine
    {
        [JsonProperty, GameAssetId(typeof(WorldAsset))]
        private readonly Guid _newGameWorld = Guid.Empty;

        [JsonProperty, GameAssetId(typeof(WorldAsset))]
        private readonly Guid _continueWorld = Guid.Empty;

        private MenuInfo _menuInfo = new();
        private OptionsInfo _optionsInfo = new();

        private OptionsInfo GetMainMenuOptions() =>
            new OptionsInfo(options: new MenuOption[] { new("Continue", selectable: MurderSaveServices.CanLoadSave()), new("New Game"), new("Options"), new("Credits"), new("Exit") });

        private OptionsInfo GetOptionOptions() =>
            new OptionsInfo(options: new MenuOption[] {
                new(Game.Preferences.SoundVolume == 1 ? "Sounds on" : "Sounds off"),
                new("Back to menu") });

        public MainMenuStateMachine()
        {
            State(Main);
        }

        protected override void OnStart()
        {
            Entity.SetCustomDraw(DrawMainMenu);

            _menuInfo.Selection = MurderSaveServices.CanLoadSave() ? 0 : 1;
        }

        private IEnumerator<Wait> Main()
        {
            _onCredits = false;

            _optionsInfo = GetMainMenuOptions();
            _menuInfo.Selection = _optionsInfo.NextAvailableOption(-1, 1);

            // Update whatever preferences we previously had.
            Game.Preferences.OnPreferencesChanged();

            while (true)
            {
                int previousInput = _menuInfo.Selection;

                if (Game.Input.VerticalMenu(ref _menuInfo, _optionsInfo))
                {
                    LDGameSoundPlayer.Instance.PlayEvent(LibraryServices.GetRoadLibrary().UiConfirm, isLoop: false);

                    switch (_menuInfo.Selection)
                    {
                        case 0: //  Continue Game
                            EffectsServices.FadeIn(World, .5f, Palette.Colors[1], false);
                            // yield return Wait.ForSeconds(.5f);

                            Game.Instance.QueueWorldTransition(_continueWorld);

                            break;

                        case 1: //  New Game
                            Game.Data.DeleteAllSaves();

                            EffectsServices.FadeIn(World, .5f, Palette.Colors[1], false);
                            // yield return Wait.ForSeconds(.5f);

                            Game.Instance.QueueWorldTransition(_newGameWorld);
                            break;

                        case 2: // Options
                            yield return GoTo(Options);
                            break;

                        case 3: // Credits
                            yield return GoTo(Credits);
                            break;

                        case 4: //  Exit
                            Game.Instance.ExitGame();
                            break;

                        default:
                            break;
                    }
                }

                if (previousInput != _menuInfo.Selection)
                {
                    LDGameSoundPlayer.Instance.PlayEvent(LibraryServices.GetRoadLibrary().UiHover, isLoop: false);
                }

                yield return Wait.NextFrame;
            }
        }
        
        private IEnumerator<Wait> Options()
        {
            _optionsInfo = GetOptionOptions();
            _menuInfo.Selection = _optionsInfo.NextAvailableOption(-1, 1);

            Debug.Assert(_optionsInfo.Options is not null);

            while (true)
            {
                int previousInput = _menuInfo.Selection;

                if (Game.Input.VerticalMenu(ref _menuInfo, _optionsInfo))
                {
                    LDGameSoundPlayer.Instance.PlayEvent(LibraryServices.GetRoadLibrary().UiConfirm, isLoop: false);

                    switch (_menuInfo.Selection)
                    {
                        case 0: // Tweak sound
                            float volume = Game.Preferences.ToggleMusicVolumeAndSave();

                            _optionsInfo.Options[0] = volume == 1 ? new("Sounds on") : new("Sounds off");
                            break;

                        case 1: // Go back
                            yield return GoTo(Main);
                            break;
                        
                        default:
                            break;
                    }
                }

                if (previousInput != _menuInfo.Selection)
                {
                    LDGameSoundPlayer.Instance.PlayEvent(LibraryServices.GetRoadLibrary().UiHover, isLoop: false);
                }

                yield return Wait.NextFrame;
            }
        }

        private bool _onCredits = false;

        private IEnumerator<Wait> Credits()
        {
            _onCredits = true;

            while (!Game.Input.PressedAndConsume(InputButtons.Submit) && 
                !Game.Input.PressedAndConsume(InputButtons.Interact) && 
                !Game.Input.PressedAndConsume(InputButtons.Esc))
            {
                yield return Wait.NextFrame;
            }

            yield return GoTo(Main);
        }

        private void DrawMainMenu(RenderContext render)
        {
            Debug.Assert(_optionsInfo.Options is not null);

            if (!_onCredits)
            {
                Point cameraHalfSize = render.Camera.Size / 2f - new Point(0, _optionsInfo.Length * 7);
                RenderServices.DrawRectangle(render.UiBatch, new Rectangle(0, 0, render.Camera.Width, render.Camera.Height), Palette.Colors[24], 1f);
                RenderServices.DrawVerticalMenu(render, cameraHalfSize + new Point(0, 100), new Vector2(.5f, .5f), Game.Data.MediumFont, selectedColor: Palette.Colors[17],
                    color: Palette.Colors[15], shadow: Palette.Colors[13], _menuInfo.Selection,
                    out _, _optionsInfo.Options);
            }
            else
            {
                int width = render.Camera.Width;

                string credits =
@"Pedro Medeiros (@saint11)
Isadora Rodopoulos (@isainstars)
Davey Wreden (@HelloCakebread)
Ryan Roth (@DualRyan)";

                Point cameraHalfSize = render.Camera.Size / 2f - new Point(0, _optionsInfo.Length * 7);
                Game.Data.MediumFont.Draw(render.GameUiBatch, credits, cameraHalfSize + new Point(0, 120), new Vector2(.5f, 0), 
                    sort: .5f, Palette.Colors[20], null, null, width - 350, doLineWrapping: false);

                Game.Data.MediumFont.Draw(render.GameUiBatch, "Back to menu", cameraHalfSize + new Point(0, 168), new Vector2(.5f, 0),
                    sort: .5f, Palette.Colors[17], null, shadowColor: Palette.Colors[13], width - 350, doLineWrapping: false);
            }
            
            var skin = LibraryServices.GetUiSkin();

            RenderServices.DrawSprite(render.UiBatch, skin.Logo, render.Camera.Size / 2f + new Vector2(0, 10 - 10 * MathF.Sin(Game.Now)), "",0, new DrawInfo(0.8f)
            {
                Origin = new Vector2(.5f, .5f)
            });
        }
    }
}
