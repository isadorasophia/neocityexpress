using Bang.Components;
using Murder.Assets;
using Murder.Attributes;
using Murder.Core.Sounds;

namespace LDGame.Components
{
    [Unique]
    [DoNotPersistEntityOnSave]
    public readonly struct PlayerComponent : IComponent
    {
        public readonly PlayerStates CurrentState;

        public PlayerComponent(PlayerStates state)
        {
            CurrentState = state;
        }

        internal PlayerComponent SetState(PlayerStates state)
        {
            return new PlayerComponent(state);
        }
    }

    public enum PlayerStates
    {
        Normal,
        Casting,
    }
}
