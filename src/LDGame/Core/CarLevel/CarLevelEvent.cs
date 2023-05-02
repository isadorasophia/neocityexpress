using Murder.Assets;
using Murder.Attributes;

namespace LDGame.Core;

public readonly struct CarLevelEvent
{
    public readonly float WaitTime = 0f;
    public readonly int Lane = 0;
    public  readonly LanePosition Position = LanePosition.Bottom;
    
    [GameAssetId<PrefabAsset>]
    public readonly Guid PrefabToSpawn = Guid.Empty;

    public readonly bool Warning = true;
    public CarLevelEvent()
    {
    }
}