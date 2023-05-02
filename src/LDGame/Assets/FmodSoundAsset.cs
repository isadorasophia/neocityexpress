using Murder.Assets;
using Murder.Core.Sounds;

namespace Road.Assets
{
    public class FmodSoundAsset : SoundAsset
    {
        public readonly SoundEventId Event = new();
        
        public override SoundEventId Sound() => Event;
    }
}
