using Bang.Components;
using Bang.Entities;
using Bang.StateMachines;
using LDGame.Assets;
using LDGame.Core;
using LDGame.Core.Sounds;
using LDGame.Services;
using Murder;
using Murder.Assets.Graphics;
using Murder.Components;
using Murder.Core;
using Murder.Core.Dialogs;
using Murder.Core.Geometry;
using Murder.Core.Graphics;
using Murder.Services;
using Murder.Utilities;
using System.Collections.Immutable;

namespace LDGame.StateMachines
{
    internal class TinderStateMachine : StateMachine
    {
        private float _phoneEnterAnimation = 0f;
        private bool _puttingPhoneAway = false;

        private readonly ImmutableArray<DateProfileData> _pool = ImmutableArray<DateProfileData>.Empty;
        private int _currentProfileIndex = 0;

        private readonly Point _dimensions = new(x: 150, y: 260);
        public static readonly Point MaskDimensions = new(x: 150, y: 260);

        private float _phoneSway = 0f;
        private float _phoneSwayPosition = 0f;

        private string _description = string.Empty;

        private bool _isMatch = false;
        private readonly int _thresholdSwipesForMatch = 4;

        public TinderStateMachine(ImmutableArray<DateProfileData> pool, int minimumThreshold) : this()
        {
            _pool = pool;
            _thresholdSwipesForMatch = minimumThreshold;
        }

        public TinderStateMachine()
        {
            State(Main);
        }

        private bool _right = false;
        private bool _left = false;

        private float _lastTimeSwiped = 0;
        private readonly float _swipeDuration = 1f;

        private int _totalSwipesSoFar = 0;

        private string _matchName = string.Empty;

        private IEnumerator<Wait> Main()
        {
            if (_pool.Length == 0)
            {
                yield return Wait.Stop;
            }

            if (DayCycle.TryGetCurrentDay(World) is DayCycle dayCycle)
            {
                foreach (Guid mod in dayCycle.StartingModifiers)
                {
                    _phoneSway += (Game.Data.GetAsset<ModifierAsset>(mod)).PhoneSway;
                }
            }

            Entity.SetCustomDraw(DrawMessage);
            _phoneEnterAnimation = Game.Now;

            yield return GoTo(GoToProfile);
        }

        private IEnumerator<Wait> GoToProfile()
        {
            if (_currentProfileIndex == SaveServices.GetOrCreateSave().TinderIdMatched)
            {
                // Skip matched profiles!
                _currentProfileIndex++;
            }

            if (_currentProfileIndex >= _pool.Length)
            {
                _currentProfileIndex = 0;
            }

            _totalSwipesSoFar++;

            if (!InitializeProfile())
            {
                yield return GoTo(PutPhoneDown);
            }

            while (true)
            {
                bool pressed = false;

                if (Game.Input.PressedAndConsume(InputButtons.LeftSwipe))
                {
                    _left = true;
                    pressed = true;
                }
                else if (Game.Input.PressedAndConsume(InputButtons.RightSwipe))
                {
                    _right = true;
                    pressed = true;
                }

                if (pressed)
                {
                    _lastTimeSwiped = Game.Now;

                    if (_right && _pool[_currentProfileIndex].CanMatch &&
                        _currentProfileIndex > _thresholdSwipesForMatch)
                    {
                        // Likehood of match gets higher as you reach the end of the pool.
                        float ratio = Math.Max(50, Calculator.Clamp01((float)_totalSwipesSoFar / (_pool.Length - 1) * 100));
                        if (RandomExtensions.TryWithChanceOf(Game.Random, ratio))
                        {
                            LDGameSoundPlayer.Instance.PlayEvent(LibraryServices.GetRoadLibrary().TinderMatch, isLoop: false);

                            _isMatch = true;

                            LdSaveData save = SaveServices.GetOrCreateSave();

                            _matchName = save.TinderIdMatched == -1 ? "Remy" : "Grungo";
                            save.TinderIdMatched = _currentProfileIndex;

                            yield return GoTo(OnMatch);
                        }
                    }
                    else if (_right)
                    {
                        LDGameSoundPlayer.Instance.PlayEvent(LibraryServices.GetRoadLibrary().TinderSwipeRight, isLoop: false);
                    }
                    else
                    {
                        LDGameSoundPlayer.Instance.PlayEvent(LibraryServices.GetRoadLibrary().TinderSwipeLeft, isLoop: false);
                    }

                    yield return Wait.ForSeconds(1f);

                    _right = _left = false;
                    _currentProfileIndex++;

                    _lastTimeSwiped = Game.Now;

                    yield return GoTo(GoToProfile);
                }

                yield return Wait.NextFrame;
            }
        }

        private IEnumerator<Wait> OnMatch()
        {
            yield return Wait.ForSeconds(2.5f);
            yield return GoTo(PutPhoneDown);
        }

        private IEnumerator<Wait> PutPhoneDown()
        {
            _phoneEnterAnimation = Game.Now;
            _puttingPhoneAway = true;

            yield return Wait.ForSeconds(1f);

            Entity.RemoveCustomDraw();
            Entity.Destroy();
        }

        private bool InitializeProfile()
        {
            SituationComponent situation = _pool[_currentProfileIndex].Description;

            Character? character = DialogServices.CreateCharacterFrom(situation.Character, situation.Situation);
            if (character is null || character?.NextLine(World)?.Line is not Line line)
            {
                return false;
            }

            _description = line.Text ?? string.Empty;
            return true;
        }

        private void DrawMessage(RenderContext render)
        {
            Mask2D? mask = render.Mask2DBigger;
            if (mask is null)
            {
                return;
            }

            Batch2D batch = mask.Begin();

            _phoneSwayPosition += _phoneSway * Game.DeltaTime;
            float sway = Math.Clamp(_phoneSwayPosition, 0, 100f);

            float enterDelta = 1f - Ease.BackOut(Calculator.ClampTime(Game.Now - _phoneEnterAnimation, 1f));
            if (_puttingPhoneAway) enterDelta = 1 - enterDelta;

            var skin = LibraryServices.GetUiSkin();
            Point cameraSize = render.Camera.Size;
            Vector2 cellPosition = new(cameraSize.X - 195 - (_dimensions.X + (_puttingPhoneAway ? 100 : -60)) * enterDelta - sway, cameraSize.Y - _dimensions.Y + ((_puttingPhoneAway ? 300 : 80) * enterDelta));

            Rectangle cellphoneRect = new(0, 0, _dimensions.X, _dimensions.Y);

            // Draw Hand
            RenderServices.Draw9Slice(render.UiBatch, skin.PhoneBase, "tinder", 0,
                new Rectangle(cellPosition.X - 14, cellPosition.Y - 14, cellphoneRect.Width + 102, cellphoneRect.Height + 30), new DrawInfo(.7f));

            // We also have "idle" and "press" animations
            RenderServices.DrawSprite(render.UiBatch, skin.PhoneThumb, cellPosition.X + 45, cellPosition.Y + 146, "slow_type", new DrawInfo(0.66f));

            // ===============
            // Draw content
            // ===============
            if (_currentProfileIndex < _pool.Length)
            {
                // ===============
                // Swipe if needed
                // ===============
                float ratio = Calculator.Clamp01((Game.Now - _lastTimeSwiped) / _swipeDuration);

                float offsetX = 0;
                float fadeOut = 1;

                if (_left || _right)
                {
                    offsetX = (_left ? -1 : 1) * 250 * Ease.BackInOut(ratio);
                }
                else
                {
                    fadeOut = 1 * Ease.BackOut(ratio);
                }

                if (_isMatch)
                {
                    offsetX = 0;
                    fadeOut = Math.Max(1 - Ease.BackOut(ratio), .5f);

                    // Draw profile description.
                    Game.Data.LargeFont.Draw(batch, $"It's a match!!!  \n{_matchName}",
                        cellphoneRect.CenterPoint - new Vector2(0, 30 + MathF.Sin(Game.Now*12f) * 6), new Vector2(.5f, .5f), sort: 0.3f, Palette.Colors[6], Palette.Colors[9], null, 100);
                }

                // Draw portrait.
                Portrait portrait = _pool[_currentProfileIndex].Portrait; 
                RenderServices.DrawSprite(batch, skin.TinderBg, 0, 0, _isMatch ? "match" : "normal", new DrawInfo(0.99f));
                RenderServices.DrawSprite(batch, skin.TinderBg, offsetX, 0, _isMatch? "match":"normal", new DrawInfo(0.98f));
                if (DialogueServices.GetSpriteAssetForSituation(portrait) is (SpriteAsset asset, string animation))
                {
                    RenderServices.DrawSprite(
                        batch,
                        new Vector2(6 + offsetX, 14 + (_isMatch ? 12 -MathF.Sin(Game.Now * 12f) : 0)),
                        rotation: 0,
                        animation,
                        asset,
                        0,
                        Color.White * (_isMatch ? 1 : fadeOut),
                        0.6f,
                        false) ;
                }

                // Draw profile description.
                if (!_isMatch)
                {
                    Game.Data.PixelFont.Draw(batch, _description,
                        cellphoneRect.CenterPoint + new Vector2(offsetX, 16), new Vector2(.5f, .5f), sort: 0.61f, Palette.Colors[12] * fadeOut, null, null, 100);
                }
            }

            mask.End(render.UiBatch, cellPosition + new Vector2(0, 23), new DrawInfo(0.69f));
        }
    }
}
