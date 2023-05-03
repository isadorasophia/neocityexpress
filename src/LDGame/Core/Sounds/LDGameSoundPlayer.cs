using Murder.Core.Sounds;
using Murder.Diagnostics;
using LDGame.Core.Sounds.Fmod;
using System.Diagnostics;
using Murder;
using Newtonsoft.Json.Linq;

namespace LDGame.Core.Sounds
{
    /// <summary>
    /// This is the sound player for road, which relies on fmod.
    /// The latest version tested was "2.02.11"
    /// </summary>
    public partial class LDGameSoundPlayer : ISoundPlayer, IDisposable
    {
        public static LDGameSoundPlayer Instance => ((LDGameSoundPlayer)Game.Sound);
        private readonly static string _bankRelativeToResourcesPath = Path.Join("sounds", "fmod");

        private Studio? _studio;

        private readonly Dictionary<SoundEventId, Bank> _banks = new();
        private readonly Dictionary<SoundEventId, Bus> _buses = new();
        private readonly Dictionary<SoundEventId, EventDescription> _events = new();
        private readonly Dictionary<SoundEventId, EventInstance> _instances = new();

        private bool _initialized = false;
        
        public void Initialize(string resourcesPath)
        {
            if (_initialized)
            {
                // This was likely called from a refresh call.
                // We simply need to make sure we are refreshing the cache.
                _cacheEventDescriptions = null;
                _cacheBuses = null;

                return;
            }
            
            if (!LoadFmodAssemblies(resourcesPath))
            {
                return;
            }

            InitializeFmod();

            _ = FetchBanks(resourcesPath);
            
            _initialized = true;
        }

        /// <summary>
        /// This will load and initialize the fmod library so we are ready once the game starts.
        /// </summary>
        private void InitializeFmod()
        {
            // *apparently*, there is a requirement to call the core API before calling the studio API?
            // this seems to break Linux scenarios, but i haven't seen this yet.
            FMOD.Memory.GetStats(out int currentAllocated, out int maxAllocated);

            FMOD.RESULT result = FMOD.Studio.System.Create(out FMOD.Studio.System studio);
            if (!FmodHelpers.Check(result, "Unable to create the fmod factory system!")) return;

            result = studio.GetCoreSystem(out FMOD.System core);
            if (!FmodHelpers.Check(result, "Unable to acquire the core system?")) return;

            _studio = new(studio);

            FMOD.Studio.INITFLAGS studioInitFlags = FMOD.Studio.INITFLAGS.NORMAL;

#if DEBUG
            studioInitFlags |= FMOD.Studio.INITFLAGS.LIVEUPDATE;
#endif

            result = studio.Initialize(
                maxchannels: 256,
                studioflags: studioInitFlags,
                flags: FMOD.INITFLAGS.CHANNEL_LOWPASS | FMOD.INITFLAGS.CHANNEL_DISTANCEFILTER,
                extradriverdata: IntPtr.Zero);

            FmodHelpers.Check(result, "Unable to initialize fmod?");

            // okay, *this has to be called last*. I am not sure why, but things started exploding
            // (non-stream files would not play) if this was not the case <_<
            core.SetDSPBufferSize(bufferlength: 4, numbuffers: 32);
        }

        internal EventInstance? FetchOrCreateInstance(SoundEventId id)
        {
            if (_studio is null)
            {
                return null;
            }

            if (!_events.TryGetValue(id, out EventDescription? description))
            {
                Debug.Assert(_studio is not null);

                description = _studio.GetEvent(id);
                if (description is not null)
                {
                    _events.Add(id, description);
                }
            }

            return description?.CreateInstance();
        }
        
        public void Update()
        {
            _studio?.Update();
        }
        
        public ValueTask PlayEvent(SoundEventId id, bool isLoop, bool stopLastMusic = true)
        {
            if (isLoop)
            {
                return PlayStreaming(id, stopLastMusic);
            }
            
            // Otherwise, this will be played and immediately released.
            using EventInstance? scopedInstance = FetchOrCreateInstance(id);
            scopedInstance?.Start();

            return default;
        }

        private SoundEventId? _lastPlayedStreaming = default;

        public ValueTask PlayStreaming(SoundEventId id, bool stopLastMusic = true)
        {
            if (_lastPlayedStreaming != null && _lastPlayedStreaming.Value.Equals(id))
            {
                // TODO: Figure out how to do this switch in fmod.
                // The music is currently playing already and it is the same as the last one.
                // Se'll just skip playing it again.
                return default;
            }

            if (stopLastMusic)
            {
                // TODO: Figure out how to do this switch in fmod.
                // In the meantime, we'll manually stop all previous songs.
                Stop(fadeOut: true);
                _lastPlayedStreaming = id;
            }

            if (!_instances.TryGetValue(id, out EventInstance? instance))
            {
                instance = FetchOrCreateInstance(id);
                if (instance is not null)
                {
                    // This won't be released right away, so we will track its instance.
                    _instances.Add(id, instance);
                } 
            }

            instance?.Start();
            return default;
        }

        public void SetParameter(SoundEventId id, string name, float value)
        {
            if (_instances.TryGetValue(id, out var instance))
            {
                instance.SetParameterValue(name, value);
            }
            else
            {
                //GameLogger.Error($"Missing sound ID {id.Path}");
            }
        }

        public void SetParameter(SoundEventId id, ParameterId parameterId, float value)
        {
            if (_instances.TryGetValue(id, out var instance))
            {
                instance.SetParameterValue(parameterId.ToFmodId(), value);
            }
            else
            {
                //GameLogger.Error($"Missing sound ID {id.Path}");
            }
        }

        public void SetGlobalParameter(ParameterId parameterId, float value)
        {
            // This might not take effect if the parameter is global.
            _studio?.SetParameterValue(parameterId, value);
        }

        public float? GetGlobalParameterValue(ParameterId parameterId)
        {
            // This might not take effect if the parameter is global.
            return _studio?.GetParameterCurrentValue(parameterId);
        }

        public bool Stop(SoundEventId id, bool fadeOut)
        {
            if (_instances.TryGetValue(id, out var instance))
            {
                instance.Stop(fadeOut);
                return true;
            }
            else
            {
                return false;
                // GameLogger.Warning($"Missing sound ID {id.Path}");
            }
        }

        public void Stop(bool fadeOut)
        {
            EventInstance[] sounds = _instances.Values.ToArray();
            _instances.Clear();
            
            foreach (EventInstance instance in sounds)
            {
                instance.Stop(fadeOut);
                instance.Dispose();
            }
            
            _lastPlayedStreaming = null;
        }
        
        public void SetVolume(SoundEventId? id, float volume)
        {
            if (id is null)
            {
                GameLogger.Fail("Unable to find a null bus id.");
                return;
            }

            if (!_buses.TryGetValue(id.Value, out Bus? bus))
            {
                bus = _studio?.GetBus(id.Value);
                if (bus is null)
                {
                    // GameLogger.Fail("Invalid bus name!");
                    return;
                }

                _buses.Add(id.Value, bus);
            }

            bus.Volume = volume;
        }
        
        public void Dispose()
        {
            foreach (Bank bank in _banks.Values)
            {
                bank.Dispose();
            }
            
            foreach (EventDescription @event in _events.Values)
            {
                @event.Dispose();
            }
            
            foreach (Bus bus in _buses.Values)
            {
                bus.Dispose();
            }

            foreach (EventInstance instance in _instances.Values)
            {
                instance.Dispose();
            }

            _studio?.Dispose();
        }
    }
}
