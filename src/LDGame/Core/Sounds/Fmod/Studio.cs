using FMOD.Studio;
using Murder.Core.Sounds;
using Murder.Diagnostics;
using System.Collections.Immutable;

namespace LDGame.Core.Sounds.Fmod
{
    internal class Studio : IDisposable
    {
        private readonly FMOD.Studio.System _studio;

        public Studio(FMOD.Studio.System studio)
        {
            _studio = studio;
        }

        /// <summary>
        /// Load a fmod bank asynchronously from a file path specified by <paramref name="path"/>.
        /// </summary>
        public async ValueTask<Bank> LoadBankAsync(
            string path, 
            LOAD_BANK_FLAGS flags = LOAD_BANK_FLAGS.NORMAL)
        {
            byte[] bytes = await File.ReadAllBytesAsync(path);

            FMOD.RESULT result = _studio.LoadBankMemory(bytes, flags, out FMOD.Studio.Bank bank);
            FmodHelpers.Check(result, $"Unable to load bank from memory for {path}.");
            
            return new(bank, Path.GetFileNameWithoutExtension(path));
        }

        public void Update()
        {
            _studio.Update();
        }

        /// <summary>
        /// List all the parameters available in the bank.
        /// </summary>
        public ImmutableArray<PARAMETER_DESCRIPTION> FetchParameters()
        {
            FMOD.RESULT result = _studio.GetParameterDescriptionList(out PARAMETER_DESCRIPTION[]? parameters);
            if (result != FMOD.RESULT.OK || parameters is null)
            {
                GameLogger.Error("Unable to list parameters for studio instance.");
                return ImmutableArray<PARAMETER_DESCRIPTION>.Empty;
            }

            return parameters.ToImmutableArray();
        }

        /// <summary>
        /// This fetches an event based on an internal guid represeted by <paramref name="id"/>.
        /// </summary>
        public EventDescription? GetEvent(SoundEventId id)
        {
            FMOD.GUID guid = id.ToFmodGuid();

            FMOD.RESULT result = _studio.GetEventByID(guid, out FMOD.Studio.EventDescription description);
            if (result != FMOD.RESULT.OK)
            {
                return null;
            }

            return new(description);
        }

        /// <summary>
        /// This fetches an event based on an internal path, i.e. "event:/UI/Cancel".
        /// </summary>
        public EventDescription? GetEvent(string path)
        {
            FMOD.RESULT result = _studio.GetEvent(path, out FMOD.Studio.EventDescription description);
            if (result != FMOD.RESULT.OK)
            {
                return null;
            }
            
            return new(description);
        }

        /// <summary>
        /// This fetches a bus based on an internal path, i.e. "bus:/UI".
        /// </summary>
        public Bus? GetBus(SoundEventId id)
        {
            FMOD.RESULT result = _studio.GetBusByID(id.ToFmodGuid(), out FMOD.Studio.Bus bus);
            if (result != FMOD.RESULT.OK)
            {
                return null;
            }

            return new(bus);
        }

        /// <summary>
        /// This fetches a bus based on an internal path, i.e. "bus:/UI".
        /// </summary>
        public Bus? GetBus(string path)
        {
            FMOD.RESULT result = _studio.GetBus(path, out FMOD.Studio.Bus bus);
            if (result != FMOD.RESULT.OK)
            {
                return null;
            }

            return new(bus);
        }

        /// <summary>
        /// Retrieves a global parameter target value.
        /// This *ignores* modulation or automation applied to the parameter within the Studio.
        /// </summary>
        /// <param name="id">Id of the global parameter.</param>
        public float GetParameterTargetValue(ParameterId id)
        {
            FMOD.RESULT result = _studio.GetParameterByID(id.ToFmodId(), out float _, out float finalValue);
            if (result != FMOD.RESULT.OK)
            {
                return -1;
            }

            return finalValue;
        }

        /// <summary>
        /// Retrieves a global parameter current value.
		/// This takes into account modulation / automation applied to the parameter within Studio.
        /// </summary>
        /// <param name="id">Id of the global parameter.</param>
        public float GetParameterCurrentValue(ParameterId id)
        {
            FMOD.RESULT result = _studio.GetParameterByID(id.ToFmodId(), out float value, out float _);
            if (result != FMOD.RESULT.OK)
            {
                return -1;
            }

            return value;
        }

        /// <summary>
        /// Set a global parameter value according to its name.
        /// </summary>
        /// <param name="name">Name of the global parameter.</param>
        /// <param name="value">Value.</param>
        /// <param name="ignoreSeekSpeed">If enable, set the value instantly, overriding its speed.</param>
        public void SetParameterValue(string name, float value, bool ignoreSeekSpeed = false)
        {
            _studio.SetParameterByName(name, value, ignoreSeekSpeed);
        }

        /// <summary>
        /// Set a global parameter value according to its id.
        /// </summary>
        /// <param name="id">Id of the global parameter.</param>
        /// <param name="value">Value.</param>
        /// <param name="ignoreSeekSpeed">If enable, set the value instantly, overriding its speed.</param>
        public void SetParameterValue(ParameterId id, float value, bool ignoreSeekSpeed = false)
        {
            _studio.SetParameterByID(id.ToFmodId(), value, ignoreSeekSpeed);
        }

        /// <summary>
        /// Set a collection of global parameters according to its id.
        /// </summary>
        /// <param name="ignoreSeekSpeed">If enable, set the value instantly, overriding its speed.</param>
        /// <param name="parameters">Collection of parameters.</param>
        public void SetParameterValues(bool ignoreSeekSpeed, params (ParameterId id, float value)[] parameters)
        {
            PARAMETER_ID[] ids = parameters.Select(i => i.id.ToFmodId()).ToArray();
            float[] values = parameters.Select(i => i.value).ToArray();

            _studio.SetParametersByIDs(ids, values, values.Length, ignoreSeekSpeed);
        }
        
        public void Dispose()
        {
            _studio.Release();
        }
    }
}
