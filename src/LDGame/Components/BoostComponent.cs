using Bang.Components;

namespace LDGame.Components;

public readonly struct BoostComponent: IComponent
{
    public readonly float StopWhen = 0f;

    public BoostComponent(float when)
    {
        StopWhen = when;
    }
}