using Murder.Assets;
using Murder.Attributes;
using Murder.Components;
using Murder.Interactions;
using System.Collections.Immutable;

namespace LDGame.Assets
{
    public readonly struct Consequence
    {
        public readonly InteractOnRuleMatchCollectionComponent Triggers = new();

        [Tooltip("What happens once the trigger happens.")]
        public readonly InteractionCollection Effects = new();

        public Consequence() { }
    }

    internal class ConsequencesAsset : GameAsset
    {
        public override string EditorFolder => "#\uf714Consequences";

        public override char Icon => '\uf714';

        public readonly ImmutableArray<Consequence> Consequences = ImmutableArray<Consequence>.Empty;

        public ConsequencesAsset() { }
    }
}
