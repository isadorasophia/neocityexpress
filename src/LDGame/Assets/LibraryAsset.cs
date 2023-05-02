using Murder.Assets;
using Murder.Assets.Graphics;
using Murder.Attributes;
using Murder.Core;
using Murder.Core.Geometry;
using Murder.Core.Sounds;
using Murder.Utilities;
using System.Collections.Immutable;
using System.Numerics;

namespace LDGame.Assets
{
    public class LibraryAsset : GameAsset
    {
        public override string EditorFolder => "#\uf02dLibraries";

        public override char Icon => '\uf02d';

        public override Vector4 EditorColor => "#FA5276".ToVector4Color();
        
        [GameAssetId<UiSkinAsset>]
        public readonly Guid UiSkin = Guid.Empty;

        [GameAssetId<PrefabAsset>]
        public readonly Guid Warning = Guid.Empty;

        [GameAssetId<SpeakerAsset>]
        public readonly Guid DriverSpeaker = Guid.Empty;

        [GameAssetId<SpeakerAsset>]
        public readonly Guid GrannySpeaker = Guid.Empty;

        public ImmutableArray<int> LanePosition = ImmutableArray<int>.Empty;

        [GameAssetId<PrefabAsset>]
        public readonly ImmutableArray<Guid> Explosions = ImmutableArray<Guid>.Empty;

        [Tooltip("This is the bounds that the road which the player will be driving will be displayed.")]
        public IntRectangle Bounds = Rectangle.Empty;

        [GameAssetId<WorldAsset>]
        [Tooltip("World asset if the player dies.")]
        public Guid DeathWorld = Guid.Empty;

        [GameAssetId<WorldAsset>]
        public Guid MainMenu = Guid.Empty;

        [GameAssetId<PrefabAsset>]
        public Guid PausePrefab = Guid.Empty;

        [GameAssetId<DayCycleAsset>]
        public Guid FirstDay = Guid.Empty;

        [GameAssetId<PrefabAsset>]
        public Guid NearMissPrefab = Guid.Empty;

        public Portrait FmodImage = new();

        public Portrait CityImage = new();

        [GameAssetId<WorldAsset>]
        public Guid TrueEnding = Guid.Empty;

        public readonly SoundEventId CarLoop;
        public readonly SoundEventId CarImpact;

        public readonly SoundEventId GameOverCrash;
        public readonly SoundEventId CarTireSqueal;

        public readonly SoundEventId CarInterfaceGlitch1;
        public readonly SoundEventId CarInterfaceGlitch2;
        public readonly SoundEventId CarInterfaceGlitch3;

        public readonly SoundEventId CarInterfaceAlarm1;
        public readonly SoundEventId CarInterfaceAlarm2;

        public readonly SoundEventId CarBoostStart;
        public readonly SoundEventId CarBoostLoop;
        public readonly SoundEventId CarBoostEnd;

        public readonly SoundEventId GrandmaTextVibrate;
        public readonly SoundEventId GrandmaTextBeep;
        public readonly SoundEventId DaggersTextBeep;

        public readonly SoundEventId LeftDrug;
        public readonly SoundEventId Fire;

        public readonly SoundEventId Horn;
        public readonly SoundEventId KeyboardType;
        public readonly SoundEventId UiBack;
        public readonly SoundEventId UiConfirm;
        public readonly SoundEventId UiHover;
        public readonly SoundEventId WarningCar;
        public readonly SoundEventId TextMessageGet;
        public readonly SoundEventId TinderMatch;
        public readonly SoundEventId TinderSwipeLeft;
        public readonly SoundEventId TinderSwipeRight;

        public readonly SoundEventId LevelMusic;
        public readonly SoundEventId TrueEndingNarrator;

        public readonly SoundEventId HospitalMusic;
        public readonly SoundEventId HeartMonitor;

        public readonly ParameterId EyesClosedParameter;

        internal float GetLanePosition(int lane)
        {
            if (lane < 0)
                return LanePosition[0];
            else if (lane >= LanePosition.Length)
                return LanePosition[^1];

            return LanePosition[lane];
        }
    }
}