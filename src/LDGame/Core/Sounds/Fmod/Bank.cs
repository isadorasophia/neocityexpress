using Murder.Core.Sounds;
using Murder.Diagnostics;
using System.Collections.Immutable;

namespace LDGame.Core.Sounds.Fmod
{
    internal class Bank : IDisposable
    {
        public readonly string Name;
        private readonly FMOD.Studio.Bank _bank;

        private FMOD.GUID? _id = default;
        
        public Bank(FMOD.Studio.Bank bank, string name)
        {
            _bank = bank;
            Name = name;
        }
        
        public SoundEventId Id => Guid.ToSoundId().WithPath(Name);

        public FMOD.GUID Guid
        {
            get
            {
                if (_id is null)
                {
                    _bank.GetID(out FMOD.GUID id);
                    _id = id;
                }
                
                return _id.Value;
            }
        }

        /// <summary>
        /// List all the events available in the bank.
        /// </summary>
        public ImmutableArray<EventDescription> FetchEvents()
        {
            FMOD.RESULT result = _bank.GetEventList(out FMOD.Studio.EventDescription[]? events);
            if (result != FMOD.RESULT.OK || events is null)
            {
                GameLogger.Error("Unable to list events for bank.");
                return ImmutableArray<EventDescription>.Empty;
            }

            return events.Select(e => new EventDescription(e)).ToImmutableArray();
        }

        /// <summary>
        /// List all the bus available in the bank.
        /// </summary>
        public ImmutableArray<Bus> FetchBuses()
        {
            FMOD.RESULT result = _bank.GetBusList(out FMOD.Studio.Bus[]? buses);
            if (result != FMOD.RESULT.OK || buses is null)
            {
                GameLogger.Error("Unable to list events for bank.");
                return ImmutableArray<Bus>.Empty;
            }

            return buses.Select(e => new Bus(e)).ToImmutableArray();
        }
        
        /// <summary>
        /// There are three ways to load sample data (any non-streamed sound):
        ///   - From a bank. This will keep all the bank's data in memory, until unloaded.
        ///   - From an event description. This will keep the event's necessary data in memory until unloaded.
        ///   - From an event instance. The data will only be in memory while the instance is.
        /// </summary>
        public void LoadSampleData()
        {
            FMOD.RESULT result = _bank.LoadSampleData();
            GameLogger.Verify(result == FMOD.RESULT.OK, "Unable to load sample data for target bank.");
        }

        public void Dispose()
        {
            _bank.UnloadSampleData();
            _bank.Unload();
        }
    }
}
