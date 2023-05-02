using Bang.Entities;
using Bang.StateMachines;
using LDGame.Assets;
using LDGame.Core;
using LDGame.Services;
using Murder;
using Murder.Assets;
using Murder.Attributes;
using Murder.Core.Geometry;
using Murder.Core.Graphics;
using Murder.Services;
using Newtonsoft.Json;
using System.Diagnostics;

namespace LDGame.StateMachines
{
    internal class DeliveryStatsStateMachine : StateMachine
    {
        [JsonProperty]
        private DeliveryStats? _stats = null;

        [JsonProperty]
        [GameAssetId<WorldAsset>]
        private readonly Guid _nextLevel = new();

        private bool _isShowing = false;

        public DeliveryStatsStateMachine()
        {
            State(Main);
        }

        private IEnumerator<Wait> Main()
        {
            LdSaveData save = SaveServices.GetOrCreateSave();

            _isShowing = true;

            // Attempt to get the content of this delivery.
            if (save.NextLevel is Guid nextLevelGuid && 
                Game.Data.TryGetAsset<DayCycleAsset>(nextLevelGuid) is DayCycleAsset dayCycle)
            {
                _stats = dayCycle.DeliveryContent;
            }
            else
            {
                LibraryAsset library = LibraryServices.GetRoadLibrary();
                if (Game.Data.TryGetAsset<DayCycleAsset>(library.FirstDay) is DayCycleAsset firstDay)
                {
                    _stats = firstDay.DeliveryContent;
                }
            }

            if (_stats is null)
            {
                yield break;
            }

            Entity.SetCustomDraw(DrawMessage);

            yield return Wait.ForSeconds(.5f);

            while (!Game.Input.PressedAndConsume(InputButtons.Submit) && !Game.Input.PressedAndConsume(InputButtons.Interact))
            {
                yield return Wait.NextFrame;
            }

            _isShowing = false;
            yield return Wait.ForSeconds(.5f);

            LevelServices.SwitchScene(_nextLevel);
        }

        private void DrawMessage(RenderContext render)
        {
            Debug.Assert(_stats is not null);

            if (!_isShowing)
            {
                return;
            }

            Vector2 position = render.Camera.Size / 2f;

            int leftText = 200;
            int rightText = 60;

            Game.Data.MediumFont.Draw(
                render.UiBatch,
                "Day:",
                position - new Vector2(leftText, 25),
                alignment: Vector2.Zero,
                sort: 0.21f,
                color: Palette.Colors[6],
                strokeColor: null,
                shadowColor: Palette.Colors[12],
                maxWidth: 400
            );

            Game.Data.MediumFont.Draw(
                render.UiBatch,
                "Package being delivered:",
                position - new Vector2(leftText, 5),
                alignment: Vector2.Zero,
                sort: 0.21f,
                color: Palette.Colors[6],
                strokeColor: null,
                shadowColor: Palette.Colors[12],
                maxWidth: 400
            );

            Game.Data.MediumFont.Draw(
                render.UiBatch,
                "Destination:",
                position - new Vector2(leftText, -15),
                alignment: Vector2.Zero,
                sort: 0.21f,
                color: Palette.Colors[6],
                strokeColor: null,
                shadowColor: Palette.Colors[12],
                maxWidth: 400
            );

            Game.Data.LargeFont.Draw(
                render.UiBatch,
                _stats.Value.Day.ToString(),
                position - new Vector2(rightText, 25),
                alignment: Vector2.Zero,
                sort: 0.21f,
                color: Palette.Colors[20],
                strokeColor: null,
                shadowColor: null,
                maxWidth: 400
            );

            Game.Data.LargeFont.Draw(
                render.UiBatch,
                _stats.Value.Content.ToString(),
                position - new Vector2(rightText, 8),
                alignment: Vector2.Zero,
                sort: 0.21f,
                color: Palette.Colors[20],
                strokeColor: null,
                shadowColor: null,
                maxWidth: 400
            );

            Game.Data.LargeFont.Draw(
                render.UiBatch,
                _stats.Value.Destination.ToString(),
                position - new Vector2(rightText, -12),
                alignment: Vector2.Zero,
                sort: 0.21f,
                color: Palette.Colors[20],
                strokeColor: null,
                shadowColor: null,
                maxWidth: 400
            );

            RenderServices.DrawSprite(
                render.UiBatch,
                _stats.Value.Image.Aseprite,
                position + new Vector2(130, 0),
                _stats.Value.Image.AnimationId,
                0,
                new DrawInfo { Origin = Vector2.Center, Sort = .24f });
        }
    }
}
