using LDGame.Core;
using Murder.Assets;
using System.Collections.Immutable;
using System.Numerics;

namespace LDGame.Assets
{
    public class RoadCarsAsset : GameAsset
    {
        public override char Icon => '';
        public override string EditorFolder => "#CarLevels";
        public override Vector4 EditorColor => new Vector4(.8f, 1f, .35f, 1f);

        public ImmutableArray<CarLevelEvent> Events = ImmutableArray<CarLevelEvent>.Empty;
    }
}