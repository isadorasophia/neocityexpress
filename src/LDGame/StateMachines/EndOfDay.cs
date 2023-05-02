using Bang;
using Bang.Components;
using Bang.Entities;
using Bang.StateMachines;
using LDGame.Assets;
using LDGame.Core;
using LDGame.Services;
using Murder;
using Murder.Assets;
using Murder.Attributes;
using Murder.Components;
using Murder.Core.Dialogs;
using Murder.Core.Geometry;
using Murder.Core.Graphics;
using Murder.Services;
using Newtonsoft.Json;

namespace LDGame.StateMachines
{
    internal class EndOfDayStateMachine : StateMachine
    {
        private string _message = string.Empty;

        [JsonProperty]
        private readonly SituationComponent _endOfDay = new();

        [JsonProperty]
        [GameAssetId<WorldAsset>]
        private readonly Guid _nextLevel = new();

        public EndOfDayStateMachine()
        {
            State(Main);
        }

        private IEnumerator<Wait> Main()
        {
            Entity.SetCustomDraw(DrawMessage);

            LdSaveData save = SaveServices.GetOrCreateSave();

            save.Day++;
            save.TraveledDistance = 0;

            SaveServices.QuickSave();

            Character? character = DialogServices.CreateCharacterFrom(_endOfDay.Character, _endOfDay.Situation);
            if (character is null)
            {
                yield return Wait.Stop;
            }

            yield return Wait.ForSeconds(.5f);

            while (character?.NextLine(World)?.Line?.Text is string message)
            {
                _message = message;

                yield return Wait.ForSeconds(3);
            }

            _message = string.Empty;

            LevelServices.SwitchScene(_nextLevel);
        }

        private void DrawMessage(RenderContext render)
        {
            var cameraHalfSize = render.Camera.Size / 2f;
            Game.Data.MediumFont.Draw(render.UiBatch, _message, 1, cameraHalfSize, Vector2.Center, 0, Palette.Colors[7]);
        }
    }
}
