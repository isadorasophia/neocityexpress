using Murder.Core.Sounds;
using Murder.Diagnostics;
using System.Collections.Immutable;

namespace LDGame.Core.Sounds.Fmod
{
    /// <summary>
    /// Wrapper for an event. This is used to create event instances.
    /// This can be reetrieved from a bank by querying all events in a bank, or through
    /// a specific path or guid.
    /// </summary>
    public class EventDescription : IDisposable
    {
        private readonly FMOD.Studio.EventDescription _event;
        
        private string? _path;
        private FMOD.GUID? _id;

        private ImmutableArray<EventInstance>? _instancesCached;

        public bool IsLoaded { get; private set; }

        public EventDescription(FMOD.Studio.EventDescription @event)
        {
            _event = @event;
        }

        public SoundEventId Id => Guid.ToSoundId().WithPath(Path);
        
        public string Path
        {
            get
            {
                if (_path is null)
                {
                    _event.GetPath(out string? path);
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
                    _event.GetID(out FMOD.GUID id);
                    _id = id;
                }

                return _id.Value;
            }
        }
        
        public ImmutableArray<EventInstance> GetInstances()
        {
            if (_instancesCached is null)
            {
                FMOD.RESULT result = _event.GetInstanceList(out FMOD.Studio.EventInstance[]? instances);
                if (result != FMOD.RESULT.OK || instances is null)
                {
                    GameLogger.Error("Unable to list instances for an event.");
                    return ImmutableArray<EventInstance>.Empty;
                }

                _instancesCached = instances.Select(e => new EventInstance(e)).ToImmutableArray();
            }

            return _instancesCached.Value;
        }

        /// <summary>
        /// Create a new instance of the event.
        /// Loading sample data through this method will require a bit of time,
        /// make sure you don't need to play the event immediately.
        /// </summary>
        public EventInstance CreateInstance()
        {
            _event.CreateInstance(out FMOD.Studio.EventInstance instance);
            _instancesCached = null;
            
            return new(instance);
        }

        /// <summary>
        /// There are three ways to load sample data (any non-streamed sound):
        ///   - From a bank. This will keep all the bank's data in memory, until unloaded.
        ///   - From an event description. This will keep the event's encessary data in memory until unloaded.
        ///   - From an event instance. The data will only be in memory while the instance is.
        /// </summary>
        public void LoadSampleData()
        {
            FMOD.RESULT result = _event.LoadSampleData();
            GameLogger.Verify(result == FMOD.RESULT.OK, "Unable to load sample data for target event.");

            IsLoaded = true;
        }

        public void Dispose()
        {
            _event.UnloadSampleData();
            _event.ReleaseAllInstances();
        }
    }
}
