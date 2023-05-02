/* ======================================================================================== */
/* FMOD Studio API - C# wrapper.                                                            */
/* Copyright (c), Firelight Technologies Pty, Ltd. 2004-2022.                               */
/*                                                                                          */
/* For more detail visit:                                                                   */
/* https://fmod.com/docs/2.02/api/studio-api.html                                           */
/* ======================================================================================== */

using System.Runtime.InteropServices;

namespace FMOD.Studio
{
    public partial class STUDIO_VERSION
    {
#if !UNITY_2019_4_OR_NEWER
        public const string dll = "fmodstudio";
#endif
    }

    public enum STOP_MODE : int
    {
        ALLOWFADEOUT,
        IMMEDIATE,
    }

    public enum LOADING_STATE : int
    {
        UNLOADING,
        UNLOADED,
        LOADING,
        LOADED,
        ERROR,
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PROGRAMMER_SOUND_PROPERTIES
    {
        public StringWrapper name;
        public IntPtr sound;
        public int subsoundIndex;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct TIMELINE_MARKER_PROPERTIES
    {
        public StringWrapper name;
        public int position;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct TIMELINE_BEAT_PROPERTIES
    {
        public int bar;
        public int beat;
        public int position;
        public float tempo;
        public int timesignatureupper;
        public int timesignaturelower;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct TIMELINE_NESTED_BEAT_PROPERTIES
    {
        public GUID eventid;
        public TIMELINE_BEAT_PROPERTIES properties;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ADVANCEDSETTINGS
    {
        public int cbsize;
        public int commandqueuesize;
        public int handleinitialsize;
        public int studioupdateperiod;
        public int idlesampledatapoolsize;
        public int streamingscheduledelay;
        public IntPtr encryptionkey;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct CPU_USAGE
    {
        public float update;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct BUFFER_INFO
    {
        public int currentusage;
        public int peakusage;
        public int capacity;
        public int stallcount;
        public float stalltime;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct BUFFER_USAGE
    {
        public BUFFER_INFO studiocommandqueue;
        public BUFFER_INFO studiohandle;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct BANK_INFO
    {
        public int size;
        public IntPtr userdata;
        public int userdatalength;
        public FILE_OPEN_CALLBACK opencallback;
        public FILE_CLOSE_CALLBACK closecallback;
        public FILE_READ_CALLBACK readcallback;
        public FILE_SEEK_CALLBACK seekcallback;
    }

    [Flags]
    public enum SYSTEM_CALLBACK_TYPE : uint
    {
        PREUPDATE = 0x00000001,
        POSTUPDATE = 0x00000002,
        BANK_UNLOAD = 0x00000004,
        LIVEUPDATE_CONNECTED = 0x00000008,
        LIVEUPDATE_DISCONNECTED = 0x00000010,
        ALL = 0xFFFFFFFF,
    }

    public delegate RESULT SYSTEM_CALLBACK(IntPtr system, SYSTEM_CALLBACK_TYPE type, IntPtr commanddata, IntPtr userdata);

    public enum PARAMETER_TYPE : int
    {
        GAME_CONTROLLED,
        AUTOMATIC_DISTANCE,
        AUTOMATIC_EVENT_CONE_ANGLE,
        AUTOMATIC_EVENT_ORIENTATION,
        AUTOMATIC_DIRECTION,
        AUTOMATIC_ELEVATION,
        AUTOMATIC_LISTENER_ORIENTATION,
        AUTOMATIC_SPEED,
        AUTOMATIC_SPEED_ABSOLUTE,
        AUTOMATIC_DISTANCE_NORMALIZED,
        MAX
    }

    [Flags]
    public enum PARAMETER_FLAGS : uint
    {
        READONLY = 0x00000001,
        AUTOMATIC = 0x00000002,
        GLOBAL = 0x00000004,
        DISCRETE = 0x00000008,
        LABELED = 0x00000010,
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PARAMETER_ID
    {
        public uint Data1;
        public uint Data2;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PARAMETER_DESCRIPTION
    {
        public StringWrapper Name;
        public PARAMETER_ID Id;
        public float Minimum;
        public float Maximum;
        public float DefaultValue;
        public PARAMETER_TYPE Type;
        public PARAMETER_FLAGS Flags;
        public GUID Guid;
    }

    // This is only need for loading memory and given our C# wrapper LOAD_MEMORY_POINT isn't feasible anyway
    enum LOAD_MEMORY_MODE : int
    {
        LOAD_MEMORY,
        LOAD_MEMORY_POINT,
    }

    enum LOAD_MEMORY_ALIGNMENT : int
    {
        VALUE = 32
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SOUND_INFO
    {
        public IntPtr name_or_data;
        public MODE mode;
        public CREATESOUNDEXINFO exinfo;
        public int subsoundindex;

        public string Name
        {
            get
            {
                using (StringHelper.ThreadSafeEncoding encoding = StringHelper.GetFreeHelper())
                {
                    return ((mode & (MODE.OPENMEMORY | MODE.OPENMEMORY_POINT)) == 0) ? encoding.StringFromNative(name_or_data) : String.Empty;
                }
            }
        }
    }

    public enum USER_PROPERTY_TYPE : int
    {
        INTEGER,
        BOOLEAN,
        FLOAT,
        STRING,
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct USER_PROPERTY
    {
        public StringWrapper name;
        public USER_PROPERTY_TYPE type;
        private Union_IntBoolFloatString _value;

        public int IntValue() { return (type == USER_PROPERTY_TYPE.INTEGER) ? _value.intvalue : -1; }
        public bool BoolValue() { return (type == USER_PROPERTY_TYPE.BOOLEAN) ? _value.boolvalue : false; }
        public float FloatValue() { return (type == USER_PROPERTY_TYPE.FLOAT) ? _value.floatvalue : -1; }
        public string StringValue() { return (type == USER_PROPERTY_TYPE.STRING) ? _value.stringvalue : ""; }
    };

    [StructLayout(LayoutKind.Explicit)]
    struct Union_IntBoolFloatString
    {
        [FieldOffset(0)]
        public int intvalue;
        [FieldOffset(0)]
        public bool boolvalue;
        [FieldOffset(0)]
        public float floatvalue;
        [FieldOffset(0)]
        public StringWrapper stringvalue;
    }

    [Flags]
    public enum INITFLAGS : uint
    {
        NORMAL = 0x00000000,
        LIVEUPDATE = 0x00000001,
        ALLOW_MISSING_PLUGINS = 0x00000002,
        SYNCHRONOUS_UPDATE = 0x00000004,
        DEFERRED_CALLBACKS = 0x00000008,
        LOAD_FROM_UPDATE = 0x00000010,
        MEMORY_TRACKING = 0x00000020,
    }

    [Flags]
    public enum LOAD_BANK_FLAGS : uint
    {
        NORMAL = 0x00000000,
        NONBLOCKING = 0x00000001,
        DECOMPRESS_SAMPLES = 0x00000002,
        UNENCRYPTED = 0x00000004,
    }

    [Flags]
    public enum COMMANDCAPTURE_FLAGS : uint
    {
        NORMAL = 0x00000000,
        FILEFLUSH = 0x00000001,
        SKIP_INITIAL_STATE = 0x00000002,
    }

    [Flags]
    public enum COMMANDREPLAY_FLAGS : uint
    {
        NORMAL = 0x00000000,
        SKIP_CLEANUP = 0x00000001,
        FAST_FORWARD = 0x00000002,
        SKIP_BANK_LOAD = 0x00000004,
    }

    public enum PLAYBACK_STATE : int
    {
        PLAYING,
        SUSTAINING,
        STOPPED,
        STARTING,
        STOPPING,
    }

    public enum EVENT_PROPERTY : int
    {
        CHANNELPRIORITY,
        SCHEDULE_DELAY,
        SCHEDULE_LOOKAHEAD,
        MINIMUM_DISTANCE,
        MAXIMUM_DISTANCE,
        COOLDOWN,
        MAX
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct PLUGIN_INSTANCE_PROPERTIES
    {
        public IntPtr name;
        public IntPtr dsp;
    }

    [Flags]
    public enum EVENT_CALLBACK_TYPE : uint
    {
        CREATED = 0x00000001,
        DESTROYED = 0x00000002,
        STARTING = 0x00000004,
        STARTED = 0x00000008,
        RESTARTED = 0x00000010,
        STOPPED = 0x00000020,
        START_FAILED = 0x00000040,
        CREATE_PROGRAMMER_SOUND = 0x00000080,
        DESTROY_PROGRAMMER_SOUND = 0x00000100,
        PLUGIN_CREATED = 0x00000200,
        PLUGIN_DESTROYED = 0x00000400,
        TIMELINE_MARKER = 0x00000800,
        TIMELINE_BEAT = 0x00001000,
        SOUND_PLAYED = 0x00002000,
        SOUND_STOPPED = 0x00004000,
        REAL_TO_VIRTUAL = 0x00008000,
        VIRTUAL_TO_REAL = 0x00010000,
        START_EVENT_COMMAND = 0x00020000,
        NESTED_TIMELINE_BEAT = 0x00040000,

        ALL = 0xFFFFFFFF,
    }

    public delegate RESULT EVENT_CALLBACK(EVENT_CALLBACK_TYPE type, IntPtr _event, IntPtr parameters);

    public delegate RESULT COMMANDREPLAY_FRAME_CALLBACK(IntPtr replay, int commandindex, float currenttime, IntPtr userdata);
    public delegate RESULT COMMANDREPLAY_LOAD_BANK_CALLBACK(IntPtr replay, int commandindex, GUID bankguid, IntPtr bankfilename, LOAD_BANK_FLAGS flags, out IntPtr bank, IntPtr userdata);
    public delegate RESULT COMMANDREPLAY_CREATE_INSTANCE_CALLBACK(IntPtr replay, int commandindex, IntPtr eventdescription, out IntPtr instance, IntPtr userdata);

    public enum INSTANCETYPE : int
    {
        NONE,
        SYSTEM,
        EVENTDESCRIPTION,
        EVENTINSTANCE,
        PARAMETERINSTANCE,
        BUS,
        VCA,
        BANK,
        COMMANDREPLAY,
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct COMMAND_INFO
    {
        public StringWrapper commandname;
        public int parentcommandindex;
        public int framenumber;
        public float frametime;
        public INSTANCETYPE instancetype;
        public INSTANCETYPE outputtype;
        public UInt32 instancehandle;
        public UInt32 outputhandle;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MEMORY_USAGE
    {
        public int exclusive;
        public int inclusive;
        public int sampledata;
    }

    public struct Util
    {
        public static RESULT ParseID(string idString, out GUID id)
        {
            using (StringHelper.ThreadSafeEncoding encoder = StringHelper.GetFreeHelper())
            {
                return FMOD_Studio_ParseID(encoder.ByteFromStringUTF8(idString), out id);
            }
        }

        #region importfunctions
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_ParseID(byte[]? idString, out GUID id);
        #endregion
    }

    public struct System
    {
        // Initialization / system functions.
        public static RESULT Create(out System system)
        {
            return FMOD_Studio_System_Create(out system.Handle, VERSION.number);
        }
        public RESULT SetAdvancedSettings(ADVANCEDSETTINGS settings)
        {
            settings.cbsize = MarshalHelper.SizeOf(typeof(ADVANCEDSETTINGS));
            return FMOD_Studio_System_SetAdvancedSettings(this.Handle, ref settings);
        }
        public RESULT SetAdvancedSettings(ADVANCEDSETTINGS settings, string encryptionKey)
        {
            using (StringHelper.ThreadSafeEncoding encoder = StringHelper.GetFreeHelper())
            {
                IntPtr userKey = settings.encryptionkey;
                settings.encryptionkey = encoder.IntptrFromStringUTF8(encryptionKey);
                FMOD.RESULT result = SetAdvancedSettings(settings);
                settings.encryptionkey = userKey;
                return result;
            }
        }
        public RESULT GetAdvancedSettings(out ADVANCEDSETTINGS settings)
        {
            settings.cbsize = MarshalHelper.SizeOf(typeof(ADVANCEDSETTINGS));
            return FMOD_Studio_System_GetAdvancedSettings(this.Handle, out settings);
        }
        public RESULT Initialize(int maxchannels, INITFLAGS studioflags, FMOD.INITFLAGS flags, IntPtr extradriverdata)
        {
            return FMOD_Studio_System_Initialize(this.Handle, maxchannels, studioflags, flags, extradriverdata);
        }
        public RESULT Release()
        {
            return FMOD_Studio_System_Release(this.Handle);
        }
        public RESULT Update()
        {
            return FMOD_Studio_System_Update(this.Handle);
        }
        public RESULT GetCoreSystem(out FMOD.System coresystem)
        {
            return FMOD_Studio_System_GetCoreSystem(this.Handle, out coresystem.Handle);
        }
        public RESULT GetEvent(string path, out EventDescription _event)
        {
            using (StringHelper.ThreadSafeEncoding encoder = StringHelper.GetFreeHelper())
            {
                return FMOD_Studio_System_GetEvent(this.Handle, encoder.ByteFromStringUTF8(path), out _event.Handle);
            }
        }
        public RESULT GetBus(string path, out Bus bus)
        {
            using (StringHelper.ThreadSafeEncoding encoder = StringHelper.GetFreeHelper())
            {
                return FMOD_Studio_System_GetBus(this.Handle, encoder.ByteFromStringUTF8(path), out bus.handle);
            }
        }
        public RESULT GetVCA(string path, out VCA vca)
        {
            using (StringHelper.ThreadSafeEncoding encoder = StringHelper.GetFreeHelper())
            {
                return FMOD_Studio_System_GetVCA(this.Handle, encoder.ByteFromStringUTF8(path), out vca.handle);
            }
        }
        public RESULT GetBank(string path, out Bank bank)
        {
            using (StringHelper.ThreadSafeEncoding encoder = StringHelper.GetFreeHelper())
            {
                return FMOD_Studio_System_GetBank(this.Handle, encoder.ByteFromStringUTF8(path), out bank.handle);
            }
        }

        public RESULT GetEventByID(GUID id, out EventDescription _event)
        {
            return FMOD_Studio_System_GetEventByID(this.Handle, ref id, out _event.Handle);
        }
        public RESULT GetBusByID(GUID id, out Bus bus)
        {
            return FMOD_Studio_System_GetBusByID(this.Handle, ref id, out bus.handle);
        }
        public RESULT GetVCAByID(GUID id, out VCA vca)
        {
            return FMOD_Studio_System_GetVCAByID(this.Handle, ref id, out vca.handle);
        }
        public RESULT GetBankByID(GUID id, out Bank bank)
        {
            return FMOD_Studio_System_GetBankByID(this.Handle, ref id, out bank.handle);
        }
        public RESULT GetSoundInfo(string key, out SOUND_INFO info)
        {
            using (StringHelper.ThreadSafeEncoding encoder = StringHelper.GetFreeHelper())
            {
                return FMOD_Studio_System_GetSoundInfo(this.Handle, encoder.ByteFromStringUTF8(key), out info);
            }
        }
        public RESULT GetParameterDescriptionByName(string name, out PARAMETER_DESCRIPTION parameter)
        {
            using (StringHelper.ThreadSafeEncoding encoder = StringHelper.GetFreeHelper())
            {
                return FMOD_Studio_System_GetParameterDescriptionByName(this.Handle, encoder.ByteFromStringUTF8(name), out parameter);
            }
        }
        public RESULT GetParameterDescriptionByID(PARAMETER_ID id, out PARAMETER_DESCRIPTION parameter)
        {
            return FMOD_Studio_System_GetParameterDescriptionByID(this.Handle, id, out parameter);
        }
        public RESULT GetParameterLabelByName(string name, int labelindex, out string? label)
        {
            label = null;

            using (StringHelper.ThreadSafeEncoding encoder = StringHelper.GetFreeHelper())
            {
                IntPtr stringMem = Marshal.AllocHGlobal(256);
                int retrieved = 0;
                byte[]? nameBytes = encoder.ByteFromStringUTF8(name);
                RESULT result = FMOD_Studio_System_GetParameterLabelByName(this.Handle, nameBytes, labelindex, stringMem, 256, out retrieved);

                if (result == RESULT.ERR_TRUNCATED)
                {
                    Marshal.FreeHGlobal(stringMem);
                    result = FMOD_Studio_System_GetParameterLabelByName(this.Handle, nameBytes, labelindex, IntPtr.Zero, 0, out retrieved);
                    stringMem = Marshal.AllocHGlobal(retrieved);
                    result = FMOD_Studio_System_GetParameterLabelByName(this.Handle, nameBytes, labelindex, stringMem, retrieved, out retrieved);
                }

                if (result == RESULT.OK)
                {
                    label = encoder.StringFromNative(stringMem);
                }
                Marshal.FreeHGlobal(stringMem);
                return result;
            }
        }
        public RESULT GetParameterLabelByID(PARAMETER_ID id, int labelindex, out string? label)
        {
            label = null;

            using (StringHelper.ThreadSafeEncoding encoder = StringHelper.GetFreeHelper())
            {
                IntPtr stringMem = Marshal.AllocHGlobal(256);
                int retrieved = 0;
                RESULT result = FMOD_Studio_System_GetParameterLabelByID(this.Handle, id, labelindex, stringMem, 256, out retrieved);

                if (result == RESULT.ERR_TRUNCATED)
                {
                    Marshal.FreeHGlobal(stringMem);
                    result = FMOD_Studio_System_GetParameterLabelByID(this.Handle, id, labelindex, IntPtr.Zero, 0, out retrieved);
                    stringMem = Marshal.AllocHGlobal(retrieved);
                    result = FMOD_Studio_System_GetParameterLabelByID(this.Handle, id, labelindex, stringMem, retrieved, out retrieved);
                }

                if (result == RESULT.OK)
                {
                    label = encoder.StringFromNative(stringMem);
                }
                Marshal.FreeHGlobal(stringMem);
                return result;
            }
        }
        public RESULT GetParameterByID(PARAMETER_ID id, out float value)
        {
            return GetParameterByID(id, out value, out float finalValue);
        }
        
        public RESULT GetParameterByID(PARAMETER_ID id, out float value, out float finalvalue)
        {
            return FMOD_Studio_System_GetParameterByID(this.Handle, id, out value, out finalvalue);
        }
        public RESULT SetParameterByID(PARAMETER_ID id, float value, bool ignoreseekspeed = false)
        {
            return FMOD_Studio_System_SetParameterByID(this.Handle, id, value, ignoreseekspeed);
        }
        public RESULT SetParameterByIDWithLabel(PARAMETER_ID id, string label, bool ignoreseekspeed = false)
        {
            using (StringHelper.ThreadSafeEncoding encoder = StringHelper.GetFreeHelper())
            {
                return FMOD_Studio_System_SetParameterByIDWithLabel(this.Handle, id, encoder.ByteFromStringUTF8(label), ignoreseekspeed);
            }
        }
        public RESULT SetParametersByIDs(PARAMETER_ID[] ids, float[] values, int count, bool ignoreseekspeed = false)
        {
            return FMOD_Studio_System_SetParametersByIDs(this.Handle, ids, values, count, ignoreseekspeed);
        }
        public RESULT GetParameterByName(string name, out float value)
        {
            float finalValue;
            return GetParameterByName(name, out value, out finalValue);
        }
        public RESULT GetParameterByName(string name, out float value, out float finalvalue)
        {
            using (StringHelper.ThreadSafeEncoding encoder = StringHelper.GetFreeHelper())
            {
                return FMOD_Studio_System_GetParameterByName(this.Handle, encoder.ByteFromStringUTF8(name), out value, out finalvalue);
            }
        }
        public RESULT SetParameterByName(string name, float value, bool ignoreseekspeed = false)
        {
            using (StringHelper.ThreadSafeEncoding encoder = StringHelper.GetFreeHelper())
            {
                return FMOD_Studio_System_SetParameterByName(this.Handle, encoder.ByteFromStringUTF8(name), value, ignoreseekspeed);
            }
        }
        public RESULT SetParameterByNameWithLabel(string name, string label, bool ignoreseekspeed = false)
        {
            using (StringHelper.ThreadSafeEncoding encoder = StringHelper.GetFreeHelper(),
                                                   labelEncoder = StringHelper.GetFreeHelper())
            {
                return FMOD_Studio_System_SetParameterByNameWithLabel(this.Handle, encoder.ByteFromStringUTF8(name), labelEncoder.ByteFromStringUTF8(label), ignoreseekspeed);
            }
        }
        public RESULT LookupID(string path, out GUID id)
        {
            using (StringHelper.ThreadSafeEncoding encoder = StringHelper.GetFreeHelper())
            {
                return FMOD_Studio_System_LookupID(this.Handle, encoder.ByteFromStringUTF8(path), out id);
            }
        }
        public RESULT LookupPath(GUID id, out string? path)
        {
            path = null;

            using (StringHelper.ThreadSafeEncoding encoder = StringHelper.GetFreeHelper())
            {
                IntPtr stringMem = Marshal.AllocHGlobal(256);
                int retrieved = 0;
                RESULT result = FMOD_Studio_System_LookupPath(this.Handle, ref id, stringMem, 256, out retrieved);

                if (result == RESULT.ERR_TRUNCATED)
                {
                    Marshal.FreeHGlobal(stringMem);
                    stringMem = Marshal.AllocHGlobal(retrieved);
                    result = FMOD_Studio_System_LookupPath(this.Handle, ref id, stringMem, retrieved, out retrieved);
                }

                if (result == RESULT.OK)
                {
                    path = encoder.StringFromNative(stringMem);
                }
                Marshal.FreeHGlobal(stringMem);
                return result;
            }
        }
        public RESULT GetNumListeners(out int numlisteners)
        {
            return FMOD_Studio_System_GetNumListeners(this.Handle, out numlisteners);
        }
        public RESULT SetNumListeners(int numlisteners)
        {
            return FMOD_Studio_System_SetNumListeners(this.Handle, numlisteners);
        }
        public RESULT GetListenerAttributes(int listener, out ATTRIBUTES_3D attributes)
        {
            return FMOD_Studio_System_GetListenerAttributes(this.Handle, listener, out attributes, IntPtr.Zero);
        }
        public RESULT GetListenerAttributes(int listener, out ATTRIBUTES_3D attributes, out VECTOR attenuationposition)
        {
            return FMOD_Studio_System_GetListenerAttributes(this.Handle, listener, out attributes, out attenuationposition);
        }
        public RESULT SetListenerAttributes(int listener, ATTRIBUTES_3D attributes)
        {
            return FMOD_Studio_System_SetListenerAttributes(this.Handle, listener, ref attributes, IntPtr.Zero);
        }
        public RESULT SetListenerAttributes(int listener, ATTRIBUTES_3D attributes, VECTOR attenuationposition)
        {
            return FMOD_Studio_System_SetListenerAttributes(this.Handle, listener, ref attributes, ref attenuationposition);
        }
        public RESULT GetListenerWeight(int listener, out float weight)
        {
            return FMOD_Studio_System_GetListenerWeight(this.Handle, listener, out weight);
        }
        public RESULT SetListenerWeight(int listener, float weight)
        {
            return FMOD_Studio_System_SetListenerWeight(this.Handle, listener, weight);
        }
        public RESULT LoadBankFile(string filename, LOAD_BANK_FLAGS flags, out Bank bank)
        {
            using (StringHelper.ThreadSafeEncoding encoder = StringHelper.GetFreeHelper())
            {
                return FMOD_Studio_System_LoadBankFile(this.Handle, encoder.ByteFromStringUTF8(filename), flags, out bank.handle);
            }
        }
        public RESULT LoadBankMemory(byte[] buffer, LOAD_BANK_FLAGS flags, out Bank bank)
        {
            // Manually pin the byte array. It's what the marshaller should do anyway but don't leave it to chance.
            GCHandle pinnedArray = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            IntPtr pointer = pinnedArray.AddrOfPinnedObject();
            RESULT result = FMOD_Studio_System_LoadBankMemory(this.Handle, pointer, buffer.Length, LOAD_MEMORY_MODE.LOAD_MEMORY, flags, out bank.handle);
            pinnedArray.Free();
            return result;
        }
        public RESULT LoadBankCustom(BANK_INFO info, LOAD_BANK_FLAGS flags, out Bank bank)
        {
            info.size = MarshalHelper.SizeOf(typeof(BANK_INFO));
            return FMOD_Studio_System_LoadBankCustom(this.Handle, ref info, flags, out bank.handle);
        }
        public RESULT UnloadAll()
        {
            return FMOD_Studio_System_UnloadAll(this.Handle);
        }
        public RESULT FlushCommands()
        {
            return FMOD_Studio_System_FlushCommands(this.Handle);
        }
        public RESULT FlushSampleLoading()
        {
            return FMOD_Studio_System_FlushSampleLoading(this.Handle);
        }
        public RESULT StartCommandCapture(string filename, COMMANDCAPTURE_FLAGS flags)
        {
            using (StringHelper.ThreadSafeEncoding encoder = StringHelper.GetFreeHelper())
            {
                return FMOD_Studio_System_StartCommandCapture(this.Handle, encoder.ByteFromStringUTF8(filename), flags);
            }
        }
        public RESULT StopCommandCapture()
        {
            return FMOD_Studio_System_StopCommandCapture(this.Handle);
        }
        public RESULT LoadCommandReplay(string filename, COMMANDREPLAY_FLAGS flags, out CommandReplay replay)
        {
            using (StringHelper.ThreadSafeEncoding encoder = StringHelper.GetFreeHelper())
            {
                return FMOD_Studio_System_LoadCommandReplay(this.Handle, encoder.ByteFromStringUTF8(filename), flags, out replay.Handle);
            }
        }
        public RESULT GetBankCount(out int count)
        {
            return FMOD_Studio_System_GetBankCount(this.Handle, out count);
        }
        public RESULT GetBankList(out Bank[]? array)
        {
            array = null;

            RESULT result;
            int capacity;
            result = FMOD_Studio_System_GetBankCount(this.Handle, out capacity);
            if (result != RESULT.OK)
            {
                return result;
            }
            if (capacity == 0)
            {
                array = new Bank[0];
                return result;
            }

            IntPtr[] rawArray = new IntPtr[capacity];
            int actualCount;
            result = FMOD_Studio_System_GetBankList(this.Handle, rawArray, capacity, out actualCount);
            if (result != RESULT.OK)
            {
                return result;
            }
            if (actualCount > capacity) // More items added since we queried just now?
            {
                actualCount = capacity;
            }
            array = new Bank[actualCount];
            for (int i = 0; i < actualCount; ++i)
            {
                array[i].handle = rawArray[i];
            }
            return RESULT.OK;
        }
        public RESULT GetParameterDescriptionCount(out int count)
        {
            return FMOD_Studio_System_GetParameterDescriptionCount(this.Handle, out count);
        }
        public RESULT GetParameterDescriptionList(out PARAMETER_DESCRIPTION[]? array)
        {
            array = null;

            int capacity;
            RESULT result = FMOD_Studio_System_GetParameterDescriptionCount(this.Handle, out capacity);
            if (result != RESULT.OK)
            {
                return result;
            }
            if (capacity == 0)
            {
                array = new PARAMETER_DESCRIPTION[0];
                return RESULT.OK;
            }

            PARAMETER_DESCRIPTION[] tempArray = new PARAMETER_DESCRIPTION[capacity];
            int actualCount;
            result = FMOD_Studio_System_GetParameterDescriptionList(this.Handle, tempArray, capacity, out actualCount);
            if (result != RESULT.OK)
            {
                return result;
            }

            if (actualCount != capacity)
            {
                Array.Resize(ref tempArray, actualCount);
            }

            array = tempArray;

            return RESULT.OK;
        }
        public RESULT GetCPUUsage(out CPU_USAGE usage, out FMOD.CPU_USAGE usage_core)
        {
            return FMOD_Studio_System_GetCPUUsage(this.Handle, out usage, out usage_core);
        }
        public RESULT GetBufferUsage(out BUFFER_USAGE usage)
        {
            return FMOD_Studio_System_GetBufferUsage(this.Handle, out usage);
        }
        public RESULT ResetBufferUsage()
        {
            return FMOD_Studio_System_ResetBufferUsage(this.Handle);
        }

        public RESULT SetCallback(SYSTEM_CALLBACK callback, SYSTEM_CALLBACK_TYPE callbackmask = SYSTEM_CALLBACK_TYPE.ALL)
        {
            return FMOD_Studio_System_SetCallback(this.Handle, callback, callbackmask);
        }

        public RESULT GetUserData(out IntPtr userdata)
        {
            return FMOD_Studio_System_GetUserData(this.Handle, out userdata);
        }

        public RESULT SetUserData(IntPtr userdata)
        {
            return FMOD_Studio_System_SetUserData(this.Handle, userdata);
        }

        public RESULT GetMemoryUsage(out MEMORY_USAGE memoryusage)
        {
            return FMOD_Studio_System_GetMemoryUsage(this.Handle, out memoryusage);
        }

        #region importfunctions
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_System_Create(out IntPtr system, uint headerversion);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern bool FMOD_Studio_System_IsValid(IntPtr system);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_System_SetAdvancedSettings(IntPtr system, ref ADVANCEDSETTINGS settings);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_System_GetAdvancedSettings(IntPtr system, out ADVANCEDSETTINGS settings);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_System_Initialize(IntPtr system, int maxchannels, INITFLAGS studioflags, FMOD.INITFLAGS flags, IntPtr extradriverdata);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_System_Release(IntPtr system);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_System_Update(IntPtr system);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_System_GetCoreSystem(IntPtr system, out IntPtr coresystem);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_System_GetEvent(IntPtr system, byte[]? path, out IntPtr _event);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_System_GetBus(IntPtr system, byte[]? path, out IntPtr bus);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_System_GetVCA(IntPtr system, byte[]? path, out IntPtr vca);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_System_GetBank(IntPtr system, byte[]? path, out IntPtr bank);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_System_GetEventByID(IntPtr system, ref GUID id, out IntPtr _event);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_System_GetBusByID(IntPtr system, ref GUID id, out IntPtr bus);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_System_GetVCAByID(IntPtr system, ref GUID id, out IntPtr vca);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_System_GetBankByID(IntPtr system, ref GUID id, out IntPtr bank);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_System_GetSoundInfo(IntPtr system, byte[]? key, out SOUND_INFO info);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_System_GetParameterDescriptionByName(IntPtr system, byte[]? name, out PARAMETER_DESCRIPTION parameter);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_System_GetParameterDescriptionByID(IntPtr system, PARAMETER_ID id, out PARAMETER_DESCRIPTION parameter);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_System_GetParameterLabelByName(IntPtr system, byte[]? name, int labelindex, IntPtr label, int size, out int retrieved);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_System_GetParameterLabelByID(IntPtr system, PARAMETER_ID id, int labelindex, IntPtr label, int size, out int retrieved);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_System_GetParameterByID(IntPtr system, PARAMETER_ID id, out float value, out float finalvalue);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_System_SetParameterByID(IntPtr system, PARAMETER_ID id, float value, bool ignoreseekspeed);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_System_SetParameterByIDWithLabel(IntPtr system, PARAMETER_ID id, byte[]? label, bool ignoreseekspeed);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_System_SetParametersByIDs(IntPtr system, PARAMETER_ID[] ids, float[] values, int count, bool ignoreseekspeed);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_System_GetParameterByName(IntPtr system, byte[]? name, out float value, out float finalvalue);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_System_SetParameterByName(IntPtr system, byte[]? name, float value, bool ignoreseekspeed);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_System_SetParameterByNameWithLabel(IntPtr system, byte[]? name, byte[]? label, bool ignoreseekspeed);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_System_LookupID(IntPtr system, byte[]? path, out GUID id);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_System_LookupPath(IntPtr system, ref GUID id, IntPtr path, int size, out int retrieved);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_System_GetNumListeners(IntPtr system, out int numlisteners);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_System_SetNumListeners(IntPtr system, int numlisteners);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_System_GetListenerAttributes(IntPtr system, int listener, out ATTRIBUTES_3D attributes, IntPtr zero);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_System_GetListenerAttributes(IntPtr system, int listener, out ATTRIBUTES_3D attributes, out VECTOR attenuationposition);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_System_SetListenerAttributes(IntPtr system, int listener, ref ATTRIBUTES_3D attributes, IntPtr zero);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_System_SetListenerAttributes(IntPtr system, int listener, ref ATTRIBUTES_3D attributes, ref VECTOR attenuationposition);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_System_GetListenerWeight(IntPtr system, int listener, out float weight);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_System_SetListenerWeight(IntPtr system, int listener, float weight);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_System_LoadBankFile(IntPtr system, byte[]? filename, LOAD_BANK_FLAGS flags, out IntPtr bank);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_System_LoadBankMemory(IntPtr system, IntPtr buffer, int length, LOAD_MEMORY_MODE mode, LOAD_BANK_FLAGS flags, out IntPtr bank);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_System_LoadBankCustom(IntPtr system, ref BANK_INFO info, LOAD_BANK_FLAGS flags, out IntPtr bank);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_System_UnloadAll(IntPtr system);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_System_FlushCommands(IntPtr system);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_System_FlushSampleLoading(IntPtr system);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_System_StartCommandCapture(IntPtr system, byte[]? filename, COMMANDCAPTURE_FLAGS flags);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_System_StopCommandCapture(IntPtr system);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_System_LoadCommandReplay(IntPtr system, byte[]? filename, COMMANDREPLAY_FLAGS flags, out IntPtr replay);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_System_GetBankCount(IntPtr system, out int count);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_System_GetBankList(IntPtr system, IntPtr[] array, int capacity, out int count);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_System_GetParameterDescriptionCount(IntPtr system, out int count);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_System_GetParameterDescriptionList(IntPtr system, [Out] PARAMETER_DESCRIPTION[] array, int capacity, out int count);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_System_GetCPUUsage(IntPtr system, out CPU_USAGE usage, out FMOD.CPU_USAGE usage_core);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_System_GetBufferUsage(IntPtr system, out BUFFER_USAGE usage);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_System_ResetBufferUsage(IntPtr system);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_System_SetCallback(IntPtr system, SYSTEM_CALLBACK callback, SYSTEM_CALLBACK_TYPE callbackmask);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_System_GetUserData(IntPtr system, out IntPtr userdata);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_System_SetUserData(IntPtr system, IntPtr userdata);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_System_GetMemoryUsage(IntPtr system, out MEMORY_USAGE memoryusage);
        #endregion

        #region wrapperinternal

        public IntPtr Handle;

        public System(IntPtr ptr) { this.Handle = ptr; }
        public bool HasHandle() { return this.Handle != IntPtr.Zero; }
        public void ClearHandle() { this.Handle = IntPtr.Zero; }

        public bool IsValid()
        {
            return HasHandle() && FMOD_Studio_System_IsValid(this.Handle);
        }

        #endregion
    }

    public struct EventDescription
    {
        public RESULT GetID(out GUID id)
        {
            return FMOD_Studio_EventDescription_GetID(this.Handle, out id);
        }
        public RESULT GetPath(out string? path)
        {
            path = null;

            using (StringHelper.ThreadSafeEncoding encoder = StringHelper.GetFreeHelper())
            {
                IntPtr stringMem = Marshal.AllocHGlobal(256);
                int retrieved = 0;
                RESULT result = FMOD_Studio_EventDescription_GetPath(this.Handle, stringMem, 256, out retrieved);

                if (result == RESULT.ERR_TRUNCATED)
                {
                    Marshal.FreeHGlobal(stringMem);
                    stringMem = Marshal.AllocHGlobal(retrieved);
                    result = FMOD_Studio_EventDescription_GetPath(this.Handle, stringMem, retrieved, out retrieved);
                }

                if (result == RESULT.OK)
                {
                    path = encoder.StringFromNative(stringMem);
                }
                Marshal.FreeHGlobal(stringMem);
                return result;
            }
        }
        public RESULT GetParameterDescriptionCount(out int count)
        {
            return FMOD_Studio_EventDescription_GetParameterDescriptionCount(this.Handle, out count);
        }
        public RESULT GetParameterDescriptionByIndex(int index, out PARAMETER_DESCRIPTION parameter)
        {
            return FMOD_Studio_EventDescription_GetParameterDescriptionByIndex(this.Handle, index, out parameter);
        }
        public RESULT GetParameterDescriptionByName(string name, out PARAMETER_DESCRIPTION parameter)
        {
            using (StringHelper.ThreadSafeEncoding encoder = StringHelper.GetFreeHelper())
            {
                return FMOD_Studio_EventDescription_GetParameterDescriptionByName(this.Handle, encoder.ByteFromStringUTF8(name), out parameter);
            }
        }
        public RESULT GetParameterDescriptionByID(PARAMETER_ID id, out PARAMETER_DESCRIPTION parameter)
        {
            return FMOD_Studio_EventDescription_GetParameterDescriptionByID(this.Handle, id, out parameter);
        }
        public RESULT GetParameterLabelByIndex(int index, int labelindex, out string? label)
        {
            label = null;

            using (StringHelper.ThreadSafeEncoding encoder = StringHelper.GetFreeHelper())
            {
                IntPtr stringMem = Marshal.AllocHGlobal(256);
                int retrieved = 0;
                RESULT result = FMOD_Studio_EventDescription_GetParameterLabelByIndex(this.Handle, index, labelindex, stringMem, 256, out retrieved);

                if (result == RESULT.ERR_TRUNCATED)
                {
                    Marshal.FreeHGlobal(stringMem);
                    result = FMOD_Studio_EventDescription_GetParameterLabelByIndex(this.Handle, index, labelindex, IntPtr.Zero, 0, out retrieved);
                    stringMem = Marshal.AllocHGlobal(retrieved);
                    result = FMOD_Studio_EventDescription_GetParameterLabelByIndex(this.Handle, index, labelindex, stringMem, retrieved, out retrieved);
                }

                if (result == RESULT.OK)
                {
                    label = encoder.StringFromNative(stringMem);
                }
                Marshal.FreeHGlobal(stringMem);
                return result;
            }
        }
        public RESULT GetParameterLabelByName(string name, int labelindex, out string? label)
        {
            label = null;

            using (StringHelper.ThreadSafeEncoding encoder = StringHelper.GetFreeHelper())
            {
                IntPtr stringMem = Marshal.AllocHGlobal(256);
                int retrieved = 0;
                byte[]? nameBytes = encoder.ByteFromStringUTF8(name);
                RESULT result = FMOD_Studio_EventDescription_GetParameterLabelByName(this.Handle, nameBytes, labelindex, stringMem, 256, out retrieved);

                if (result == RESULT.ERR_TRUNCATED)
                {
                    Marshal.FreeHGlobal(stringMem);
                    result = FMOD_Studio_EventDescription_GetParameterLabelByName(this.Handle, nameBytes, labelindex, IntPtr.Zero, 0, out retrieved);
                    stringMem = Marshal.AllocHGlobal(retrieved);
                    result = FMOD_Studio_EventDescription_GetParameterLabelByName(this.Handle, nameBytes, labelindex, stringMem, retrieved, out retrieved);
                }

                if (result == RESULT.OK)
                {
                    label = encoder.StringFromNative(stringMem);
                }
                Marshal.FreeHGlobal(stringMem);
                return result;
            }
        }
        public RESULT GetParameterLabelByID(PARAMETER_ID id, int labelindex, out string? label)
        {
            label = null;

            using (StringHelper.ThreadSafeEncoding encoder = StringHelper.GetFreeHelper())
            {
                IntPtr stringMem = Marshal.AllocHGlobal(256);
                int retrieved = 0;
                RESULT result = FMOD_Studio_EventDescription_GetParameterLabelByID(this.Handle, id, labelindex, stringMem, 256, out retrieved);

                if (result == RESULT.ERR_TRUNCATED)
                {
                    Marshal.FreeHGlobal(stringMem);
                    result = FMOD_Studio_EventDescription_GetParameterLabelByID(this.Handle, id, labelindex, IntPtr.Zero, 0, out retrieved);
                    stringMem = Marshal.AllocHGlobal(retrieved);
                    result = FMOD_Studio_EventDescription_GetParameterLabelByID(this.Handle, id, labelindex, stringMem, retrieved, out retrieved);
                }

                if (result == RESULT.OK)
                {
                    label = encoder.StringFromNative(stringMem);
                }
                Marshal.FreeHGlobal(stringMem);
                return result;
            }
        }
        public RESULT GetUserPropertyCount(out int count)
        {
            return FMOD_Studio_EventDescription_GetUserPropertyCount(this.Handle, out count);
        }
        public RESULT GetUserPropertyByIndex(int index, out USER_PROPERTY property)
        {
            return FMOD_Studio_EventDescription_GetUserPropertyByIndex(this.Handle, index, out property);
        }
        public RESULT GetUserProperty(string name, out USER_PROPERTY property)
        {
            using (StringHelper.ThreadSafeEncoding encoder = StringHelper.GetFreeHelper())
            {
                return FMOD_Studio_EventDescription_GetUserProperty(this.Handle, encoder.ByteFromStringUTF8(name), out property);
            }
        }
        public RESULT GetLength(out int length)
        {
            return FMOD_Studio_EventDescription_GetLength(this.Handle, out length);
        }
        public RESULT GetMinMaxDistance(out float min, out float max)
        {
            return FMOD_Studio_EventDescription_GetMinMaxDistance(this.Handle, out min, out max);
        }
        public RESULT GetSoundSize(out float size)
        {
            return FMOD_Studio_EventDescription_GetSoundSize(this.Handle, out size);
        }
        public RESULT IsSnapshot(out bool snapshot)
        {
            return FMOD_Studio_EventDescription_IsSnapshot(this.Handle, out snapshot);
        }
        public RESULT IsOneshot(out bool oneshot)
        {
            return FMOD_Studio_EventDescription_IsOneshot(this.Handle, out oneshot);
        }
        public RESULT IsStream(out bool isStream)
        {
            return FMOD_Studio_EventDescription_IsStream(this.Handle, out isStream);
        }
        public RESULT Is3D(out bool is3D)
        {
            return FMOD_Studio_EventDescription_Is3D(this.Handle, out is3D);
        }
        public RESULT IsDopplerEnabled(out bool doppler)
        {
            return FMOD_Studio_EventDescription_IsDopplerEnabled(this.Handle, out doppler);
        }
        public RESULT HasSustainPoint(out bool sustainPoint)
        {
            return FMOD_Studio_EventDescription_HasSustainPoint(this.Handle, out sustainPoint);
        }

        public RESULT CreateInstance(out EventInstance instance)
        {
            return FMOD_Studio_EventDescription_CreateInstance(this.Handle, out instance.Handle);
        }

        public RESULT GetInstanceCount(out int count)
        {
            return FMOD_Studio_EventDescription_GetInstanceCount(this.Handle, out count);
        }
        public RESULT GetInstanceList(out EventInstance[]? array)
        {
            array = null;

            RESULT result;
            int capacity;
            result = FMOD_Studio_EventDescription_GetInstanceCount(this.Handle, out capacity);
            if (result != RESULT.OK)
            {
                return result;
            }
            if (capacity == 0)
            {
                array = new EventInstance[0];
                return result;
            }

            IntPtr[] rawArray = new IntPtr[capacity];
            int actualCount;
            result = FMOD_Studio_EventDescription_GetInstanceList(this.Handle, rawArray, capacity, out actualCount);
            if (result != RESULT.OK)
            {
                return result;
            }
            if (actualCount > capacity) // More items added since we queried just now?
            {
                actualCount = capacity;
            }
            array = new EventInstance[actualCount];
            for (int i = 0; i < actualCount; ++i)
            {
                array[i].Handle = rawArray[i];
            }
            return RESULT.OK;
        }

        public RESULT LoadSampleData()
        {
            return FMOD_Studio_EventDescription_LoadSampleData(this.Handle);
        }

        public RESULT UnloadSampleData()
        {
            return FMOD_Studio_EventDescription_UnloadSampleData(this.Handle);
        }

        public RESULT GetSampleLoadingState(out LOADING_STATE state)
        {
            return FMOD_Studio_EventDescription_GetSampleLoadingState(this.Handle, out state);
        }

        public RESULT ReleaseAllInstances()
        {
            return FMOD_Studio_EventDescription_ReleaseAllInstances(this.Handle);
        }
        public RESULT SetCallback(EVENT_CALLBACK callback, EVENT_CALLBACK_TYPE callbackmask = EVENT_CALLBACK_TYPE.ALL)
        {
            return FMOD_Studio_EventDescription_SetCallback(this.Handle, callback, callbackmask);
        }

        public RESULT GetUserData(out IntPtr userdata)
        {
            return FMOD_Studio_EventDescription_GetUserData(this.Handle, out userdata);
        }

        public RESULT SetUserData(IntPtr userdata)
        {
            return FMOD_Studio_EventDescription_SetUserData(this.Handle, userdata);
        }

        #region importfunctions
        [DllImport(STUDIO_VERSION.dll)]
        private static extern bool FMOD_Studio_EventDescription_IsValid(IntPtr eventdescription);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_EventDescription_GetID(IntPtr eventdescription, out GUID id);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_EventDescription_GetPath(IntPtr eventdescription, IntPtr path, int size, out int retrieved);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_EventDescription_GetParameterDescriptionCount(IntPtr eventdescription, out int count);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_EventDescription_GetParameterDescriptionByIndex(IntPtr eventdescription, int index, out PARAMETER_DESCRIPTION parameter);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_EventDescription_GetParameterDescriptionByName(IntPtr eventdescription, byte[]? name, out PARAMETER_DESCRIPTION parameter);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_EventDescription_GetParameterDescriptionByID(IntPtr eventdescription, PARAMETER_ID id, out PARAMETER_DESCRIPTION parameter);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_EventDescription_GetParameterLabelByIndex(IntPtr eventdescription, int index, int labelindex, IntPtr label, int size, out int retrieved);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_EventDescription_GetParameterLabelByName(IntPtr eventdescription, byte[]? name, int labelindex, IntPtr label, int size, out int retrieved);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_EventDescription_GetParameterLabelByID(IntPtr eventdescription, PARAMETER_ID id, int labelindex, IntPtr label, int size, out int retrieved);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_EventDescription_GetUserPropertyCount(IntPtr eventdescription, out int count);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_EventDescription_GetUserPropertyByIndex(IntPtr eventdescription, int index, out USER_PROPERTY property);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_EventDescription_GetUserProperty(IntPtr eventdescription, byte[]? name, out USER_PROPERTY property);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_EventDescription_GetLength(IntPtr eventdescription, out int length);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_EventDescription_GetMinMaxDistance(IntPtr eventdescription, out float min, out float max);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_EventDescription_GetSoundSize(IntPtr eventdescription, out float size);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_EventDescription_IsSnapshot(IntPtr eventdescription, out bool snapshot);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_EventDescription_IsOneshot(IntPtr eventdescription, out bool oneshot);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_EventDescription_IsStream(IntPtr eventdescription, out bool isStream);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_EventDescription_Is3D(IntPtr eventdescription, out bool is3D);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_EventDescription_IsDopplerEnabled(IntPtr eventdescription, out bool doppler);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_EventDescription_HasSustainPoint(IntPtr eventdescription, out bool sustainPoint);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_EventDescription_CreateInstance(IntPtr eventdescription, out IntPtr instance);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_EventDescription_GetInstanceCount(IntPtr eventdescription, out int count);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_EventDescription_GetInstanceList(IntPtr eventdescription, IntPtr[] array, int capacity, out int count);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_EventDescription_LoadSampleData(IntPtr eventdescription);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_EventDescription_UnloadSampleData(IntPtr eventdescription);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_EventDescription_GetSampleLoadingState(IntPtr eventdescription, out LOADING_STATE state);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_EventDescription_ReleaseAllInstances(IntPtr eventdescription);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_EventDescription_SetCallback(IntPtr eventdescription, EVENT_CALLBACK callback, EVENT_CALLBACK_TYPE callbackmask);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_EventDescription_GetUserData(IntPtr eventdescription, out IntPtr userdata);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_EventDescription_SetUserData(IntPtr eventdescription, IntPtr userdata);
        #endregion
        #region wrapperinternal

        public IntPtr Handle;

        public EventDescription(IntPtr ptr) { this.Handle = ptr; }
        public bool HasHandle() { return this.Handle != IntPtr.Zero; }
        public void ClearHandle() { this.Handle = IntPtr.Zero; }

        public bool IsValid()
        {
            return HasHandle() && FMOD_Studio_EventDescription_IsValid(this.Handle);
        }

        #endregion
    }

    public struct EventInstance
    {
        public RESULT GetDescription(out EventDescription description)
        {
            return FMOD_Studio_EventInstance_GetDescription(this.Handle, out description.Handle);
        }
        public RESULT GetVolume(out float volume)
        {
            return FMOD_Studio_EventInstance_GetVolume(this.Handle, out volume, IntPtr.Zero);
        }
        public RESULT GetVolume(out float volume, out float finalvolume)
        {
            return FMOD_Studio_EventInstance_GetVolume(this.Handle, out volume, out finalvolume);
        }
        public RESULT SetVolume(float volume)
        {
            return FMOD_Studio_EventInstance_SetVolume(this.Handle, volume);
        }
        public RESULT GetPitch(out float pitch)
        {
            return FMOD_Studio_EventInstance_GetPitch(this.Handle, out pitch, IntPtr.Zero);
        }
        public RESULT GetPitch(out float pitch, out float finalpitch)
        {
            return FMOD_Studio_EventInstance_GetPitch(this.Handle, out pitch, out finalpitch);
        }
        public RESULT SetPitch(float pitch)
        {
            return FMOD_Studio_EventInstance_SetPitch(this.Handle, pitch);
        }
        public RESULT Get3DAttributes(out ATTRIBUTES_3D attributes)
        {
            return FMOD_Studio_EventInstance_Get3DAttributes(this.Handle, out attributes);
        }
        public RESULT Set3DAttributes(ATTRIBUTES_3D attributes)
        {
            return FMOD_Studio_EventInstance_Set3DAttributes(this.Handle, ref attributes);
        }
        public RESULT GetListenerMask(out uint mask)
        {
            return FMOD_Studio_EventInstance_GetListenerMask(this.Handle, out mask);
        }
        public RESULT SetListenerMask(uint mask)
        {
            return FMOD_Studio_EventInstance_SetListenerMask(this.Handle, mask);
        }
        public RESULT GetProperty(EVENT_PROPERTY index, out float value)
        {
            return FMOD_Studio_EventInstance_GetProperty(this.Handle, index, out value);
        }
        public RESULT SetProperty(EVENT_PROPERTY index, float value)
        {
            return FMOD_Studio_EventInstance_SetProperty(this.Handle, index, value);
        }
        public RESULT GetReverbLevel(int index, out float level)
        {
            return FMOD_Studio_EventInstance_GetReverbLevel(this.Handle, index, out level);
        }
        public RESULT SetReverbLevel(int index, float level)
        {
            return FMOD_Studio_EventInstance_SetReverbLevel(this.Handle, index, level);
        }
        public RESULT GetPaused(out bool paused)
        {
            return FMOD_Studio_EventInstance_GetPaused(this.Handle, out paused);
        }
        public RESULT SetPaused(bool paused)
        {
            return FMOD_Studio_EventInstance_SetPaused(this.Handle, paused);
        }
        public RESULT Start()
        {
            return FMOD_Studio_EventInstance_Start(this.Handle);
        }
        public RESULT Stop(STOP_MODE mode)
        {
            return FMOD_Studio_EventInstance_Stop(this.Handle, mode);
        }
        public RESULT GetTimelinePosition(out int position)
        {
            return FMOD_Studio_EventInstance_GetTimelinePosition(this.Handle, out position);
        }
        public RESULT SetTimelinePosition(int position)
        {
            return FMOD_Studio_EventInstance_SetTimelinePosition(this.Handle, position);
        }
        public RESULT GetPlaybackState(out PLAYBACK_STATE state)
        {
            return FMOD_Studio_EventInstance_GetPlaybackState(this.Handle, out state);
        }
        public RESULT GetChannelGroup(out FMOD.ChannelGroup group)
        {
            return FMOD_Studio_EventInstance_GetChannelGroup(this.Handle, out group.handle);
        }
        public RESULT GetMinMaxDistance(out float min, out float max)
        {
            return FMOD_Studio_EventInstance_GetMinMaxDistance(this.Handle, out min, out max);
        }
        public RESULT Release()
        {
            return FMOD_Studio_EventInstance_Release(this.Handle);
        }
        public RESULT IsVirtual(out bool virtualstate)
        {
            return FMOD_Studio_EventInstance_IsVirtual(this.Handle, out virtualstate);
        }
        public RESULT GetParameterByID(PARAMETER_ID id, out float value)
        {
            float finalvalue;
            return GetParameterByID(id, out value, out finalvalue);
        }
        public RESULT GetParameterByID(PARAMETER_ID id, out float value, out float finalvalue)
        {
            return FMOD_Studio_EventInstance_GetParameterByID(this.Handle, id, out value, out finalvalue);
        }
        public RESULT SetParameterByID(PARAMETER_ID id, float value, bool ignoreseekspeed = false)
        {
            return FMOD_Studio_EventInstance_SetParameterByID(this.Handle, id, value, ignoreseekspeed);
        }
        public RESULT SetParameterByIDWithLabel(PARAMETER_ID id, string label, bool ignoreseekspeed = false)
        {
            using (StringHelper.ThreadSafeEncoding encoder = StringHelper.GetFreeHelper())
            {
                return FMOD_Studio_EventInstance_SetParameterByIDWithLabel(this.Handle, id, encoder.ByteFromStringUTF8(label), ignoreseekspeed);
            }
        }
        public RESULT SetParametersByIDs(PARAMETER_ID[] ids, float[] values, int count, bool ignoreseekspeed = false)
        {
            return FMOD_Studio_EventInstance_SetParametersByIDs(this.Handle, ids, values, count, ignoreseekspeed);
        }
        public RESULT GetParameterByName(string name, out float value)
        {
            float finalValue;
            return GetParameterByName(name, out value, out finalValue);
        }
        public RESULT GetParameterByName(string name, out float value, out float finalvalue)
        {
            using (StringHelper.ThreadSafeEncoding encoder = StringHelper.GetFreeHelper())
            {
                return FMOD_Studio_EventInstance_GetParameterByName(this.Handle, encoder.ByteFromStringUTF8(name), out value, out finalvalue);
            }
        }
        public RESULT SetParameterByName(string name, float value, bool ignoreseekspeed = false)
        {
            using (StringHelper.ThreadSafeEncoding encoder = StringHelper.GetFreeHelper())
            {
                return FMOD_Studio_EventInstance_SetParameterByName(this.Handle, encoder.ByteFromStringUTF8(name), value, ignoreseekspeed);
            }
        }
        public RESULT SetParameterByNameWithLabel(string name, string label, bool ignoreseekspeed = false)
        {
            using (StringHelper.ThreadSafeEncoding encoder = StringHelper.GetFreeHelper(),
                                                   labelEncoder = StringHelper.GetFreeHelper())
            {
                return FMOD_Studio_EventInstance_SetParameterByNameWithLabel(this.Handle, encoder.ByteFromStringUTF8(name), labelEncoder.ByteFromStringUTF8(label), ignoreseekspeed);
            }
        }
        public RESULT KeyOff()
        {
            return FMOD_Studio_EventInstance_KeyOff(this.Handle);
        }
        public RESULT SetCallback(EVENT_CALLBACK callback, EVENT_CALLBACK_TYPE callbackmask = EVENT_CALLBACK_TYPE.ALL)
        {
            return FMOD_Studio_EventInstance_SetCallback(this.Handle, callback, callbackmask);
        }
        public RESULT GetUserData(out IntPtr userdata)
        {
            return FMOD_Studio_EventInstance_GetUserData(this.Handle, out userdata);
        }
        public RESULT SetUserData(IntPtr userdata)
        {
            return FMOD_Studio_EventInstance_SetUserData(this.Handle, userdata);
        }
        public RESULT GetCPUUsage(out uint exclusive, out uint inclusive)
        {
            return FMOD_Studio_EventInstance_GetCPUUsage(this.Handle, out exclusive, out inclusive);
        }
        public RESULT GetMemoryUsage(out MEMORY_USAGE memoryusage)
        {
            return FMOD_Studio_EventInstance_GetMemoryUsage(this.Handle, out memoryusage);
        }
        #region importfunctions
        [DllImport(STUDIO_VERSION.dll)]
        private static extern bool FMOD_Studio_EventInstance_IsValid(IntPtr _event);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_EventInstance_GetDescription(IntPtr _event, out IntPtr description);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_EventInstance_GetVolume(IntPtr _event, out float volume, IntPtr zero);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_EventInstance_GetVolume(IntPtr _event, out float volume, out float finalvolume);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_EventInstance_SetVolume(IntPtr _event, float volume);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_EventInstance_GetPitch(IntPtr _event, out float pitch, IntPtr zero);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_EventInstance_GetPitch(IntPtr _event, out float pitch, out float finalpitch);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_EventInstance_SetPitch(IntPtr _event, float pitch);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_EventInstance_Get3DAttributes(IntPtr _event, out ATTRIBUTES_3D attributes);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_EventInstance_Set3DAttributes(IntPtr _event, ref ATTRIBUTES_3D attributes);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_EventInstance_GetListenerMask(IntPtr _event, out uint mask);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_EventInstance_SetListenerMask(IntPtr _event, uint mask);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_EventInstance_GetProperty(IntPtr _event, EVENT_PROPERTY index, out float value);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_EventInstance_SetProperty(IntPtr _event, EVENT_PROPERTY index, float value);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_EventInstance_GetReverbLevel(IntPtr _event, int index, out float level);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_EventInstance_SetReverbLevel(IntPtr _event, int index, float level);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_EventInstance_GetPaused(IntPtr _event, out bool paused);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_EventInstance_SetPaused(IntPtr _event, bool paused);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_EventInstance_Start(IntPtr _event);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_EventInstance_Stop(IntPtr _event, STOP_MODE mode);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_EventInstance_GetTimelinePosition(IntPtr _event, out int position);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_EventInstance_SetTimelinePosition(IntPtr _event, int position);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_EventInstance_GetPlaybackState(IntPtr _event, out PLAYBACK_STATE state);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_EventInstance_GetChannelGroup(IntPtr _event, out IntPtr group);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_EventInstance_GetMinMaxDistance(IntPtr _event, out float min, out float max);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_EventInstance_Release(IntPtr _event);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_EventInstance_IsVirtual(IntPtr _event, out bool virtualstate);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_EventInstance_GetParameterByName(IntPtr _event, byte[]? name, out float value, out float finalvalue);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_EventInstance_SetParameterByName(IntPtr _event, byte[]? name, float value, bool ignoreseekspeed);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_EventInstance_SetParameterByNameWithLabel(IntPtr _event, byte[]? name, byte[]? label, bool ignoreseekspeed);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_EventInstance_GetParameterByID(IntPtr _event, PARAMETER_ID id, out float value, out float finalvalue);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_EventInstance_SetParameterByID(IntPtr _event, PARAMETER_ID id, float value, bool ignoreseekspeed);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_EventInstance_SetParameterByIDWithLabel(IntPtr _event, PARAMETER_ID id, byte[]? label, bool ignoreseekspeed);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_EventInstance_SetParametersByIDs(IntPtr _event, PARAMETER_ID[] ids, float[] values, int count, bool ignoreseekspeed);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_EventInstance_KeyOff(IntPtr _event);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_EventInstance_SetCallback(IntPtr _event, EVENT_CALLBACK callback, EVENT_CALLBACK_TYPE callbackmask);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_EventInstance_GetUserData(IntPtr _event, out IntPtr userdata);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_EventInstance_SetUserData(IntPtr _event, IntPtr userdata);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_EventInstance_GetCPUUsage(IntPtr _event, out uint exclusive, out uint inclusive);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_EventInstance_GetMemoryUsage(IntPtr _event, out MEMORY_USAGE memoryusage);
        #endregion

        #region wrapperinternal

        public IntPtr Handle;

        public EventInstance(IntPtr ptr) { this.Handle = ptr; }
        public bool HasHandle() { return this.Handle != IntPtr.Zero; }
        public void ClearHandle() { this.Handle = IntPtr.Zero; }
        
        public bool IsValid()
        {
            return HasHandle() && FMOD_Studio_EventInstance_IsValid(this.Handle);
        }

        #endregion
    }

    public struct Bus
    {
        public RESULT GetID(out GUID id)
        {
            return FMOD_Studio_Bus_GetID(this.handle, out id);
        }
        public RESULT GetPath(out string? path)
        {
            path = null;

            using (StringHelper.ThreadSafeEncoding encoder = StringHelper.GetFreeHelper())
            {
                IntPtr stringMem = Marshal.AllocHGlobal(256);
                int retrieved = 0;
                RESULT result = FMOD_Studio_Bus_GetPath(this.handle, stringMem, 256, out retrieved);

                if (result == RESULT.ERR_TRUNCATED)
                {
                    Marshal.FreeHGlobal(stringMem);
                    stringMem = Marshal.AllocHGlobal(retrieved);
                    result = FMOD_Studio_Bus_GetPath(this.handle, stringMem, retrieved, out retrieved);
                }

                if (result == RESULT.OK)
                {
                    path = encoder.StringFromNative(stringMem);
                }
                Marshal.FreeHGlobal(stringMem);
                return result;
            }

        }
        public RESULT GetVolume(out float volume)
        {
            float finalVolume;
            return GetVolume(out volume, out finalVolume);
        }
        public RESULT GetVolume(out float volume, out float finalvolume)
        {
            return FMOD_Studio_Bus_GetVolume(this.handle, out volume, out finalvolume);
        }
        public RESULT SetVolume(float volume)
        {
            return FMOD_Studio_Bus_SetVolume(this.handle, volume);
        }
        public RESULT GetPaused(out bool paused)
        {
            return FMOD_Studio_Bus_GetPaused(this.handle, out paused);
        }
        public RESULT SetPaused(bool paused)
        {
            return FMOD_Studio_Bus_SetPaused(this.handle, paused);
        }
        public RESULT GetMute(out bool mute)
        {
            return FMOD_Studio_Bus_GetMute(this.handle, out mute);
        }
        public RESULT SetMute(bool mute)
        {
            return FMOD_Studio_Bus_SetMute(this.handle, mute);
        }
        public RESULT StopAllEvents(STOP_MODE mode)
        {
            return FMOD_Studio_Bus_StopAllEvents(this.handle, mode);
        }
        public RESULT LockChannelGroup()
        {
            return FMOD_Studio_Bus_LockChannelGroup(this.handle);
        }
        public RESULT UnlockChannelGroup()
        {
            return FMOD_Studio_Bus_UnlockChannelGroup(this.handle);
        }
        public RESULT GetChannelGroup(out FMOD.ChannelGroup group)
        {
            return FMOD_Studio_Bus_GetChannelGroup(this.handle, out group.handle);
        }
        public RESULT GetCPUUsage(out uint exclusive, out uint inclusive)
        {
            return FMOD_Studio_Bus_GetCPUUsage(this.handle, out exclusive, out inclusive);
        }
        public RESULT GetMemoryUsage(out MEMORY_USAGE memoryusage)
        {
            return FMOD_Studio_Bus_GetMemoryUsage(this.handle, out memoryusage);
        }
        public RESULT GetPortIndex(out ulong index)
        {
            return FMOD_Studio_Bus_GetPortIndex(this.handle, out index);
        }
        public RESULT SetPortIndex(ulong index)
        {
            return FMOD_Studio_Bus_SetPortIndex(this.handle, index);
        }

        #region importfunctions
        [DllImport(STUDIO_VERSION.dll)]
        private static extern bool FMOD_Studio_Bus_IsValid(IntPtr bus);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_Bus_GetID(IntPtr bus, out GUID id);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_Bus_GetPath(IntPtr bus, IntPtr path, int size, out int retrieved);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_Bus_GetVolume(IntPtr bus, out float volume, out float finalvolume);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_Bus_SetVolume(IntPtr bus, float volume);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_Bus_GetPaused(IntPtr bus, out bool paused);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_Bus_SetPaused(IntPtr bus, bool paused);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_Bus_GetMute(IntPtr bus, out bool mute);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_Bus_SetMute(IntPtr bus, bool mute);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_Bus_StopAllEvents(IntPtr bus, STOP_MODE mode);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_Bus_LockChannelGroup(IntPtr bus);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_Bus_UnlockChannelGroup(IntPtr bus);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_Bus_GetChannelGroup(IntPtr bus, out IntPtr group);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_Bus_GetCPUUsage(IntPtr bus, out uint exclusive, out uint inclusive);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_Bus_GetMemoryUsage(IntPtr bus, out MEMORY_USAGE memoryusage);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_Bus_GetPortIndex(IntPtr bus, out ulong index);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_Bus_SetPortIndex(IntPtr bus, ulong index);
        #endregion

        #region wrapperinternal

        public IntPtr handle;

        public Bus(IntPtr ptr) { this.handle = ptr; }
        public bool HasHandle() { return this.handle != IntPtr.Zero; }
        public void ClearHandle() { this.handle = IntPtr.Zero; }
        
        public bool IsValid()
        {
            return HasHandle() && FMOD_Studio_Bus_IsValid(this.handle);
        }

        #endregion
    }

    public struct VCA
    {
        public RESULT GetID(out GUID id)
        {
            return FMOD_Studio_VCA_GetID(this.handle, out id);
        }
        public RESULT GetPath(out string? path)
        {
            path = null;

            using (StringHelper.ThreadSafeEncoding encoder = StringHelper.GetFreeHelper())
            {
                IntPtr stringMem = Marshal.AllocHGlobal(256);
                int retrieved = 0;
                RESULT result = FMOD_Studio_VCA_GetPath(this.handle, stringMem, 256, out retrieved);

                if (result == RESULT.ERR_TRUNCATED)
                {
                    Marshal.FreeHGlobal(stringMem);
                    stringMem = Marshal.AllocHGlobal(retrieved);
                    result = FMOD_Studio_VCA_GetPath(this.handle, stringMem, retrieved, out retrieved);
                }

                if (result == RESULT.OK)
                {
                    path = encoder.StringFromNative(stringMem);
                }
                Marshal.FreeHGlobal(stringMem);
                return result;
            }
        }
        public RESULT GetVolume(out float volume)
        {
            float finalVolume;
            return GetVolume(out volume, out finalVolume);
        }
        public RESULT GetVolume(out float volume, out float finalvolume)
        {
            return FMOD_Studio_VCA_GetVolume(this.handle, out volume, out finalvolume);
        }
        public RESULT SetVolume(float volume)
        {
            return FMOD_Studio_VCA_SetVolume(this.handle, volume);
        }

        #region importfunctions
        [DllImport(STUDIO_VERSION.dll)]
        private static extern bool FMOD_Studio_VCA_IsValid(IntPtr vca);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_VCA_GetID(IntPtr vca, out GUID id);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_VCA_GetPath(IntPtr vca, IntPtr path, int size, out int retrieved);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_VCA_GetVolume(IntPtr vca, out float volume, out float finalvolume);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_VCA_SetVolume(IntPtr vca, float volume);
        #endregion

        #region wrapperinternal

        public IntPtr handle;

        public VCA(IntPtr ptr) { this.handle = ptr; }
        public bool HasHandle() { return this.handle != IntPtr.Zero; }
        public void ClearHandle() { this.handle = IntPtr.Zero; }

        public bool IsValid()
        {
            return HasHandle() && FMOD_Studio_VCA_IsValid(this.handle);
        }

        #endregion
    }

    public struct Bank
    {
        // Property access

        public RESULT GetID(out GUID id)
        {
            return FMOD_Studio_Bank_GetID(this.handle, out id);
        }
        public RESULT GetPath(out string? path)
        {
            path = null;

            using (StringHelper.ThreadSafeEncoding encoder = StringHelper.GetFreeHelper())
            {
                IntPtr stringMem = Marshal.AllocHGlobal(256);
                int retrieved = 0;
                RESULT result = FMOD_Studio_Bank_GetPath(this.handle, stringMem, 256, out retrieved);

                if (result == RESULT.ERR_TRUNCATED)
                {
                    Marshal.FreeHGlobal(stringMem);
                    stringMem = Marshal.AllocHGlobal(retrieved);
                    result = FMOD_Studio_Bank_GetPath(this.handle, stringMem, retrieved, out retrieved);
                }

                if (result == RESULT.OK)
                {
                    path = encoder.StringFromNative(stringMem);
                }
                Marshal.FreeHGlobal(stringMem);
                return result;
            }
        }
        public RESULT Unload()
        {
            return FMOD_Studio_Bank_Unload(this.handle);
        }
        public RESULT LoadSampleData()
        {
            return FMOD_Studio_Bank_LoadSampleData(this.handle);
        }
        public RESULT UnloadSampleData()
        {
            return FMOD_Studio_Bank_UnloadSampleData(this.handle);
        }
        public RESULT GetLoadingState(out LOADING_STATE state)
        {
            return FMOD_Studio_Bank_GetLoadingState(this.handle, out state);
        }
        public RESULT GetSampleLoadingState(out LOADING_STATE state)
        {
            return FMOD_Studio_Bank_GetSampleLoadingState(this.handle, out state);
        }

        // Enumeration
        public RESULT GetStringCount(out int count)
        {
            return FMOD_Studio_Bank_GetStringCount(this.handle, out count);
        }
        public RESULT GetStringInfo(int index, out GUID id, out string? path)
        {
            path = null;
            id = new GUID();

            using (StringHelper.ThreadSafeEncoding encoder = StringHelper.GetFreeHelper())
            {
                IntPtr stringMem = Marshal.AllocHGlobal(256);
                int retrieved = 0;
                RESULT result = FMOD_Studio_Bank_GetStringInfo(this.handle, index, out id, stringMem, 256, out retrieved);

                if (result == RESULT.ERR_TRUNCATED)
                {
                    Marshal.FreeHGlobal(stringMem);
                    stringMem = Marshal.AllocHGlobal(retrieved);
                    result = FMOD_Studio_Bank_GetStringInfo(this.handle, index, out id, stringMem, retrieved, out retrieved);
                }

                if (result == RESULT.OK)
                {
                    path = encoder.StringFromNative(stringMem);
                }
                Marshal.FreeHGlobal(stringMem);
                return result;
            }
        }

        public RESULT GetEventCount(out int count)
        {
            return FMOD_Studio_Bank_GetEventCount(this.handle, out count);
        }
        public RESULT GetEventList(out EventDescription[]? array)
        {
            array = null;

            RESULT result;
            int capacity;
            result = FMOD_Studio_Bank_GetEventCount(this.handle, out capacity);
            if (result != RESULT.OK)
            {
                return result;
            }
            if (capacity == 0)
            {
                array = new EventDescription[0];
                return result;
            }

            IntPtr[] rawArray = new IntPtr[capacity];
            int actualCount;
            result = FMOD_Studio_Bank_GetEventList(this.handle, rawArray, capacity, out actualCount);
            if (result != RESULT.OK)
            {
                return result;
            }
            if (actualCount > capacity) // More items added since we queried just now?
            {
                actualCount = capacity;
            }
            array = new EventDescription[actualCount];
            for (int i = 0; i < actualCount; ++i)
            {
                array[i].Handle = rawArray[i];
            }
            return RESULT.OK;
        }
        public RESULT GetBusCount(out int count)
        {
            return FMOD_Studio_Bank_GetBusCount(this.handle, out count);
        }
        public RESULT GetBusList(out Bus[]? array)
        {
            array = null;

            RESULT result;
            int capacity;
            result = FMOD_Studio_Bank_GetBusCount(this.handle, out capacity);
            if (result != RESULT.OK)
            {
                return result;
            }
            if (capacity == 0)
            {
                array = new Bus[0];
                return result;
            }

            IntPtr[] rawArray = new IntPtr[capacity];
            int actualCount;
            result = FMOD_Studio_Bank_GetBusList(this.handle, rawArray, capacity, out actualCount);
            if (result != RESULT.OK)
            {
                return result;
            }
            if (actualCount > capacity) // More items added since we queried just now?
            {
                actualCount = capacity;
            }
            array = new Bus[actualCount];
            for (int i = 0; i < actualCount; ++i)
            {
                array[i].handle = rawArray[i];
            }
            return RESULT.OK;
        }
        public RESULT GetVCACount(out int count)
        {
            return FMOD_Studio_Bank_GetVCACount(this.handle, out count);
        }
        public RESULT GetVCAList(out VCA[]? array)
        {
            array = null;

            RESULT result;
            int capacity;
            result = FMOD_Studio_Bank_GetVCACount(this.handle, out capacity);
            if (result != RESULT.OK)
            {
                return result;
            }
            if (capacity == 0)
            {
                array = new VCA[0];
                return result;
            }

            IntPtr[] rawArray = new IntPtr[capacity];
            int actualCount;
            result = FMOD_Studio_Bank_GetVCAList(this.handle, rawArray, capacity, out actualCount);
            if (result != RESULT.OK)
            {
                return result;
            }
            if (actualCount > capacity) // More items added since we queried just now?
            {
                actualCount = capacity;
            }
            array = new VCA[actualCount];
            for (int i = 0; i < actualCount; ++i)
            {
                array[i].handle = rawArray[i];
            }
            return RESULT.OK;
        }

        public RESULT GetUserData(out IntPtr userdata)
        {
            return FMOD_Studio_Bank_GetUserData(this.handle, out userdata);
        }

        public RESULT SetUserData(IntPtr userdata)
        {
            return FMOD_Studio_Bank_SetUserData(this.handle, userdata);
        }

        #region importfunctions
        [DllImport(STUDIO_VERSION.dll)]
        private static extern bool FMOD_Studio_Bank_IsValid(IntPtr bank);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_Bank_GetID(IntPtr bank, out GUID id);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_Bank_GetPath(IntPtr bank, IntPtr path, int size, out int retrieved);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_Bank_Unload(IntPtr bank);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_Bank_LoadSampleData(IntPtr bank);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_Bank_UnloadSampleData(IntPtr bank);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_Bank_GetLoadingState(IntPtr bank, out LOADING_STATE state);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_Bank_GetSampleLoadingState(IntPtr bank, out LOADING_STATE state);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_Bank_GetStringCount(IntPtr bank, out int count);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_Bank_GetStringInfo(IntPtr bank, int index, out GUID id, IntPtr path, int size, out int retrieved);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_Bank_GetEventCount(IntPtr bank, out int count);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_Bank_GetEventList(IntPtr bank, IntPtr[] array, int capacity, out int count);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_Bank_GetBusCount(IntPtr bank, out int count);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_Bank_GetBusList(IntPtr bank, IntPtr[] array, int capacity, out int count);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_Bank_GetVCACount(IntPtr bank, out int count);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_Bank_GetVCAList(IntPtr bank, IntPtr[] array, int capacity, out int count);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_Bank_GetUserData(IntPtr bank, out IntPtr userdata);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_Bank_SetUserData(IntPtr bank, IntPtr userdata);
        #endregion

        #region wrapperinternal

        public IntPtr handle;

        public Bank(IntPtr ptr) { this.handle = ptr; }
        public bool HasHandle() { return this.handle != IntPtr.Zero; }
        public void ClearHandle() { this.handle = IntPtr.Zero; }

        public bool IsValid()
        {
            return HasHandle() && FMOD_Studio_Bank_IsValid(this.handle);
        }

        #endregion
    }

    public struct CommandReplay
    {
        // Information query
        public RESULT GetSystem(out System system)
        {
            return FMOD_Studio_CommandReplay_GetSystem(this.Handle, out system.Handle);
        }

        public RESULT GetLength(out float length)
        {
            return FMOD_Studio_CommandReplay_GetLength(this.Handle, out length);
        }
        public RESULT GetCommandCount(out int count)
        {
            return FMOD_Studio_CommandReplay_GetCommandCount(this.Handle, out count);
        }
        public RESULT GetCommandInfo(int commandIndex, out COMMAND_INFO info)
        {
            return FMOD_Studio_CommandReplay_GetCommandInfo(this.Handle, commandIndex, out info);
        }

        public RESULT GetCommandString(int commandIndex, out string? buffer)
        {
            buffer = null;
            using (StringHelper.ThreadSafeEncoding encoder = StringHelper.GetFreeHelper())
            {
                int stringLength = 256;
                IntPtr stringMem = Marshal.AllocHGlobal(256);
                RESULT result = FMOD_Studio_CommandReplay_GetCommandString(this.Handle, commandIndex, stringMem, stringLength);

                while (result == RESULT.ERR_TRUNCATED)
                {
                    Marshal.FreeHGlobal(stringMem);
                    stringLength *= 2;
                    stringMem = Marshal.AllocHGlobal(stringLength);
                    result = FMOD_Studio_CommandReplay_GetCommandString(this.Handle, commandIndex, stringMem, stringLength);
                }

                if (result == RESULT.OK)
                {
                    buffer = encoder.StringFromNative(stringMem);
                }
                Marshal.FreeHGlobal(stringMem);
                return result;
            }
        }
        public RESULT GetCommandAtTime(float time, out int commandIndex)
        {
            return FMOD_Studio_CommandReplay_GetCommandAtTime(this.Handle, time, out commandIndex);
        }
        // Playback
        public RESULT SetBankPath(string bankPath)
        {
            using (StringHelper.ThreadSafeEncoding encoder = StringHelper.GetFreeHelper())
            {
                return FMOD_Studio_CommandReplay_SetBankPath(this.Handle, encoder.ByteFromStringUTF8(bankPath));
            }
        }
        public RESULT Start()
        {
            return FMOD_Studio_CommandReplay_Start(this.Handle);
        }
        public RESULT Stop()
        {
            return FMOD_Studio_CommandReplay_Stop(this.Handle);
        }
        public RESULT SeekToTime(float time)
        {
            return FMOD_Studio_CommandReplay_SeekToTime(this.Handle, time);
        }
        public RESULT SeekToCommand(int commandIndex)
        {
            return FMOD_Studio_CommandReplay_SeekToCommand(this.Handle, commandIndex);
        }
        public RESULT GetPaused(out bool paused)
        {
            return FMOD_Studio_CommandReplay_GetPaused(this.Handle, out paused);
        }
        public RESULT SetPaused(bool paused)
        {
            return FMOD_Studio_CommandReplay_SetPaused(this.Handle, paused);
        }
        public RESULT GetPlaybackState(out PLAYBACK_STATE state)
        {
            return FMOD_Studio_CommandReplay_GetPlaybackState(this.Handle, out state);
        }
        public RESULT GetCurrentCommand(out int commandIndex, out float currentTime)
        {
            return FMOD_Studio_CommandReplay_GetCurrentCommand(this.Handle, out commandIndex, out currentTime);
        }
        // Release
        public RESULT Release()
        {
            return FMOD_Studio_CommandReplay_Release(this.Handle);
        }
        // Callbacks
        public RESULT SetFrameCallback(COMMANDREPLAY_FRAME_CALLBACK callback)
        {
            return FMOD_Studio_CommandReplay_SetFrameCallback(this.Handle, callback);
        }
        public RESULT SetLoadBankCallback(COMMANDREPLAY_LOAD_BANK_CALLBACK callback)
        {
            return FMOD_Studio_CommandReplay_SetLoadBankCallback(this.Handle, callback);
        }
        public RESULT SetCreateInstanceCallback(COMMANDREPLAY_CREATE_INSTANCE_CALLBACK callback)
        {
            return FMOD_Studio_CommandReplay_SetCreateInstanceCallback(this.Handle, callback);
        }
        public RESULT GetUserData(out IntPtr userdata)
        {
            return FMOD_Studio_CommandReplay_GetUserData(this.Handle, out userdata);
        }
        public RESULT SetUserData(IntPtr userdata)
        {
            return FMOD_Studio_CommandReplay_SetUserData(this.Handle, userdata);
        }

        #region importfunctions
        [DllImport(STUDIO_VERSION.dll)]
        private static extern bool FMOD_Studio_CommandReplay_IsValid(IntPtr replay);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_CommandReplay_GetSystem(IntPtr replay, out IntPtr system);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_CommandReplay_GetLength(IntPtr replay, out float length);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_CommandReplay_GetCommandCount(IntPtr replay, out int count);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_CommandReplay_GetCommandInfo(IntPtr replay, int commandindex, out COMMAND_INFO info);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_CommandReplay_GetCommandString(IntPtr replay, int commandIndex, IntPtr buffer, int length);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_CommandReplay_GetCommandAtTime(IntPtr replay, float time, out int commandIndex);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_CommandReplay_SetBankPath(IntPtr replay, byte[]? bankPath);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_CommandReplay_Start(IntPtr replay);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_CommandReplay_Stop(IntPtr replay);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_CommandReplay_SeekToTime(IntPtr replay, float time);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_CommandReplay_SeekToCommand(IntPtr replay, int commandIndex);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_CommandReplay_GetPaused(IntPtr replay, out bool paused);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_CommandReplay_SetPaused(IntPtr replay, bool paused);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_CommandReplay_GetPlaybackState(IntPtr replay, out PLAYBACK_STATE state);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_CommandReplay_GetCurrentCommand(IntPtr replay, out int commandIndex, out float currentTime);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_CommandReplay_Release(IntPtr replay);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_CommandReplay_SetFrameCallback(IntPtr replay, COMMANDREPLAY_FRAME_CALLBACK callback);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_CommandReplay_SetLoadBankCallback(IntPtr replay, COMMANDREPLAY_LOAD_BANK_CALLBACK callback);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_CommandReplay_SetCreateInstanceCallback(IntPtr replay, COMMANDREPLAY_CREATE_INSTANCE_CALLBACK callback);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_CommandReplay_GetUserData(IntPtr replay, out IntPtr userdata);
        [DllImport(STUDIO_VERSION.dll)]
        private static extern RESULT FMOD_Studio_CommandReplay_SetUserData(IntPtr replay, IntPtr userdata);
        #endregion

        #region wrapperinternal

        public IntPtr Handle;

        public CommandReplay(IntPtr ptr) { this.Handle = ptr; }
        public bool HasHandle() { return this.Handle != IntPtr.Zero; }
        public void ClearHandle() { this.Handle = IntPtr.Zero; }

        public bool IsValid()
        {
            return HasHandle() && FMOD_Studio_CommandReplay_IsValid(this.Handle);
        }

        #endregion
    }
} // FMOD
