using Bang.Components;
using Murder.Systems;
using Murder.Utilities;

namespace LDGame.Components;

public readonly struct PlayerSpeedComponent : IComponent
{
    public readonly float Speed = 0f;

    public PlayerSpeedComponent()
    {
    }

    public PlayerSpeedComponent(float speed)
    {
        Speed = speed;
    }

    internal PlayerSpeedComponent Lower(float amount)
    {
        return new PlayerSpeedComponent(Math.Max(0.25f, Speed - amount));
    }
    internal PlayerSpeedComponent Increase(float amount)
    {
        if (Speed + amount < 2.8f)
        {
            return new PlayerSpeedComponent(Speed + amount);
        }

        return this;
    }

    internal PlayerSpeedComponent Brake(float amount)
    {
        return new PlayerSpeedComponent(Math.Max(0f, Speed - amount));
    }

    internal PlayerSpeedComponent Approach(float target, float amount)
    {
        return new PlayerSpeedComponent(Calculator.Approach(Speed,target, amount));
    }
}