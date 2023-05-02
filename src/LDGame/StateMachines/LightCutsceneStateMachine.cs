using Bang.Entities;
using Bang.StateMachines;
using LDGame.Assets;
using LDGame.Core;
using LDGame.Core.Sounds;
using LDGame.Services;
using Murder;
using Murder.Assets;
using Murder.Attributes;
using Murder.Components;
using Murder.Core;
using Murder.Core.Dialogs;
using Murder.Core.Geometry;
using Murder.Core.Graphics;
using Murder.Services;
using Murder.Utilities;
using Newtonsoft.Json;

namespace LDGame.StateMachines
{
    internal class LightCutsceneStateMachine : StateMachine
    {
        private string _message = string.Empty;

        [JsonProperty]
        private SituationComponent? _situation = null;

        private Portrait? _portrait = null;

        [JsonProperty]
        [GameAssetId<WorldAsset>]
        private readonly Guid _nextLevel = new();

        [JsonProperty]
        [Tooltip("Whether it should save and keep progress of the game.")]
        private bool _progressGame = true;

        [JsonProperty]
        private bool _fetchContentFromSave = true;

        public LightCutsceneStateMachine()
        {
            State(Main);
        }

        private IEnumerator<Wait> Main()
        {
            SituationComponent? situation;

            LdSaveData save = SaveServices.GetOrCreateSave();
            if (!_fetchContentFromSave && _situation is not null)
            {
                // use the default value
                situation = _situation;
            }
            else
            {
                situation = save.NextLevelCutscene ?? _situation;
            }

            if (situation is null)
            {
                yield break;
            }

            LDGameSoundPlayer.Instance.Stop(fadeOut: true);

            if (!_progressGame)
            {
                // Hospital Game over screen
                LDGameSoundPlayer.Instance.PlayEvent(LibraryServices.GetRoadLibrary().GameOverCrash, isLoop: false);

                yield return Wait.ForSeconds(3.5f);

                LDGameSoundPlayer.Instance.PlayEvent(LibraryServices.GetRoadLibrary().HospitalMusic, isLoop: true);
                LDGameSoundPlayer.Instance.PlayEvent(LibraryServices.GetRoadLibrary().HeartMonitor, isLoop: true, stopLastMusic: false);
            }

            bool isTrueEnding = save.GameplayBlackboard.TrueEndingUnlocked;
            if (_progressGame && !isTrueEnding)
            {
                save.Day++;
                save.TraveledDistance = 0;

                SaveServices.QuickSave();
            }

            Entity.SetCustomDraw(DrawMessage);

            Character? character = DialogServices.CreateCharacterFrom(situation.Value.Character, situation.Value.Situation);
            if (character is null)
            {
                yield return Wait.Stop;
            }

            yield return Wait.ForSeconds(.5f);

            while (character?.NextLine(World)?.Line is Line line)
            {
                _message = line.Text ?? string.Empty;

                if (AssetHelpers.GetPortraitForLine(line) is Portrait target)
                {
                    _portrait = target;
                }

                while (!Game.Input.PressedAndConsume(InputButtons.Submit) && !Game.Input.PressedAndConsume(InputButtons.Interact))
                {
                    yield return Wait.NextFrame;
                }
            }

            _message = string.Empty;
            _portrait = null;

            yield return Wait.ForSeconds(.5f);

            // Fetch the value again after finishing executing the dialog.
            isTrueEnding = save.GameplayBlackboard.TrueEndingUnlocked;
            if (isTrueEnding)
            {
                LevelServices.SwitchScene(LibraryServices.GetRoadLibrary().TrueEnding);
            }
            else
            {
                LevelServices.SwitchScene(_nextLevel);
            }
        }

        private void DrawMessage(RenderContext render)
        {
            Vector2 position = render.Camera.Size / 2f;

            // Draw(Batch2D spriteBatch, string text, Vector2 position, float sort, Color color, Color? strokeColor, Color? shadowColor, int maxWidth)
            // TODO: Break lines.
            Game.Data.MediumFont.Draw(
                render.UiBatch,
                _message,
                position + new Vector2(0, 100),
                alignment: Vector2.Center,
                sort: 0.21f,
                color: Palette.Colors[7],
                strokeColor: null,
                shadowColor: null,
                maxWidth: 400
            );

            if (_portrait is not null)
            {
                RenderServices.DrawSprite(
                    render.UiBatch,
                    _portrait.Value.Aseprite,
                    position - new Vector2(0, 10),
                    _portrait.Value.AnimationId ?? string.Empty,
                    0,
                    new DrawInfo { Origin = Vector2.Center, Sort = .24f });
            }
        }

        public override void OnDestroyed()
        {
            LDGameSoundPlayer.Instance.Stop(fadeOut: true);
        }
    }
}
