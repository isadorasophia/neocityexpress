using Bang.Components;
using LDGame.Assets;
using Murder.Attributes;
using Murder.Utilities.Attributes;
using Newtonsoft.Json;

namespace LDGame.Components
{
    [Unique]
    [Story]
    public readonly struct DayCycleComponent : IComponent
    {
        [JsonProperty]
        [GameAssetId(typeof(DayCycleAsset))]
        public readonly Guid DayCycle = Guid.Empty;

        public DayCycleComponent() { }

        public DayCycleComponent(Guid day) => DayCycle = day;
    }
}
