using Murder.Assets;
using Murder.Assets.Graphics;
using Murder.Attributes;
using System.Numerics;

namespace LDGame.Assets
{
    public class UiSkinAsset : GameAsset
    {
        
        public override string EditorFolder => "#Ui";

        public override char Icon => '';
        
        public override Vector4  EditorColor => new Vector4(1f, .8f, .25f, 1f);

        [GameAssetId<SpriteAsset>]
        public Guid Logo = Guid.Empty;
        
        [GameAssetId<SpriteAsset>]
        public Guid BoxBasic = Guid.Empty;

        [GameAssetId<SpriteAsset>]
        public Guid PackageDelivered = Guid.Empty;

        [GameAssetId<SpriteAsset>]
        public Guid MapLine = Guid.Empty;
        [GameAssetId<SpriteAsset>]
        public Guid MapPoint = Guid.Empty;

        [GameAssetId<SpriteAsset>]
        public Guid DrugSlider = Guid.Empty;
        [GameAssetId<SpriteAsset>]
        public Guid DrugPointer = Guid.Empty;
        [GameAssetId<SpriteAsset>]
        public Guid DrugButton = Guid.Empty;
        [GameAssetId<SpriteAsset>]
        public Guid DrugButtonDown = Guid.Empty;

        [GameAssetId<SpriteAsset>]
        public Guid LeftHudBase = Guid.Empty;
        [GameAssetId<SpriteAsset>]
        public Guid RightHudBase = Guid.Empty;
        [GameAssetId<SpriteAsset>]
        public Guid HealthBar = Guid.Empty;

        [GameAssetId<SpriteAsset>]
        public Guid MessageLeft = Guid.Empty;
        [GameAssetId<SpriteAsset>]
        public Guid MessageRight = Guid.Empty;

        // Phone

        [GameAssetId<SpriteAsset>]
        public Guid PhoneBase = Guid.Empty;
        [GameAssetId<SpriteAsset>]
        public Guid PhoneThumb = Guid.Empty;
        [GameAssetId<SpriteAsset>]
        public Guid MessageIn = Guid.Empty;
        [GameAssetId<SpriteAsset>]
        public Guid MessageOut = Guid.Empty;
        [GameAssetId<SpriteAsset>]
        public Guid MessageIdea = Guid.Empty;
        [GameAssetId<SpriteAsset>]
        public Guid TinderBg = Guid.Empty;
    }
}