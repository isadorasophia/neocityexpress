using LDGame.Attributes;
using Murder.Assets;
using Murder.Attributes;
using Murder.Core.Sounds;

namespace LDGame.Assets
{
    public class LDGameProfile : GameProfile
    {
        [GameAssetId(typeof(LibraryAsset))]
        public readonly Guid Library;
        
        [FmodId(FmodIdKind.Bus)]
        [Tooltip("This is the bus in fmod that translates to the music setting.")]
        public readonly SoundEventId MusicBus;

        [FmodId(FmodIdKind.Bus)]
        [Tooltip("This is the bus in fmod that translates to the sound setting.")]
        public readonly SoundEventId SoundBus;
    }
}