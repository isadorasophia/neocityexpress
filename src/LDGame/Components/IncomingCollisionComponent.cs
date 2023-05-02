using Bang.Components;

namespace LDGame.Components;

public readonly struct IncomingCollisionComponent : IComponent
{
    public readonly float When = 0f;

    public IncomingCollisionComponent()
    {
    }

    public IncomingCollisionComponent(float when)
    {
        When = when;
    }
}