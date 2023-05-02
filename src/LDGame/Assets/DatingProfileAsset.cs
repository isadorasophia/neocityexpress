using Murder.Assets;
using Murder.Attributes;
using Murder.Components;
using Murder.Core;
using System.Collections.Immutable;

namespace LDGame.Assets
{
    public readonly struct DateProfileData
    {
        public readonly Portrait Portrait;

        [Tooltip("This is the prompt for the profile description.")]
        public readonly SituationComponent Description;

        public readonly bool CanMatch = true;

        public DateProfileData() { }
    }

    public class DatingProfileAsset : GameAsset
    {
        public override string EditorFolder => "#\uf004Dating";

        public override char Icon => '\uf004';

        [Tooltip("This is the threshold of the likehood of getting a match on the amount of swipes.")]
        public readonly int ThresholdForStartMatching = 0;

        public ImmutableArray<DateProfileData> Profiles = ImmutableArray<DateProfileData>.Empty;

        public DatingProfileAsset() { }
    }
}
