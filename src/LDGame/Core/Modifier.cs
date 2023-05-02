using Murder.Attributes;
using Murder.Core.Geometry;

namespace LDGame.Core;

public readonly struct Modifier
{
    public readonly Vector2 ConstantInput = Vector2.Zero;

    [Slider(0, 2f)]
    public readonly float OtherCarsSpeedMultiplier = 1f;
    
    [Slider(0, 8f)]
    public readonly float SuddenBreakEvery = 0f;

    public readonly bool TheGoodShit = false;
    public Modifier()
    {
    }
}