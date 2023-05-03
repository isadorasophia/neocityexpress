using Murder.Diagnostics;
using Murder.Serialization;
using LDGame.Core.Sounds.Fmod;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO;

namespace LDGame.Core.Sounds
{
    /// <summary>
    /// This loads library into the process. I guess this is XPlat??
    /// </summary>
    public partial class LDGameSoundPlayer
    {
        private bool LoadFmodAssemblies(string resourcesPath)
        {
            string fullResourcesPath = FileHelper.GetPath(resourcesPath);
            
            string fmodPath = Path.Join(fullResourcesPath, "fmod", "pc");
            if (!Directory.Exists(fmodPath))
            {
                GameLogger.Error($"You must place fmod.dll libraries at {fmodPath} in order to run sounds.");
                return false;
            }
            else if (Directory.GetFiles(fmodPath).Length == 0)
            {
                GameLogger.Error($"You must place fmod.dll libraries at {fmodPath} in order to run sounds.");
                return false;
            }
            
            // This resolves the assembly when using the logger.
            NativeLibrary.SetDllImportResolver(typeof(LDGame).Assembly,
                (name, assembly, dllImportSearchPath) =>
            {
                name = Path.GetFileNameWithoutExtension(name);
                if (dllImportSearchPath is null)
                {
                    dllImportSearchPath = DllImportSearchPath.ApplicationDirectory;
                }

                return NativeLibrary.Load(Path.Join(fmodPath, GetLibraryName(name)));
            });

            return true;
        }

        private string GetLibraryName(string name, bool loadLogOnDebug = true)
        {
            bool isLoggingEnabled = loadLogOnDebug;

#if !DEBUG
            isLoggingEnabled = false;
#endif

            if (OperatingSystem.IsWindows())
            {
                return isLoggingEnabled ? $"{name}L.dll" : $"{name}.dll";
            }
            else if (OperatingSystem.IsLinux())
            {
                return isLoggingEnabled ? $"lib{name}L.so" : $"lib{name}.so";
            }
            else if (OperatingSystem.IsMacOS())
            {
                return isLoggingEnabled ? $"lib{name}L.dylib" : $"lib{name}.dylib";
            }
            
            // TODO: Support consoles?
            throw new PlatformNotSupportedException();
        }

        public async Task FetchBanks(string resourcesPath)
        {
            Debug.Assert(_studio is not null);

            string path = FileHelper.GetPath(Path.Join(resourcesPath, _bankRelativeToResourcesPath));
            if (!Directory.Exists(path))
            {
                // Skip loading sounds.
                GameLogger.Log($"No sounds found at {resourcesPath}");
                return;
            }

            foreach (string bankPath in Directory.EnumerateFiles(path))
            {
                Bank bank = await _studio.LoadBankAsync(bankPath);

                // bank.LoadSampleData();
                _banks.Add(bank.Id, bank);
            }
        }

        private ImmutableArray<EventDescription>? _cacheEventDescriptions = default;

        /// <summary>
        /// This fetches all the events currently available in the loaded banks by the 
        /// sound player.
        /// </summary>
        public ImmutableArray<EventDescription> ListAllEvents()
        {
            if (_cacheEventDescriptions is null)
            {
                var builder = ImmutableArray.CreateBuilder<EventDescription>();
                foreach (Bank bank in _banks.Values)
                {
                    builder.AddRange(bank.FetchEvents());
                }

                _cacheEventDescriptions = builder.ToImmutable();
            }

            return _cacheEventDescriptions.Value;
        }

        private ImmutableArray<Bus>? _cacheBuses = default;

        /// <summary>
        /// This fetches all the events currently available in the loaded banks by the 
        /// sound player.
        /// </summary>
        public ImmutableArray<Bus> ListAllBuses()
        {
            // TODO: Clean this cache on refresh?
            if (_cacheBuses is null)
            {
                var builder = ImmutableArray.CreateBuilder<Bus>();
                foreach (Bank bank in _banks.Values)
                {
                    builder.AddRange(bank.FetchBuses());
                }

                _cacheBuses = builder.ToImmutable();
            }

            return _cacheBuses.Value;
        }

        private ImmutableArray<FMOD.Studio.PARAMETER_DESCRIPTION>? _cacheParameters = default;

        /// <summary>
        /// This fetches all the events currently available in the loaded banks by the 
        /// sound player.
        /// </summary>
        public ImmutableArray<FMOD.Studio.PARAMETER_DESCRIPTION> ListAllParameters()
        {
            _cacheParameters ??= _studio?.FetchParameters();

            return _cacheParameters ?? ImmutableArray<FMOD.Studio.PARAMETER_DESCRIPTION>.Empty;
        }
    }
}
