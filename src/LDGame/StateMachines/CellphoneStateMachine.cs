using Bang.Components;
using Bang.Entities;
using Bang.StateMachines;
using LDGame.Assets;
using LDGame.Core;
using LDGame.Core.Sounds;
using LDGame.Messages;
using LDGame.Services;
using Murder;
using Murder.Core;
using Murder.Core.Geometry;
using Murder.Core.Graphics;
using Murder.Core.Input;
using Murder.Messages;
using Murder.Services;
using Murder.Utilities;

namespace LDGame.StateMachines
{
    internal class CellphoneStateMachine : StateMachine
    {
        private readonly Queue<TextMessage> _messages = new();

        private TextMessage LastMessage => _messages.Last();

        private bool _exit = false;

        private float _lastSentTime = 0;
        private readonly float _totalSecondsOnNext = 1;

        private Entity? _entitySendingMessages = null;

        public static readonly Point MaskDimensions = new(x: 150, y: 200);
        private static readonly Point _dimensions = new(x: 150, y: 260);

        private float _phoneSway = 0f;
        private float _phoneSwayPosition = 0f;

        private string _sender = string.Empty;

        private bool _sendChoiceOnFinishTyping = true;
        public CellphoneStateMachine()
        {
            State(Main);
        }

        public CellphoneStateMachine(string sender) : this()
        {
            _sender = sender;
        }

        private IEnumerator<Wait> Main()
        {
            if (DayCycle.TryGetCurrentDay(World) is DayCycle dayCycle)
            {
                foreach (var mod in dayCycle.StartingModifiers)
                {
                    _phoneSway += (Game.Data.GetAsset<ModifierAsset>(mod)).PhoneSway;
                }
            }

            _lastSentTime = Game.NowUnescaled;
            Entity.SetCustomDraw(DrawMessage);
            _phoneEnterAnim = Game.Now;

            yield return GoTo(WaitInput);
        }

        private IEnumerator<Wait> WaitInput()
        {
            while (true)
            {
                float interval = Game.NowUnescaled - _lastSentTime;

                if (_exit)
                {
                    interval = Game.Now - _phoneExitAnimation;
                }

                if ((_exit && interval > _totalSecondsOnNext) || _messages.Count == 0)
                {
                    yield return Wait.ForSeconds(2f);

                    _phoneEnterAnim = Game.Now;
                    _puttingPhoneAway = true;

                    yield return Wait.ForSeconds(1f);

                    Entity.RemoveCustomDraw();
                    Entity.RemoveStateMachine();

                    yield break;
                }

                if (LastMessage.Choices is not null)
                {
                    yield return GoTo(TypeMessage);
                }

                if (interval > _totalSecondsOnNext)
                {
                    _entitySendingMessages?.SendMessage<NextDialogMessage>();
                    yield return GoTo(WaitNextMessage);
                }

                yield return Wait.NextFrame;
            }
        }

        private IEnumerator<Wait> WaitNextMessage()
        {
            yield return Wait.ForMessage<TextMessage>();
            yield return GoTo(WaitInput);
        }

        private int FindLastMatchedIndex(string original, string match)
        {
            int matchIndex = 0;
            int originalIndex = 0;

            while (matchIndex < match.Length)
            {
                if (char.ToUpperInvariant(original[originalIndex++]) == char.ToUpperInvariant(match[matchIndex]))
                {
                    matchIndex++;
                }
            }

            return originalIndex;
        }

        private IEnumerator<Wait> TypeMessage()
        {
            using ListenKeyboardHelper listener = new(1024);

            while (true)
            {
                string parsedTarget = TextMessage.EscapePunctuation(Game.Input.GetKeyboardInput());

                int matchedIndex = -1;

                ParsedMessage[] choices = LastMessage.Choices!;

                for (int i = 0; i < choices.Length; ++i)
                {
                    string parsedChoiceText = choices[i].Parsed;
                    if (parsedChoiceText.StartsWith(parsedTarget, StringComparison.OrdinalIgnoreCase))
                    {
                        int previousIndex = choices[i].MatchedIndex;
                        choices[i].MatchedIndex = FindLastMatchedIndex(choices[i].Message, parsedTarget);

                        if (choices[i].MatchedIndex > previousIndex)
                        {
                            LDGameSoundPlayer.Instance.PlayEvent(LibraryServices.GetRoadLibrary().KeyboardType, isLoop: false);
                        }

                        matchedIndex = i;
                    }
                    else
                    {
                        choices[i].MatchedIndex = 0;
                    }

                    if (matchedIndex == i && parsedTarget.Length == parsedChoiceText.Length)
                    {
                        var save = SaveServices.GetOrCreateSave();
                        save.MessagesSent++;
                        
                        yield return Wait.ForSeconds(.5f);

                        _messages.Enqueue(TextMessage.Create(choices[i].Message, isOurs: true, sender: string.Empty));
                        _lastSentTime = Game.NowUnescaled;

                        yield return Wait.ForSeconds(.5f);
                        if (_sendChoiceOnFinishTyping)
                        {
                            _entitySendingMessages?.SendMessage<PickChoiceMessage>(new(i));
                        }
                        else
                        {
                            _entitySendingMessages?.SendMessage<NextDialogMessage>();
                        }

                        yield return GoTo(WaitInput);
                    }
                }

                if (matchedIndex == -1)
                {
                    Game.Input.ClearLastKeyboardInput(length: 1);
                }

                yield return Wait.NextFrame;
            }
        }

        float _phoneEnterAnim = 0f;
        float _phoneExitAnimation = 0;

        private bool _puttingPhoneAway;

        private void DrawMessage(RenderContext render)
        {
            if (_messages.Count == 0)
            {
                return;
            }

            _phoneSwayPosition += _phoneSway * Game.DeltaTime;
            float sway = Math.Clamp(_phoneSwayPosition, 0, 100f);

            Mask2D? mask = render.Mask2DSmaller;
            if (mask is null)
            {
                return;
            }

            var save = SaveServices.GetOrCreateSave();
            var batch = mask.Begin();

            float enterDelta = 1f - Ease.BackOut(Calculator.ClampTime(Game.Now - _phoneEnterAnim, 1f));

            if (_puttingPhoneAway)
            {
                enterDelta = 1 - enterDelta;
            }

            var skin = LibraryServices.GetUiSkin();
            Point cameraSize = render.Camera.Size;
            Vector2 cellPosition = new(cameraSize.X - 195 - (_dimensions.X + (_puttingPhoneAway ? 100 : -60)) * enterDelta - sway, cameraSize.Y - _dimensions.Y + ((_puttingPhoneAway ? 300 : 80) * enterDelta));

            Rectangle cellphoneRect = new(0, 0, _dimensions.X, _dimensions.Y);

            // Draw Hand
            RenderServices.Draw9Slice(render.UiBatch, skin.PhoneBase, "messages", 0,
                new Rectangle(cellPosition.X - 14, cellPosition.Y - 14, cellphoneRect.Width + 102, cellphoneRect.Height + 30), new DrawInfo(.7f));

            // We also have "idle" and "press" animations
            RenderServices.DrawSprite(render.UiBatch, skin.PhoneThumb, cellPosition.X + 45, cellPosition.Y + 156, "slow_type", new DrawInfo(0.66f));

            int lineHeight = 0;
            foreach (TextMessage message in _messages)
            {
                if (string.IsNullOrEmpty(message.Content))
                {
                    continue; 
                }

                Vector2 position = cellphoneRect.TopLeft + new Vector2(12, 15 + lineHeight);
                Vector2 aligment = Vector2.Zero;

                Guid msgBg = skin.MessageIn;
                Color textColor = Palette.Colors[11];
                if (message.IsOurs)
                {
                    position = cellphoneRect.TopRight + new Vector2(-16, 15 + lineHeight);
                    aligment = new Vector2(1, 0);
                    msgBg = skin.MessageOut;
                    textColor = Palette.Colors[13];
                }

                int totalLines = Game.Data.PixelFont.Draw(batch, message.Content,
                    position, aligment, sort: 0.61f, textColor, null, null, 120);

                int textHeight = (totalLines - 1) * 8 + 8;
                lineHeight += textHeight + 16;
                RenderServices.Draw9Slice(batch, msgBg,
                    new Rectangle(cellphoneRect.X + 3, position.Y - 8, cellphoneRect.Width - 10, textHeight + 14), new DrawInfo(0.63f));
            }


            int maximumMatch = 0;
            if (LastMessage.Choices is ParsedMessage[] choices)
            {
                for (int i = 0; i < choices.Length; ++i)
                {
                    ParsedMessage choice = choices[i];
                    maximumMatch = Math.Max(maximumMatch, choice.MatchedIndex);

                    int totalLines;
                    if (choice.MatchedIndex > 0)
                    {
                        totalLines = Game.Data.PixelFont.DrawWithSomeStrokeLetters(batch, choice.Message,
                        cellphoneRect.TopRight + new Vector2(-19, 15 + lineHeight), new Vector2(1, 0), sort: 0.625f, Palette.Colors[14], strokeColor: Palette.Colors[17], null,
                            characterWithStroke: choice.MatchedIndex, maxWidth: 120);
                        Game.Data.PixelFont.Draw(batch, choice.Message,
                            cellphoneRect.TopRight + new Vector2(-19, 15 + lineHeight), new Vector2(1, 0), sort: 0.61f, Palette.Colors[14], null, null, 120, choice.MatchedIndex);
                    }
                    else
                    {
                        totalLines = Game.Data.PixelFont.Draw(batch, choice.Message,
                            cellphoneRect.TopRight + new Vector2(-19 + MathF.Sin(Game.Now * 8 + i * MathF.PI * 0.5f), 15 + lineHeight), new Vector2(1, 0), sort: 0.61f, Palette.Colors[14] * 0.5f, null, null, 120, -1);
                    }

                    int textHeight = (totalLines - 1) * 8 + 8;
                    if (choice.MatchedIndex > 0)
                    {
                        RenderServices.Draw9Slice(batch, skin.MessageIdea,
                        new Rectangle(cellphoneRect.X + 1, cellphoneRect.Top + lineHeight + 9, cellphoneRect.Width - 10, textHeight + 16), new DrawInfo(0.63f));
                    }
                    else
                    {
                        RenderServices.Draw9Slice(batch, skin.MessageIdea,
                        new Rectangle(cellphoneRect.X + 4 + MathF.Sin(Game.Now * 8 + i * MathF.PI * 0.5f), cellphoneRect.Top + lineHeight + 9, cellphoneRect.Width - 10, textHeight + 16), new DrawInfo(0.63f)
                        {
                            Color = Color.White * (0.5f + 0.25f * MathF.Sin(Game.Now * 8 + i * 0.5f))
                        });
                    }

                    lineHeight += textHeight + 9;
                }

                if (save.MessagesSent == 0)
                {
                    if (maximumMatch < 8)
                    {
                        Game.Data.PixelFont.Draw(batch, "Type using your keyboard to select an option.\n\n\nIgnore punctuation and spaces.",
                                cellphoneRect.TopRight + new Vector2(-80, 15 + lineHeight), new Vector2(.5f, 0), sort: 0.61f,
                                Palette.Colors[Calculator.Blink(5f, false) ? 8 : 7], Palette.Colors[9], null, 120);
                    }
                    else
                    {
                        Game.Data.PixelFont.Draw(batch, "You can change your mind with BACKSPACE.",
                                cellphoneRect.TopRight + new Vector2(-80, 15 + lineHeight), new Vector2(.5f, 0), sort: 0.61f,
                                Palette.Colors[Calculator.Blink(5f, false) ? 4 : 5], Palette.Colors[2], null, 120);
                    }
                }
                
            }

            if (!string.IsNullOrEmpty(_sender))
            {
                Game.Data.PixelFont.Draw(render.UiBatch, _sender, cellPosition + new Vector2(38, 13), 
                    new Vector2(0, 0), sort: 0.62f, Palette.Colors[13], null, null, maxWidth: 120);
            }

            float interval = Game.NowUnescaled - _lastSentTime;

            float scrollTime = 1f;
            float ratio = Ease.BackOut(Calculator.Clamp01(interval / scrollTime));

            float cameraOffsetY = 0;
            if (lineHeight * 1.32f > MaskDimensions.Y)
            {
                cameraOffsetY = Math.Max(0, lineHeight * 1.32f - MaskDimensions.Y) * Ease.BackOut(ratio);
            }

            mask.End(render.UiBatch, cellPosition + new Vector2(0, 23), camera: new Vector2(0, -(int)cameraOffsetY), new DrawInfo(0.69f));;
        }

        protected override void OnMessage(IMessage message)
        {
            if (message is TextMessage textMessage)
            {
                if (textMessage.Clear)
                {
                    _exit = true;
                    _phoneExitAnimation = Game.Now;
                }
                else
                {
                    if (_messages.Count == 0&& !textMessage.IsOurs)
                    {
                        LDGameSoundPlayer.Instance.PlayEvent(LibraryServices.GetRoadLibrary().GrandmaTextVibrate, isLoop: false);;
                    }
                    else
                    {
                        LDGameSoundPlayer.Instance.PlayEvent(LibraryServices.GetRoadLibrary().TextMessageGet, isLoop: false);
                    }

                    if (textMessage.IsOurs && (textMessage.Choices is null || textMessage.Choices.Length == 0))
                    {
                        // Actually, we are typing everything that we sent ourselves.
                        // So switch this to a "choice".
                        _sendChoiceOnFinishTyping = false;
                        _messages.Enqueue(textMessage.AsChoice());
                    }
                    else
                    {
                        _sendChoiceOnFinishTyping = true;
                        _messages.Enqueue(textMessage);
                    }

                    _lastSentTime = Game.NowUnescaled;
                }
            }
            else if (message is TargetEntityMessage targetEntityMessage)
            {
                _entitySendingMessages = targetEntityMessage.Entity;
            }
        }
    }
}