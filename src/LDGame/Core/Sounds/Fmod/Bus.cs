using Murder.Core.Sounds;

namespace LDGame.Core.Sounds.Fmod
{
    public class Bus : IDisposable
    {
        private readonly FMOD.Studio.Bus _bus;

        private string? _path;
        private FMOD.GUID? _id;
        
        public Bus(FMOD.Studio.Bus bus)
        {
            _bus = bus;
        }

        /// <summary>
        /// Set the bus target volume, from 1 to -1.
        ///  1: Normal volume.
        ///  0: Muted.
        ///  -1: Inverted signal.
        /// This *ignores* modulation and automation applied to the volume within studio.
        /// </summary>
        public float Volume
        {
            get
            {
                _bus.GetVolume(out float volume);
                return volume;
            }
            set
            {
                _bus.SetVolume(value);
            }
        }

        /// <summary>
        /// Whether the bus is muted or not. If a bus is muted, it will override
        /// its inputs, but they will obey their individual mute states once unmuted.
        /// </summary>
        public bool Muted
        {
            get
            {
                _bus.GetMute(out bool muted);
                return muted;
            }
            set
            {
                _bus.SetMute(value);
            }
        }

        public float CurrentVolume
        {
            get
            {
                _bus.GetVolume(out float _ /* volume */, out float finalVolume);
                return finalVolume;
            }
        }
        
        public SoundEventId Id => Guid.ToSoundId().WithPath(Path);

        /// <summary>
        /// The internal path, e.g. "bus:/SFX/Ambience".
        /// </summary>
        public string Path
        {
            get
            {
                if (_path is null)
                {
                    _bus.GetPath(out string? path);
                    _path = path;
                }

                return _path!;
            }
        }

        public FMOD.GUID Guid
        {
            get
            {
                if (_id is null)
                {
                    _bus.GetID(out FMOD.GUID id);
                    _id = id;
                }

                return _id.Value;
            }
        }
        
        public void Dispose()
        {
            // I guess nothing?
        }
    }
}
