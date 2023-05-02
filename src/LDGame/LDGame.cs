using LDGame.Assets;
using LDGame.Core;
using LDGame.Core.Sounds;
using LDGame.Data;
using Microsoft.Xna.Framework.Input;
using Murder;
using Murder.Assets;
using Murder.Core.Input;
using Murder.Core.Sounds;
using Murder.Save;
using System.Collections.Immutable;

namespace LDGame
{
    public class LDGame : IMurderGame
    {
        public static LDGameProfile Profile => (LDGameProfile)Game.Profile;
        public string Name => "LDGame";

        public ISoundPlayer CreateSoundPlayer() => new LDGameSoundPlayer();

        public void Initialize()
        {
            Game.Data.CurrentPalette = Palette.Colors.ToImmutableArray();

            // Register movement axis input.
            Game.Input.Register(MurderInputAxis.Movement, GamepadAxis.LeftThumb, GamepadAxis.RightThumb, GamepadAxis.Dpad);

            Game.Input.Register(MurderInputAxis.Movement,
                new KeyboardAxis(Keys.W, Keys.A, Keys.S, Keys.D),
                new KeyboardAxis(Keys.Up, Keys.Left, Keys.Down, Keys.Right));

            Game.Input.Register(MurderInputAxis.Movement,
                new KeyboardAxis(Keys.W, Keys.A, Keys.S, Keys.D),
                new KeyboardAxis(Keys.Up, Keys.Left, Keys.Down, Keys.Right));

            Game.Input.Register(MurderInputAxis.Ui,
                new KeyboardAxis(Keys.W, Keys.A, Keys.S, Keys.D),
                new KeyboardAxis(Keys.Up, Keys.Left, Keys.Down, Keys.Right));

            Game.Input.Register(MurderInputAxis.UiTab,
                new KeyboardAxis(Keys.Q, Keys.Q, Keys.E, Keys.E),
                new KeyboardAxis(Keys.PageUp, Keys.PageUp, Keys.PageDown, Keys.PageDown));

            Game.Input.Register(MurderInputAxis.UiTab,
                new ButtonAxis(Buttons.LeftShoulder, Buttons.LeftShoulder, Buttons.RightShoulder, Buttons.RightShoulder));

            Game.Input.Register(InputButtons.LockMovement, Keys.LeftShift, Keys.RightShift);
            Game.Input.Register(InputButtons.LockMovement, Buttons.LeftTrigger, Buttons.RightTrigger);

            Game.Input.Register(InputButtons.Attack,
                Keys.Z);
            Game.Input.Register(InputButtons.Attack,
                Buttons.X);

            Game.Input.Register(InputButtons.Submit,
                Keys.Enter, Keys.Space);

            Game.Input.Register(InputButtons.SubmitWithEnter,
                Keys.Enter);

            Game.Input.Register(InputButtons.Cancel,
                Keys.Escape, Keys.Delete, Keys.Back, Keys.BrowserBack);

            Game.Input.Register(InputButtons.Interact, Keys.Space);

            Game.Input.Register(InputButtons.Interact, Buttons.Y);

            Game.Input.Register(InputButtons.Skip,
                Keys.Back, Keys.Escape, Keys.O);
            Game.Input.Register(InputButtons.Skip,
                Buttons.Start);

            Game.Input.Register(InputButtons.LeftSwipe,
                Keys.NumPad1, Keys.D1);

            Game.Input.Register(InputButtons.RightSwipe,
                Keys.NumPad2, Keys.D2);
        }

        public SaveData CreateSaveData(string name) => new LdSaveData(name);

        public GameProfile CreateGameProfile() => new LDGameProfile();

        public GamePreferences CreateGamePreferences() => new LdGamePreferences();
    }
}
