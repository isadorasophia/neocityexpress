using Murder.Attributes;
using System.Collections.Immutable;

namespace LDGame.Assets
{
    public readonly struct DatingProfiles
    {
        [GameAssetId(typeof(DatingProfileAsset))]
        public readonly ImmutableArray<Guid> Profiles = ImmutableArray<Guid>.Empty;

        public DatingProfiles() { }
    }
}
