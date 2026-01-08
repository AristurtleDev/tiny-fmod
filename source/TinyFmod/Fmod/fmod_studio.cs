/* ======================================================================================== */
/* FMOD Studio API - C# wrapper.                                                            */
/* Copyright (c), Firelight Technologies Pty, Ltd. 2004-2022.                               */
/*                                                                                          */
/* For more detail visit:                                                                   */
/* https://fmod.com/docs/2.02/api/studio-api.html                                           */
/* ======================================================================================== */

using System.Runtime.InteropServices;

#nullable disable

namespace FMOD.Studio;

public partial class STUDIO_VERSION
{
#if !UNITY_2019_4_OR_NEWER
    public const string dll     = "fmodstudio";
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
    READONLY      = 0x00000001,
    AUTOMATIC     = 0x00000002,
    GLOBAL        = 0x00000004,
    DISCRETE      = 0x00000008,
    LABELED       = 0x00000010,
}

[StructLayout(LayoutKind.Sequential)]
public struct PARAMETER_ID
{
    public uint data1;
    public uint data2;
}

[StructLayout(LayoutKind.Sequential)]
public struct PARAMETER_DESCRIPTION
{
    public StringWrapper name;
    public PARAMETER_ID id;
    public float minimum;
    public float maximum;
    public float defaultvalue;
    public PARAMETER_TYPE type;
    public PARAMETER_FLAGS flags;
    public GUID guid;
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

    public readonly string name
    {
        get
        {
            using StringHelper.ThreadSafeEncoding encoding = StringHelper.GetFreeHelper();
            return ((mode & (MODE.OPENMEMORY | MODE.OPENMEMORY_POINT)) == 0) ? encoding.stringFromNative(name_or_data) : String.Empty;
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
    private Union_IntBoolFloatString value;

    public readonly int intValue() => (type == USER_PROPERTY_TYPE.INTEGER) ? value.intvalue : -1;
    public readonly bool boolValue() => (type == USER_PROPERTY_TYPE.BOOLEAN) ? value.boolvalue : false;
    public readonly float floatValue() => (type == USER_PROPERTY_TYPE.FLOAT) ? value.floatvalue : -1;
    public readonly string stringValue() => (type == USER_PROPERTY_TYPE.STRING) ? value.stringvalue : "";
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
    NORMAL                  = 0x00000000,
    LIVEUPDATE              = 0x00000001,
    ALLOW_MISSING_PLUGINS   = 0x00000002,
    SYNCHRONOUS_UPDATE      = 0x00000004,
    DEFERRED_CALLBACKS      = 0x00000008,
    LOAD_FROM_UPDATE        = 0x00000010,
    MEMORY_TRACKING         = 0x00000020,
}

[Flags]
public enum LOAD_BANK_FLAGS : uint
{
    NORMAL                  = 0x00000000,
    NONBLOCKING             = 0x00000001,
    DECOMPRESS_SAMPLES      = 0x00000002,
    UNENCRYPTED             = 0x00000004,
}

[Flags]
public enum COMMANDCAPTURE_FLAGS : uint
{
    NORMAL                  = 0x00000000,
    FILEFLUSH               = 0x00000001,
    SKIP_INITIAL_STATE      = 0x00000002,
}

[Flags]
public enum COMMANDREPLAY_FLAGS : uint
{
    NORMAL                  = 0x00000000,
    SKIP_CLEANUP            = 0x00000001,
    FAST_FORWARD            = 0x00000002,
    SKIP_BANK_LOAD          = 0x00000004,
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
    CREATED                  = 0x00000001,
    DESTROYED                = 0x00000002,
    STARTING                 = 0x00000004,
    STARTED                  = 0x00000008,
    RESTARTED                = 0x00000010,
    STOPPED                  = 0x00000020,
    START_FAILED             = 0x00000040,
    CREATE_PROGRAMMER_SOUND  = 0x00000080,
    DESTROY_PROGRAMMER_SOUND = 0x00000100,
    PLUGIN_CREATED           = 0x00000200,
    PLUGIN_DESTROYED         = 0x00000400,
    TIMELINE_MARKER          = 0x00000800,
    TIMELINE_BEAT            = 0x00001000,
    SOUND_PLAYED             = 0x00002000,
    SOUND_STOPPED            = 0x00004000,
    REAL_TO_VIRTUAL          = 0x00008000,
    VIRTUAL_TO_REAL          = 0x00010000,
    START_EVENT_COMMAND      = 0x00020000,
    NESTED_TIMELINE_BEAT     = 0x00040000,

    ALL                      = 0xFFFFFFFF,
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
    public static RESULT parseID(string idString, out GUID id)
    {
        using StringHelper.ThreadSafeEncoding encoder = StringHelper.GetFreeHelper();
        return FMOD_Studio_ParseID(encoder.byteFromStringUTF8(idString), out id);
    }

    #region importfunctions
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_ParseID(byte[] idString, out GUID id);
    #endregion
}

public struct System(IntPtr ptr)
{
    // Initialization / system functions.
    public static RESULT create(out System system) => FMOD_Studio_System_Create(out system.handle, VERSION.number);
    public readonly RESULT setAdvancedSettings(ADVANCEDSETTINGS settings)
    {
        settings.cbsize = MarshalHelper.SizeOf(typeof(ADVANCEDSETTINGS));
        return FMOD_Studio_System_SetAdvancedSettings(this.handle, ref settings);
    }
    public readonly RESULT setAdvancedSettings(ADVANCEDSETTINGS settings, string encryptionKey)
    {
        using StringHelper.ThreadSafeEncoding encoder = StringHelper.GetFreeHelper();
        IntPtr userKey = settings.encryptionkey;
        settings.encryptionkey = encoder.intptrFromStringUTF8(encryptionKey);
        FMOD.RESULT result = setAdvancedSettings(settings);
        settings.encryptionkey = userKey;
        return result;
    }
    public readonly RESULT getAdvancedSettings(out ADVANCEDSETTINGS settings)
    {
        settings.cbsize = MarshalHelper.SizeOf(typeof(ADVANCEDSETTINGS));
        return FMOD_Studio_System_GetAdvancedSettings(this.handle, out settings);
    }
    public readonly RESULT initialize(int maxchannels, INITFLAGS studioflags, FMOD.INITFLAGS flags, IntPtr extradriverdata) => FMOD_Studio_System_Initialize(this.handle, maxchannels, studioflags, flags, extradriverdata);
    public readonly RESULT release() => FMOD_Studio_System_Release(this.handle);
    public readonly RESULT update() => FMOD_Studio_System_Update(this.handle);
    public readonly RESULT getCoreSystem(out FMOD.System coresystem) => FMOD_Studio_System_GetCoreSystem(this.handle, out coresystem.handle);
    public readonly RESULT getEvent(string path, out EventDescription _event)
    {
        using StringHelper.ThreadSafeEncoding encoder = StringHelper.GetFreeHelper();
        return FMOD_Studio_System_GetEvent(this.handle, encoder.byteFromStringUTF8(path), out _event.handle);
    }
    public readonly RESULT getBus(string path, out Bus bus)
    {
        using StringHelper.ThreadSafeEncoding encoder = StringHelper.GetFreeHelper();
        return FMOD_Studio_System_GetBus(this.handle, encoder.byteFromStringUTF8(path), out bus.handle);
    }
    public readonly RESULT getVCA(string path, out VCA vca)
    {
        using StringHelper.ThreadSafeEncoding encoder = StringHelper.GetFreeHelper();
        return FMOD_Studio_System_GetVCA(this.handle, encoder.byteFromStringUTF8(path), out vca.handle);
    }
    public readonly RESULT getBank(string path, out Bank bank)
    {
        using StringHelper.ThreadSafeEncoding encoder = StringHelper.GetFreeHelper();
        return FMOD_Studio_System_GetBank(this.handle, encoder.byteFromStringUTF8(path), out bank.handle);
    }

    public readonly RESULT getEventByID(GUID id, out EventDescription _event) => FMOD_Studio_System_GetEventByID(this.handle, ref id, out _event.handle);
    public readonly RESULT getBusByID(GUID id, out Bus bus) => FMOD_Studio_System_GetBusByID(this.handle, ref id, out bus.handle);
    public readonly RESULT getVCAByID(GUID id, out VCA vca) => FMOD_Studio_System_GetVCAByID(this.handle, ref id, out vca.handle);
    public readonly RESULT getBankByID(GUID id, out Bank bank) => FMOD_Studio_System_GetBankByID(this.handle, ref id, out bank.handle);
    public readonly RESULT getSoundInfo(string key, out SOUND_INFO info)
    {
        using StringHelper.ThreadSafeEncoding encoder = StringHelper.GetFreeHelper();
        return FMOD_Studio_System_GetSoundInfo(this.handle, encoder.byteFromStringUTF8(key), out info);
    }
    public readonly RESULT getParameterDescriptionByName(string name, out PARAMETER_DESCRIPTION parameter)
    {
        using StringHelper.ThreadSafeEncoding encoder = StringHelper.GetFreeHelper();
        return FMOD_Studio_System_GetParameterDescriptionByName(this.handle, encoder.byteFromStringUTF8(name), out parameter);
    }
    public readonly RESULT getParameterDescriptionByID(PARAMETER_ID id, out PARAMETER_DESCRIPTION parameter) => FMOD_Studio_System_GetParameterDescriptionByID(this.handle, id, out parameter);
    public readonly RESULT getParameterLabelByName(string name, int labelindex, out string label)
    {
        label = null;

        using StringHelper.ThreadSafeEncoding encoder = StringHelper.GetFreeHelper();
        IntPtr stringMem = Marshal.AllocHGlobal(256);
        byte[] nameBytes = encoder.byteFromStringUTF8(name);
        RESULT result = FMOD_Studio_System_GetParameterLabelByName(this.handle, nameBytes, labelindex, stringMem, 256, out int retrieved);

        if (result == RESULT.ERR_TRUNCATED)
        {
            Marshal.FreeHGlobal(stringMem);
            result = FMOD_Studio_System_GetParameterLabelByName(this.handle, nameBytes, labelindex, IntPtr.Zero, 0, out retrieved);
            stringMem = Marshal.AllocHGlobal(retrieved);
            result = FMOD_Studio_System_GetParameterLabelByName(this.handle, nameBytes, labelindex, stringMem, retrieved, out retrieved);
        }

        if (result == RESULT.OK)
        {
            label = encoder.stringFromNative(stringMem);
        }
        Marshal.FreeHGlobal(stringMem);
        return result;
    }
    public readonly RESULT getParameterLabelByID(PARAMETER_ID id, int labelindex, out string label)
    {
        label = null;

        using StringHelper.ThreadSafeEncoding encoder = StringHelper.GetFreeHelper();
        IntPtr stringMem = Marshal.AllocHGlobal(256);
        RESULT result = FMOD_Studio_System_GetParameterLabelByID(this.handle, id, labelindex, stringMem, 256, out int retrieved);

        if (result == RESULT.ERR_TRUNCATED)
        {
            Marshal.FreeHGlobal(stringMem);
            result = FMOD_Studio_System_GetParameterLabelByID(this.handle, id, labelindex, IntPtr.Zero, 0, out retrieved);
            stringMem = Marshal.AllocHGlobal(retrieved);
            result = FMOD_Studio_System_GetParameterLabelByID(this.handle, id, labelindex, stringMem, retrieved, out retrieved);
        }

        if (result == RESULT.OK)
        {
            label = encoder.stringFromNative(stringMem);
        }
        Marshal.FreeHGlobal(stringMem);
        return result;
    }
    public readonly RESULT getParameterByID(PARAMETER_ID id, out float value) => getParameterByID(id, out value, out _);
    public readonly RESULT getParameterByID(PARAMETER_ID id, out float value, out float finalvalue) => FMOD_Studio_System_GetParameterByID(this.handle, id, out value, out finalvalue);
    public readonly RESULT setParameterByID(PARAMETER_ID id, float value, bool ignoreseekspeed = false) => FMOD_Studio_System_SetParameterByID(this.handle, id, value, ignoreseekspeed);
    public readonly RESULT setParameterByIDWithLabel(PARAMETER_ID id, string label, bool ignoreseekspeed = false)
    {
        using StringHelper.ThreadSafeEncoding encoder = StringHelper.GetFreeHelper();
        return FMOD_Studio_System_SetParameterByIDWithLabel(this.handle, id, encoder.byteFromStringUTF8(label), ignoreseekspeed);
    }
    public readonly RESULT setParametersByIDs(PARAMETER_ID[] ids, float[] values, int count, bool ignoreseekspeed = false) => FMOD_Studio_System_SetParametersByIDs(this.handle, ids, values, count, ignoreseekspeed);
    public readonly RESULT getParameterByName(string name, out float value) => getParameterByName(name, out value, out _);
    public readonly RESULT getParameterByName(string name, out float value, out float finalvalue)
    {
        using StringHelper.ThreadSafeEncoding encoder = StringHelper.GetFreeHelper();
        return FMOD_Studio_System_GetParameterByName(this.handle, encoder.byteFromStringUTF8(name), out value, out finalvalue);
    }
    public readonly RESULT setParameterByName(string name, float value, bool ignoreseekspeed = false)
    {
        using StringHelper.ThreadSafeEncoding encoder = StringHelper.GetFreeHelper();
        return FMOD_Studio_System_SetParameterByName(this.handle, encoder.byteFromStringUTF8(name), value, ignoreseekspeed);
    }
    public readonly RESULT setParameterByNameWithLabel(string name, string label, bool ignoreseekspeed = false)
    {
        using StringHelper.ThreadSafeEncoding encoder = StringHelper.GetFreeHelper(),
                                               labelEncoder = StringHelper.GetFreeHelper();
        return FMOD_Studio_System_SetParameterByNameWithLabel(this.handle, encoder.byteFromStringUTF8(name), labelEncoder.byteFromStringUTF8(label), ignoreseekspeed);
    }
    public readonly RESULT lookupID(string path, out GUID id)
    {
        using StringHelper.ThreadSafeEncoding encoder = StringHelper.GetFreeHelper();
        return FMOD_Studio_System_LookupID(this.handle, encoder.byteFromStringUTF8(path), out id);
    }
    public readonly RESULT lookupPath(GUID id, out string path)
    {
        path = null;

        using StringHelper.ThreadSafeEncoding encoder = StringHelper.GetFreeHelper();
        IntPtr stringMem = Marshal.AllocHGlobal(256);
        RESULT result = FMOD_Studio_System_LookupPath(this.handle, ref id, stringMem, 256, out int retrieved);

        if (result == RESULT.ERR_TRUNCATED)
        {
            Marshal.FreeHGlobal(stringMem);
            stringMem = Marshal.AllocHGlobal(retrieved);
            result = FMOD_Studio_System_LookupPath(this.handle, ref id, stringMem, retrieved, out retrieved);
        }

        if (result == RESULT.OK)
        {
            path = encoder.stringFromNative(stringMem);
        }
        Marshal.FreeHGlobal(stringMem);
        return result;
    }
    public readonly RESULT getNumListeners(out int numlisteners) => FMOD_Studio_System_GetNumListeners(this.handle, out numlisteners);
    public readonly RESULT setNumListeners(int numlisteners) => FMOD_Studio_System_SetNumListeners(this.handle, numlisteners);
    public readonly RESULT getListenerAttributes(int listener, out ATTRIBUTES_3D attributes) => FMOD_Studio_System_GetListenerAttributes(this.handle, listener, out attributes, IntPtr.Zero);
    public readonly RESULT getListenerAttributes(int listener, out ATTRIBUTES_3D attributes, out VECTOR attenuationposition) => FMOD_Studio_System_GetListenerAttributes(this.handle, listener, out attributes, out attenuationposition);
    public readonly RESULT setListenerAttributes(int listener, ATTRIBUTES_3D attributes) => FMOD_Studio_System_SetListenerAttributes(this.handle, listener, ref attributes, IntPtr.Zero);
    public readonly RESULT setListenerAttributes(int listener, ATTRIBUTES_3D attributes, VECTOR attenuationposition) => FMOD_Studio_System_SetListenerAttributes(this.handle, listener, ref attributes, ref attenuationposition);
    public readonly RESULT getListenerWeight(int listener, out float weight) => FMOD_Studio_System_GetListenerWeight(this.handle, listener, out weight);
    public readonly RESULT setListenerWeight(int listener, float weight) => FMOD_Studio_System_SetListenerWeight(this.handle, listener, weight);
    public readonly RESULT loadBankFile(string filename, LOAD_BANK_FLAGS flags, out Bank bank)
    {
        using StringHelper.ThreadSafeEncoding encoder = StringHelper.GetFreeHelper();
        return FMOD_Studio_System_LoadBankFile(this.handle, encoder.byteFromStringUTF8(filename), flags, out bank.handle);
    }
    public readonly RESULT loadBankMemory(byte[] buffer, LOAD_BANK_FLAGS flags, out Bank bank)
    {
        // Manually pin the byte array. It's what the marshaller should do anyway but don't leave it to chance.
        GCHandle pinnedArray = GCHandle.Alloc(buffer, GCHandleType.Pinned);
        IntPtr pointer = pinnedArray.AddrOfPinnedObject();
        RESULT result = FMOD_Studio_System_LoadBankMemory(this.handle, pointer, buffer.Length, LOAD_MEMORY_MODE.LOAD_MEMORY, flags, out bank.handle);
        pinnedArray.Free();
        return result;
    }
    public readonly RESULT loadBankCustom(BANK_INFO info, LOAD_BANK_FLAGS flags, out Bank bank)
    {
        info.size = MarshalHelper.SizeOf(typeof(BANK_INFO));
        return FMOD_Studio_System_LoadBankCustom(this.handle, ref info, flags, out bank.handle);
    }
    public readonly RESULT unloadAll() => FMOD_Studio_System_UnloadAll(this.handle);
    public readonly RESULT flushCommands() => FMOD_Studio_System_FlushCommands(this.handle);
    public readonly RESULT flushSampleLoading() => FMOD_Studio_System_FlushSampleLoading(this.handle);
    public readonly RESULT startCommandCapture(string filename, COMMANDCAPTURE_FLAGS flags)
    {
        using StringHelper.ThreadSafeEncoding encoder = StringHelper.GetFreeHelper();
        return FMOD_Studio_System_StartCommandCapture(this.handle, encoder.byteFromStringUTF8(filename), flags);
    }
    public readonly RESULT stopCommandCapture() => FMOD_Studio_System_StopCommandCapture(this.handle);
    public readonly RESULT loadCommandReplay(string filename, COMMANDREPLAY_FLAGS flags, out CommandReplay replay)
    {
        using StringHelper.ThreadSafeEncoding encoder = StringHelper.GetFreeHelper();
        return FMOD_Studio_System_LoadCommandReplay(this.handle, encoder.byteFromStringUTF8(filename), flags, out replay.handle);
    }
    public readonly RESULT getBankCount(out int count) => FMOD_Studio_System_GetBankCount(this.handle, out count);
    public readonly RESULT getBankList(out Bank[] array)
    {
        array = null;

        RESULT result;
        result = FMOD_Studio_System_GetBankCount(this.handle, out int capacity);
        if (result != RESULT.OK)
        {
            return result;
        }
        if (capacity == 0)
        {
            array = [];
            return result;
        }

        IntPtr[] rawArray = new IntPtr[capacity];
        result = FMOD_Studio_System_GetBankList(this.handle, rawArray, capacity, out int actualCount);
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
    public readonly RESULT getParameterDescriptionCount(out int count) => FMOD_Studio_System_GetParameterDescriptionCount(this.handle, out count);
    public readonly RESULT getParameterDescriptionList(out PARAMETER_DESCRIPTION[] array)
    {
        array = null;

        RESULT result = FMOD_Studio_System_GetParameterDescriptionCount(this.handle, out int capacity);
        if (result != RESULT.OK)
        {
            return result;
        }
        if (capacity == 0)
        {
            array = [];
            return RESULT.OK;
        }

        PARAMETER_DESCRIPTION[] tempArray = new PARAMETER_DESCRIPTION[capacity];
        result = FMOD_Studio_System_GetParameterDescriptionList(this.handle, tempArray, capacity, out int actualCount);
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
    public readonly RESULT getCPUUsage(out CPU_USAGE usage, out FMOD.CPU_USAGE usage_core) => FMOD_Studio_System_GetCPUUsage(this.handle, out usage, out usage_core);
    public readonly RESULT getBufferUsage(out BUFFER_USAGE usage) => FMOD_Studio_System_GetBufferUsage(this.handle, out usage);
    public readonly RESULT resetBufferUsage() => FMOD_Studio_System_ResetBufferUsage(this.handle);

    public readonly RESULT setCallback(SYSTEM_CALLBACK callback, SYSTEM_CALLBACK_TYPE callbackmask = SYSTEM_CALLBACK_TYPE.ALL) => FMOD_Studio_System_SetCallback(this.handle, callback, callbackmask);

    public readonly RESULT getUserData(out IntPtr userdata) => FMOD_Studio_System_GetUserData(this.handle, out userdata);

    public readonly RESULT setUserData(IntPtr userdata) => FMOD_Studio_System_SetUserData(this.handle, userdata);

    public readonly RESULT getMemoryUsage(out MEMORY_USAGE memoryusage) => FMOD_Studio_System_GetMemoryUsage(this.handle, out memoryusage);

    #region importfunctions
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_System_Create                  (out IntPtr system, uint headerversion);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern bool   FMOD_Studio_System_IsValid                 (IntPtr system);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_System_SetAdvancedSettings     (IntPtr system, ref ADVANCEDSETTINGS settings);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_System_GetAdvancedSettings     (IntPtr system, out ADVANCEDSETTINGS settings);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_System_Initialize              (IntPtr system, int maxchannels, INITFLAGS studioflags, FMOD.INITFLAGS flags, IntPtr extradriverdata);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_System_Release                 (IntPtr system);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_System_Update                  (IntPtr system);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_System_GetCoreSystem           (IntPtr system, out IntPtr coresystem);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_System_GetEvent                (IntPtr system, byte[] path, out IntPtr _event);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_System_GetBus                  (IntPtr system, byte[] path, out IntPtr bus);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_System_GetVCA                  (IntPtr system, byte[] path, out IntPtr vca);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_System_GetBank                 (IntPtr system, byte[] path, out IntPtr bank);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_System_GetEventByID            (IntPtr system, ref GUID id, out IntPtr _event);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_System_GetBusByID              (IntPtr system, ref GUID id, out IntPtr bus);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_System_GetVCAByID              (IntPtr system, ref GUID id, out IntPtr vca);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_System_GetBankByID             (IntPtr system, ref GUID id, out IntPtr bank);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_System_GetSoundInfo            (IntPtr system, byte[] key, out SOUND_INFO info);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_System_GetParameterDescriptionByName(IntPtr system, byte[] name, out PARAMETER_DESCRIPTION parameter);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_System_GetParameterDescriptionByID(IntPtr system, PARAMETER_ID id, out PARAMETER_DESCRIPTION parameter);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_System_GetParameterLabelByName (IntPtr system, byte[] name, int labelindex, IntPtr label, int size, out int retrieved);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_System_GetParameterLabelByID   (IntPtr system, PARAMETER_ID id, int labelindex, IntPtr label, int size, out int retrieved);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_System_GetParameterByID        (IntPtr system, PARAMETER_ID id, out float value, out float finalvalue);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_System_SetParameterByID        (IntPtr system, PARAMETER_ID id, float value, bool ignoreseekspeed);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_System_SetParameterByIDWithLabel   (IntPtr system, PARAMETER_ID id, byte[] label, bool ignoreseekspeed);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_System_SetParametersByIDs      (IntPtr system, PARAMETER_ID[] ids, float[] values, int count, bool ignoreseekspeed);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_System_GetParameterByName      (IntPtr system, byte[] name, out float value, out float finalvalue);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_System_SetParameterByName      (IntPtr system, byte[] name, float value, bool ignoreseekspeed);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_System_SetParameterByNameWithLabel (IntPtr system, byte[] name, byte[] label, bool ignoreseekspeed);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_System_LookupID                (IntPtr system, byte[] path, out GUID id);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_System_LookupPath              (IntPtr system, ref GUID id, IntPtr path, int size, out int retrieved);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_System_GetNumListeners         (IntPtr system, out int numlisteners);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_System_SetNumListeners         (IntPtr system, int numlisteners);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_System_GetListenerAttributes   (IntPtr system, int listener, out ATTRIBUTES_3D attributes, IntPtr zero);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_System_GetListenerAttributes   (IntPtr system, int listener, out ATTRIBUTES_3D attributes, out VECTOR attenuationposition);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_System_SetListenerAttributes   (IntPtr system, int listener, ref ATTRIBUTES_3D attributes, IntPtr zero);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_System_SetListenerAttributes   (IntPtr system, int listener, ref ATTRIBUTES_3D attributes, ref VECTOR attenuationposition);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_System_GetListenerWeight       (IntPtr system, int listener, out float weight);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_System_SetListenerWeight       (IntPtr system, int listener, float weight);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_System_LoadBankFile            (IntPtr system, byte[] filename, LOAD_BANK_FLAGS flags, out IntPtr bank);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_System_LoadBankMemory          (IntPtr system, IntPtr buffer, int length, LOAD_MEMORY_MODE mode, LOAD_BANK_FLAGS flags, out IntPtr bank);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_System_LoadBankCustom          (IntPtr system, ref BANK_INFO info, LOAD_BANK_FLAGS flags, out IntPtr bank);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_System_UnloadAll               (IntPtr system);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_System_FlushCommands           (IntPtr system);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_System_FlushSampleLoading      (IntPtr system);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_System_StartCommandCapture     (IntPtr system, byte[] filename, COMMANDCAPTURE_FLAGS flags);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_System_StopCommandCapture      (IntPtr system);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_System_LoadCommandReplay       (IntPtr system, byte[] filename, COMMANDREPLAY_FLAGS flags, out IntPtr replay);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_System_GetBankCount            (IntPtr system, out int count);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_System_GetBankList             (IntPtr system, IntPtr[] array, int capacity, out int count);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_System_GetParameterDescriptionCount(IntPtr system, out int count);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_System_GetParameterDescriptionList(IntPtr system, [Out] PARAMETER_DESCRIPTION[] array, int capacity, out int count);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_System_GetCPUUsage             (IntPtr system, out CPU_USAGE usage, out FMOD.CPU_USAGE usage_core);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_System_GetBufferUsage          (IntPtr system, out BUFFER_USAGE usage);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_System_ResetBufferUsage        (IntPtr system);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_System_SetCallback             (IntPtr system, SYSTEM_CALLBACK callback, SYSTEM_CALLBACK_TYPE callbackmask);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_System_GetUserData             (IntPtr system, out IntPtr userdata);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_System_SetUserData             (IntPtr system, IntPtr userdata);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_System_GetMemoryUsage          (IntPtr system, out MEMORY_USAGE memoryusage);
    #endregion

    #region wrapperinternal

    public IntPtr handle = ptr;

    public readonly bool hasHandle() => this.handle != IntPtr.Zero;
    public void clearHandle() => this.handle = IntPtr.Zero;

    public readonly bool isValid() => hasHandle() && FMOD_Studio_System_IsValid(this.handle);

    #endregion
}

public struct EventDescription(IntPtr ptr)
{
    public readonly RESULT getID(out GUID id) => FMOD_Studio_EventDescription_GetID(this.handle, out id);
    public readonly RESULT getPath(out string path)
    {
        path = null;

        using StringHelper.ThreadSafeEncoding encoder = StringHelper.GetFreeHelper();
        IntPtr stringMem = Marshal.AllocHGlobal(256);
        RESULT result = FMOD_Studio_EventDescription_GetPath(this.handle, stringMem, 256, out int retrieved);

        if (result == RESULT.ERR_TRUNCATED)
        {
            Marshal.FreeHGlobal(stringMem);
            stringMem = Marshal.AllocHGlobal(retrieved);
            result = FMOD_Studio_EventDescription_GetPath(this.handle, stringMem, retrieved, out retrieved);
        }

        if (result == RESULT.OK)
        {
            path = encoder.stringFromNative(stringMem);
        }
        Marshal.FreeHGlobal(stringMem);
        return result;
    }
    public readonly RESULT getParameterDescriptionCount(out int count) => FMOD_Studio_EventDescription_GetParameterDescriptionCount(this.handle, out count);
    public readonly RESULT getParameterDescriptionByIndex(int index, out PARAMETER_DESCRIPTION parameter) => FMOD_Studio_EventDescription_GetParameterDescriptionByIndex(this.handle, index, out parameter);
    public readonly RESULT getParameterDescriptionByName(string name, out PARAMETER_DESCRIPTION parameter)
    {
        using StringHelper.ThreadSafeEncoding encoder = StringHelper.GetFreeHelper();
        return FMOD_Studio_EventDescription_GetParameterDescriptionByName(this.handle, encoder.byteFromStringUTF8(name), out parameter);
    }
    public readonly RESULT getParameterDescriptionByID(PARAMETER_ID id, out PARAMETER_DESCRIPTION parameter) => FMOD_Studio_EventDescription_GetParameterDescriptionByID(this.handle, id, out parameter);
    public readonly RESULT getParameterLabelByIndex(int index, int labelindex, out string label)
    {
        label = null;

        using StringHelper.ThreadSafeEncoding encoder = StringHelper.GetFreeHelper();
        IntPtr stringMem = Marshal.AllocHGlobal(256);
        RESULT result = FMOD_Studio_EventDescription_GetParameterLabelByIndex(this.handle, index, labelindex, stringMem, 256, out int retrieved);

        if (result == RESULT.ERR_TRUNCATED)
        {
            Marshal.FreeHGlobal(stringMem);
            result = FMOD_Studio_EventDescription_GetParameterLabelByIndex(this.handle, index, labelindex, IntPtr.Zero, 0, out retrieved);
            stringMem = Marshal.AllocHGlobal(retrieved);
            result = FMOD_Studio_EventDescription_GetParameterLabelByIndex(this.handle, index, labelindex, stringMem, retrieved, out retrieved);
        }

        if (result == RESULT.OK)
        {
            label = encoder.stringFromNative(stringMem);
        }
        Marshal.FreeHGlobal(stringMem);
        return result;
    }
    public readonly RESULT getParameterLabelByName(string name, int labelindex, out string label)
    {
        label = null;

        using StringHelper.ThreadSafeEncoding encoder = StringHelper.GetFreeHelper();
        IntPtr stringMem = Marshal.AllocHGlobal(256);
        byte[] nameBytes = encoder.byteFromStringUTF8(name);
        RESULT result = FMOD_Studio_EventDescription_GetParameterLabelByName(this.handle, nameBytes, labelindex, stringMem, 256, out int retrieved);

        if (result == RESULT.ERR_TRUNCATED)
        {
            Marshal.FreeHGlobal(stringMem);
            result = FMOD_Studio_EventDescription_GetParameterLabelByName(this.handle, nameBytes, labelindex, IntPtr.Zero, 0, out retrieved);
            stringMem = Marshal.AllocHGlobal(retrieved);
            result = FMOD_Studio_EventDescription_GetParameterLabelByName(this.handle, nameBytes, labelindex, stringMem, retrieved, out retrieved);
        }

        if (result == RESULT.OK)
        {
            label = encoder.stringFromNative(stringMem);
        }
        Marshal.FreeHGlobal(stringMem);
        return result;
    }
    public readonly RESULT getParameterLabelByID(PARAMETER_ID id, int labelindex, out string label)
    {
        label = null;

        using StringHelper.ThreadSafeEncoding encoder = StringHelper.GetFreeHelper();
        IntPtr stringMem = Marshal.AllocHGlobal(256);
        RESULT result = FMOD_Studio_EventDescription_GetParameterLabelByID(this.handle, id, labelindex, stringMem, 256, out int retrieved);

        if (result == RESULT.ERR_TRUNCATED)
        {
            Marshal.FreeHGlobal(stringMem);
            result = FMOD_Studio_EventDescription_GetParameterLabelByID(this.handle, id, labelindex, IntPtr.Zero, 0, out retrieved);
            stringMem = Marshal.AllocHGlobal(retrieved);
            result = FMOD_Studio_EventDescription_GetParameterLabelByID(this.handle, id, labelindex, stringMem, retrieved, out retrieved);
        }

        if (result == RESULT.OK)
        {
            label = encoder.stringFromNative(stringMem);
        }
        Marshal.FreeHGlobal(stringMem);
        return result;
    }
    public readonly RESULT getUserPropertyCount(out int count) => FMOD_Studio_EventDescription_GetUserPropertyCount(this.handle, out count);
    public readonly RESULT getUserPropertyByIndex(int index, out USER_PROPERTY property) => FMOD_Studio_EventDescription_GetUserPropertyByIndex(this.handle, index, out property);
    public readonly RESULT getUserProperty(string name, out USER_PROPERTY property)
    {
        using StringHelper.ThreadSafeEncoding encoder = StringHelper.GetFreeHelper();
        return FMOD_Studio_EventDescription_GetUserProperty(this.handle, encoder.byteFromStringUTF8(name), out property);
    }
    public readonly RESULT getLength(out int length) => FMOD_Studio_EventDescription_GetLength(this.handle, out length);
    public readonly RESULT getMinMaxDistance(out float min, out float max) => FMOD_Studio_EventDescription_GetMinMaxDistance(this.handle, out min, out max);
    public readonly RESULT getSoundSize(out float size) => FMOD_Studio_EventDescription_GetSoundSize(this.handle, out size);
    public readonly RESULT isSnapshot(out bool snapshot) => FMOD_Studio_EventDescription_IsSnapshot(this.handle, out snapshot);
    public readonly RESULT isOneshot(out bool oneshot) => FMOD_Studio_EventDescription_IsOneshot(this.handle, out oneshot);
    public readonly RESULT isStream(out bool isStream) => FMOD_Studio_EventDescription_IsStream(this.handle, out isStream);
    public readonly RESULT is3D(out bool is3D) => FMOD_Studio_EventDescription_Is3D(this.handle, out is3D);
    public readonly RESULT isDopplerEnabled(out bool doppler) => FMOD_Studio_EventDescription_IsDopplerEnabled(this.handle, out doppler);
    public readonly RESULT hasSustainPoint(out bool sustainPoint) => FMOD_Studio_EventDescription_HasSustainPoint(this.handle, out sustainPoint);

    public readonly RESULT createInstance(out EventInstance instance) => FMOD_Studio_EventDescription_CreateInstance(this.handle, out instance.handle);

    public readonly RESULT getInstanceCount(out int count) => FMOD_Studio_EventDescription_GetInstanceCount(this.handle, out count);
    public readonly RESULT getInstanceList(out EventInstance[] array)
    {
        array = null;

        RESULT result;
        result = FMOD_Studio_EventDescription_GetInstanceCount(this.handle, out int capacity);
        if (result != RESULT.OK)
        {
            return result;
        }
        if (capacity == 0)
        {
            array = [];
            return result;
        }

        IntPtr[] rawArray = new IntPtr[capacity];
        result = FMOD_Studio_EventDescription_GetInstanceList(this.handle, rawArray, capacity, out int actualCount);
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
            array[i].handle = rawArray[i];
        }
        return RESULT.OK;
    }

    public readonly RESULT loadSampleData() => FMOD_Studio_EventDescription_LoadSampleData(this.handle);

    public readonly RESULT unloadSampleData() => FMOD_Studio_EventDescription_UnloadSampleData(this.handle);

    public readonly RESULT getSampleLoadingState(out LOADING_STATE state) => FMOD_Studio_EventDescription_GetSampleLoadingState(this.handle, out state);

    public readonly RESULT releaseAllInstances() => FMOD_Studio_EventDescription_ReleaseAllInstances(this.handle);
    public readonly RESULT setCallback(EVENT_CALLBACK callback, EVENT_CALLBACK_TYPE callbackmask = EVENT_CALLBACK_TYPE.ALL) => FMOD_Studio_EventDescription_SetCallback(this.handle, callback, callbackmask);

    public readonly RESULT getUserData(out IntPtr userdata) => FMOD_Studio_EventDescription_GetUserData(this.handle, out userdata);

    public readonly RESULT setUserData(IntPtr userdata) => FMOD_Studio_EventDescription_SetUserData(this.handle, userdata);

    #region importfunctions
    [DllImport(STUDIO_VERSION.dll)]
    private static extern bool FMOD_Studio_EventDescription_IsValid                 (IntPtr eventdescription);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_EventDescription_GetID                 (IntPtr eventdescription, out GUID id);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_EventDescription_GetPath               (IntPtr eventdescription, IntPtr path, int size, out int retrieved);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_EventDescription_GetParameterDescriptionCount(IntPtr eventdescription, out int count);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_EventDescription_GetParameterDescriptionByIndex(IntPtr eventdescription, int index, out PARAMETER_DESCRIPTION parameter);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_EventDescription_GetParameterDescriptionByName(IntPtr eventdescription, byte[] name, out PARAMETER_DESCRIPTION parameter);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_EventDescription_GetParameterDescriptionByID(IntPtr eventdescription, PARAMETER_ID id, out PARAMETER_DESCRIPTION parameter);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_EventDescription_GetParameterLabelByIndex(IntPtr eventdescription, int index, int labelindex, IntPtr label, int size, out int retrieved);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_EventDescription_GetParameterLabelByName(IntPtr eventdescription, byte[] name, int labelindex, IntPtr label, int size, out int retrieved);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_EventDescription_GetParameterLabelByID (IntPtr eventdescription, PARAMETER_ID id, int labelindex, IntPtr label, int size, out int retrieved);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_EventDescription_GetUserPropertyCount  (IntPtr eventdescription, out int count);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_EventDescription_GetUserPropertyByIndex(IntPtr eventdescription, int index, out USER_PROPERTY property);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_EventDescription_GetUserProperty       (IntPtr eventdescription, byte[] name, out USER_PROPERTY property);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_EventDescription_GetLength             (IntPtr eventdescription, out int length);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_EventDescription_GetMinMaxDistance     (IntPtr eventdescription, out float min, out float max);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_EventDescription_GetSoundSize          (IntPtr eventdescription, out float size);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_EventDescription_IsSnapshot            (IntPtr eventdescription, out bool snapshot);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_EventDescription_IsOneshot             (IntPtr eventdescription, out bool oneshot);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_EventDescription_IsStream              (IntPtr eventdescription, out bool isStream);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_EventDescription_Is3D                  (IntPtr eventdescription, out bool is3D);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_EventDescription_IsDopplerEnabled      (IntPtr eventdescription, out bool doppler);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_EventDescription_HasSustainPoint       (IntPtr eventdescription, out bool sustainPoint);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_EventDescription_CreateInstance        (IntPtr eventdescription, out IntPtr instance);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_EventDescription_GetInstanceCount      (IntPtr eventdescription, out int count);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_EventDescription_GetInstanceList       (IntPtr eventdescription, IntPtr[] array, int capacity, out int count);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_EventDescription_LoadSampleData        (IntPtr eventdescription);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_EventDescription_UnloadSampleData      (IntPtr eventdescription);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_EventDescription_GetSampleLoadingState (IntPtr eventdescription, out LOADING_STATE state);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_EventDescription_ReleaseAllInstances   (IntPtr eventdescription);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_EventDescription_SetCallback           (IntPtr eventdescription, EVENT_CALLBACK callback, EVENT_CALLBACK_TYPE callbackmask);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_EventDescription_GetUserData           (IntPtr eventdescription, out IntPtr userdata);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_EventDescription_SetUserData           (IntPtr eventdescription, IntPtr userdata);
    #endregion
    #region wrapperinternal

    public IntPtr handle = ptr;

    public readonly bool hasHandle() => this.handle != IntPtr.Zero;
    public void clearHandle() => this.handle = IntPtr.Zero;

    public readonly bool isValid() => hasHandle() && FMOD_Studio_EventDescription_IsValid(this.handle);

    #endregion
}

public struct EventInstance(IntPtr ptr)
{
    public readonly RESULT getDescription(out EventDescription description) => FMOD_Studio_EventInstance_GetDescription(this.handle, out description.handle);
    public readonly RESULT getVolume(out float volume) => FMOD_Studio_EventInstance_GetVolume(this.handle, out volume, IntPtr.Zero);
    public readonly RESULT getVolume(out float volume, out float finalvolume) => FMOD_Studio_EventInstance_GetVolume(this.handle, out volume, out finalvolume);
    public readonly RESULT setVolume(float volume) => FMOD_Studio_EventInstance_SetVolume(this.handle, volume);
    public readonly RESULT getPitch(out float pitch) => FMOD_Studio_EventInstance_GetPitch(this.handle, out pitch, IntPtr.Zero);
    public readonly RESULT getPitch(out float pitch, out float finalpitch) => FMOD_Studio_EventInstance_GetPitch(this.handle, out pitch, out finalpitch);
    public readonly RESULT setPitch(float pitch) => FMOD_Studio_EventInstance_SetPitch(this.handle, pitch);
    public readonly RESULT get3DAttributes(out ATTRIBUTES_3D attributes) => FMOD_Studio_EventInstance_Get3DAttributes(this.handle, out attributes);
    public readonly RESULT set3DAttributes(ATTRIBUTES_3D attributes) => FMOD_Studio_EventInstance_Set3DAttributes(this.handle, ref attributes);
    public readonly RESULT getListenerMask(out uint mask) => FMOD_Studio_EventInstance_GetListenerMask(this.handle, out mask);
    public readonly RESULT setListenerMask(uint mask) => FMOD_Studio_EventInstance_SetListenerMask(this.handle, mask);
    public readonly RESULT getProperty(EVENT_PROPERTY index, out float value) => FMOD_Studio_EventInstance_GetProperty(this.handle, index, out value);
    public readonly RESULT setProperty(EVENT_PROPERTY index, float value) => FMOD_Studio_EventInstance_SetProperty(this.handle, index, value);
    public readonly RESULT getReverbLevel(int index, out float level) => FMOD_Studio_EventInstance_GetReverbLevel(this.handle, index, out level);
    public readonly RESULT setReverbLevel(int index, float level) => FMOD_Studio_EventInstance_SetReverbLevel(this.handle, index, level);
    public readonly RESULT getPaused(out bool paused) => FMOD_Studio_EventInstance_GetPaused(this.handle, out paused);
    public readonly RESULT setPaused(bool paused) => FMOD_Studio_EventInstance_SetPaused(this.handle, paused);
    public readonly RESULT start() => FMOD_Studio_EventInstance_Start(this.handle);
    public readonly RESULT stop(STOP_MODE mode) => FMOD_Studio_EventInstance_Stop(this.handle, mode);
    public readonly RESULT getTimelinePosition(out int position) => FMOD_Studio_EventInstance_GetTimelinePosition(this.handle, out position);
    public readonly RESULT setTimelinePosition(int position) => FMOD_Studio_EventInstance_SetTimelinePosition(this.handle, position);
    public readonly RESULT getPlaybackState(out PLAYBACK_STATE state) => FMOD_Studio_EventInstance_GetPlaybackState(this.handle, out state);
    public readonly RESULT getChannelGroup(out FMOD.ChannelGroup group) => FMOD_Studio_EventInstance_GetChannelGroup(this.handle, out group.handle);
    public readonly RESULT getMinMaxDistance(out float min, out float max) => FMOD_Studio_EventInstance_GetMinMaxDistance(this.handle, out min, out max);
    public readonly RESULT release() => FMOD_Studio_EventInstance_Release(this.handle);
    public readonly RESULT isVirtual(out bool virtualstate) => FMOD_Studio_EventInstance_IsVirtual(this.handle, out virtualstate);
    public readonly RESULT getParameterByID(PARAMETER_ID id, out float value) => getParameterByID(id, out value, out _);
    public readonly RESULT getParameterByID(PARAMETER_ID id, out float value, out float finalvalue) => FMOD_Studio_EventInstance_GetParameterByID(this.handle, id, out value, out finalvalue);
    public readonly RESULT setParameterByID(PARAMETER_ID id, float value, bool ignoreseekspeed = false) => FMOD_Studio_EventInstance_SetParameterByID(this.handle, id, value, ignoreseekspeed);
    public readonly RESULT setParameterByIDWithLabel(PARAMETER_ID id, string label, bool ignoreseekspeed = false)
    {
        using StringHelper.ThreadSafeEncoding encoder = StringHelper.GetFreeHelper();
        return FMOD_Studio_EventInstance_SetParameterByIDWithLabel(this.handle, id, encoder.byteFromStringUTF8(label), ignoreseekspeed);
    }
    public readonly RESULT setParametersByIDs(PARAMETER_ID[] ids, float[] values, int count, bool ignoreseekspeed = false) => FMOD_Studio_EventInstance_SetParametersByIDs(this.handle, ids, values, count, ignoreseekspeed);
    public readonly RESULT getParameterByName(string name, out float value) => getParameterByName(name, out value, out _);
    public readonly RESULT getParameterByName(string name, out float value, out float finalvalue)
    {
        using StringHelper.ThreadSafeEncoding encoder = StringHelper.GetFreeHelper();
        return FMOD_Studio_EventInstance_GetParameterByName(this.handle, encoder.byteFromStringUTF8(name), out value, out finalvalue);
    }
    public readonly RESULT setParameterByName(string name, float value, bool ignoreseekspeed = false)
    {
        using StringHelper.ThreadSafeEncoding encoder = StringHelper.GetFreeHelper();
        return FMOD_Studio_EventInstance_SetParameterByName(this.handle, encoder.byteFromStringUTF8(name), value, ignoreseekspeed);
    }
    public readonly RESULT setParameterByNameWithLabel(string name, string label, bool ignoreseekspeed = false)
    {
        using StringHelper.ThreadSafeEncoding encoder = StringHelper.GetFreeHelper(),
                                               labelEncoder = StringHelper.GetFreeHelper();
        return FMOD_Studio_EventInstance_SetParameterByNameWithLabel(this.handle, encoder.byteFromStringUTF8(name), labelEncoder.byteFromStringUTF8(label), ignoreseekspeed);
    }
    public readonly RESULT keyOff() => FMOD_Studio_EventInstance_KeyOff(this.handle);
    public readonly RESULT setCallback(EVENT_CALLBACK callback, EVENT_CALLBACK_TYPE callbackmask = EVENT_CALLBACK_TYPE.ALL) => FMOD_Studio_EventInstance_SetCallback(this.handle, callback, callbackmask);
    public readonly RESULT getUserData(out IntPtr userdata) => FMOD_Studio_EventInstance_GetUserData(this.handle, out userdata);
    public readonly RESULT setUserData(IntPtr userdata) => FMOD_Studio_EventInstance_SetUserData(this.handle, userdata);
    public readonly RESULT getCPUUsage(out uint exclusive, out uint inclusive) => FMOD_Studio_EventInstance_GetCPUUsage(this.handle, out exclusive, out inclusive);
    public readonly RESULT getMemoryUsage(out MEMORY_USAGE memoryusage) => FMOD_Studio_EventInstance_GetMemoryUsage(this.handle, out memoryusage);
    #region importfunctions
    [DllImport(STUDIO_VERSION.dll)]
    private static extern bool   FMOD_Studio_EventInstance_IsValid                     (IntPtr _event);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_EventInstance_GetDescription              (IntPtr _event, out IntPtr description);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_EventInstance_GetVolume                   (IntPtr _event, out float volume, IntPtr zero);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_EventInstance_GetVolume                   (IntPtr _event, out float volume, out float finalvolume);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_EventInstance_SetVolume                   (IntPtr _event, float volume);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_EventInstance_GetPitch                    (IntPtr _event, out float pitch, IntPtr zero);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_EventInstance_GetPitch                    (IntPtr _event, out float pitch, out float finalpitch);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_EventInstance_SetPitch                    (IntPtr _event, float pitch);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_EventInstance_Get3DAttributes             (IntPtr _event, out ATTRIBUTES_3D attributes);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_EventInstance_Set3DAttributes             (IntPtr _event, ref ATTRIBUTES_3D attributes);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_EventInstance_GetListenerMask             (IntPtr _event, out uint mask);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_EventInstance_SetListenerMask             (IntPtr _event, uint mask);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_EventInstance_GetProperty                 (IntPtr _event, EVENT_PROPERTY index, out float value);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_EventInstance_SetProperty                 (IntPtr _event, EVENT_PROPERTY index, float value);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_EventInstance_GetReverbLevel              (IntPtr _event, int index, out float level);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_EventInstance_SetReverbLevel              (IntPtr _event, int index, float level);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_EventInstance_GetPaused                   (IntPtr _event, out bool paused);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_EventInstance_SetPaused                   (IntPtr _event, bool paused);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_EventInstance_Start                       (IntPtr _event);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_EventInstance_Stop                        (IntPtr _event, STOP_MODE mode);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_EventInstance_GetTimelinePosition         (IntPtr _event, out int position);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_EventInstance_SetTimelinePosition         (IntPtr _event, int position);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_EventInstance_GetPlaybackState            (IntPtr _event, out PLAYBACK_STATE state);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_EventInstance_GetChannelGroup             (IntPtr _event, out IntPtr group);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_EventInstance_GetMinMaxDistance           (IntPtr _event, out float min, out float max);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_EventInstance_Release                     (IntPtr _event);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_EventInstance_IsVirtual                   (IntPtr _event, out bool virtualstate);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_EventInstance_GetParameterByName          (IntPtr _event, byte[] name, out float value, out float finalvalue);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_EventInstance_SetParameterByName          (IntPtr _event, byte[] name, float value, bool ignoreseekspeed);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_EventInstance_SetParameterByNameWithLabel (IntPtr _event, byte[] name, byte[] label, bool ignoreseekspeed);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_EventInstance_GetParameterByID            (IntPtr _event, PARAMETER_ID id, out float value, out float finalvalue);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_EventInstance_SetParameterByID            (IntPtr _event, PARAMETER_ID id, float value, bool ignoreseekspeed);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_EventInstance_SetParameterByIDWithLabel   (IntPtr _event, PARAMETER_ID id, byte[] label, bool ignoreseekspeed);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_EventInstance_SetParametersByIDs          (IntPtr _event, PARAMETER_ID[] ids, float[] values, int count, bool ignoreseekspeed);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_EventInstance_KeyOff                      (IntPtr _event);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_EventInstance_SetCallback                 (IntPtr _event, EVENT_CALLBACK callback, EVENT_CALLBACK_TYPE callbackmask);
    [DllImport (STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_EventInstance_GetUserData                 (IntPtr _event, out IntPtr userdata);
    [DllImport (STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_EventInstance_SetUserData                 (IntPtr _event, IntPtr userdata);
    [DllImport (STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_EventInstance_GetCPUUsage                 (IntPtr _event, out uint exclusive, out uint inclusive);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_EventInstance_GetMemoryUsage              (IntPtr _event, out MEMORY_USAGE memoryusage);
    #endregion

    #region wrapperinternal

    public IntPtr handle = ptr;

    public readonly bool hasHandle() => this.handle != IntPtr.Zero;
    public void clearHandle() => this.handle = IntPtr.Zero;

    public readonly bool isValid() => hasHandle() && FMOD_Studio_EventInstance_IsValid(this.handle);

    #endregion
}

public struct Bus(IntPtr ptr)
{
    public readonly RESULT getID(out GUID id) => FMOD_Studio_Bus_GetID(this.handle, out id);
    public readonly RESULT getPath(out string path)
    {
        path = null;

        using StringHelper.ThreadSafeEncoding encoder = StringHelper.GetFreeHelper();
        IntPtr stringMem = Marshal.AllocHGlobal(256);
        RESULT result = FMOD_Studio_Bus_GetPath(this.handle, stringMem, 256, out int retrieved);

        if (result == RESULT.ERR_TRUNCATED)
        {
            Marshal.FreeHGlobal(stringMem);
            stringMem = Marshal.AllocHGlobal(retrieved);
            result = FMOD_Studio_Bus_GetPath(this.handle, stringMem, retrieved, out retrieved);
        }

        if (result == RESULT.OK)
        {
            path = encoder.stringFromNative(stringMem);
        }
        Marshal.FreeHGlobal(stringMem);
        return result;

    }
    public readonly RESULT getVolume(out float volume) => getVolume(out volume, out _);
    public readonly RESULT getVolume(out float volume, out float finalvolume) => FMOD_Studio_Bus_GetVolume(this.handle, out volume, out finalvolume);
    public readonly RESULT setVolume(float volume) => FMOD_Studio_Bus_SetVolume(this.handle, volume);
    public readonly RESULT getPaused(out bool paused) => FMOD_Studio_Bus_GetPaused(this.handle, out paused);
    public readonly RESULT setPaused(bool paused) => FMOD_Studio_Bus_SetPaused(this.handle, paused);
    public readonly RESULT getMute(out bool mute) => FMOD_Studio_Bus_GetMute(this.handle, out mute);
    public readonly RESULT setMute(bool mute) => FMOD_Studio_Bus_SetMute(this.handle, mute);
    public readonly RESULT stopAllEvents(STOP_MODE mode) => FMOD_Studio_Bus_StopAllEvents(this.handle, mode);
    public readonly RESULT lockChannelGroup() => FMOD_Studio_Bus_LockChannelGroup(this.handle);
    public readonly RESULT unlockChannelGroup() => FMOD_Studio_Bus_UnlockChannelGroup(this.handle);
    public readonly RESULT getChannelGroup(out FMOD.ChannelGroup group) => FMOD_Studio_Bus_GetChannelGroup(this.handle, out group.handle);
    public readonly RESULT getCPUUsage(out uint exclusive, out uint inclusive) => FMOD_Studio_Bus_GetCPUUsage(this.handle, out exclusive, out inclusive);
    public readonly RESULT getMemoryUsage(out MEMORY_USAGE memoryusage) => FMOD_Studio_Bus_GetMemoryUsage(this.handle, out memoryusage);
    public readonly RESULT getPortIndex(out ulong index) => FMOD_Studio_Bus_GetPortIndex(this.handle, out index);
    public readonly RESULT setPortIndex(ulong index) => FMOD_Studio_Bus_SetPortIndex(this.handle, index);

    #region importfunctions
    [DllImport(STUDIO_VERSION.dll)]
    private static extern bool   FMOD_Studio_Bus_IsValid              (IntPtr bus);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_Bus_GetID                (IntPtr bus, out GUID id);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_Bus_GetPath              (IntPtr bus, IntPtr path, int size, out int retrieved);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_Bus_GetVolume            (IntPtr bus, out float volume, out float finalvolume);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_Bus_SetVolume            (IntPtr bus, float volume);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_Bus_GetPaused            (IntPtr bus, out bool paused);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_Bus_SetPaused            (IntPtr bus, bool paused);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_Bus_GetMute              (IntPtr bus, out bool mute);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_Bus_SetMute              (IntPtr bus, bool mute);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_Bus_StopAllEvents        (IntPtr bus, STOP_MODE mode);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_Bus_LockChannelGroup     (IntPtr bus);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_Bus_UnlockChannelGroup   (IntPtr bus);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_Bus_GetChannelGroup      (IntPtr bus, out IntPtr group);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_Bus_GetCPUUsage          (IntPtr bus, out uint exclusive, out uint inclusive);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_Bus_GetMemoryUsage       (IntPtr bus, out MEMORY_USAGE memoryusage);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_Bus_GetPortIndex         (IntPtr bus, out ulong index);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_Bus_SetPortIndex         (IntPtr bus, ulong index);
    #endregion

    #region wrapperinternal

    public IntPtr handle = ptr;

    public readonly bool hasHandle() => this.handle != IntPtr.Zero;
    public void clearHandle() => this.handle = IntPtr.Zero;

    public readonly bool isValid() => hasHandle() && FMOD_Studio_Bus_IsValid(this.handle);

    #endregion
}

public struct VCA(IntPtr ptr)
{
    public readonly RESULT getID(out GUID id) => FMOD_Studio_VCA_GetID(this.handle, out id);
    public readonly RESULT getPath(out string path)
    {
        path = null;

        using StringHelper.ThreadSafeEncoding encoder = StringHelper.GetFreeHelper();
        IntPtr stringMem = Marshal.AllocHGlobal(256);
        RESULT result = FMOD_Studio_VCA_GetPath(this.handle, stringMem, 256, out int retrieved);

        if (result == RESULT.ERR_TRUNCATED)
        {
            Marshal.FreeHGlobal(stringMem);
            stringMem = Marshal.AllocHGlobal(retrieved);
            result = FMOD_Studio_VCA_GetPath(this.handle, stringMem, retrieved, out retrieved);
        }

        if (result == RESULT.OK)
        {
            path = encoder.stringFromNative(stringMem);
        }
        Marshal.FreeHGlobal(stringMem);
        return result;
    }
    public readonly RESULT getVolume(out float volume) => getVolume(out volume, out _);
    public readonly RESULT getVolume(out float volume, out float finalvolume) => FMOD_Studio_VCA_GetVolume(this.handle, out volume, out finalvolume);
    public readonly RESULT setVolume(float volume) => FMOD_Studio_VCA_SetVolume(this.handle, volume);

    #region importfunctions
    [DllImport(STUDIO_VERSION.dll)]
    private static extern bool   FMOD_Studio_VCA_IsValid       (IntPtr vca);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_VCA_GetID         (IntPtr vca, out GUID id);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_VCA_GetPath       (IntPtr vca, IntPtr path, int size, out int retrieved);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_VCA_GetVolume     (IntPtr vca, out float volume, out float finalvolume);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_VCA_SetVolume     (IntPtr vca, float volume);
    #endregion

    #region wrapperinternal

    public IntPtr handle = ptr;

    public readonly bool hasHandle() => this.handle != IntPtr.Zero;
    public void clearHandle() => this.handle = IntPtr.Zero;

    public readonly bool isValid() => hasHandle() && FMOD_Studio_VCA_IsValid(this.handle);

    #endregion
}

public struct Bank(IntPtr ptr)
{
    // Property access

    public readonly RESULT getID(out GUID id) => FMOD_Studio_Bank_GetID(this.handle, out id);
    public readonly RESULT getPath(out string path)
    {
        path = null;

        using StringHelper.ThreadSafeEncoding encoder = StringHelper.GetFreeHelper();
        IntPtr stringMem = Marshal.AllocHGlobal(256);
        RESULT result = FMOD_Studio_Bank_GetPath(this.handle, stringMem, 256, out int retrieved);

        if (result == RESULT.ERR_TRUNCATED)
        {
            Marshal.FreeHGlobal(stringMem);
            stringMem = Marshal.AllocHGlobal(retrieved);
            result = FMOD_Studio_Bank_GetPath(this.handle, stringMem, retrieved, out retrieved);
        }

        if (result == RESULT.OK)
        {
            path = encoder.stringFromNative(stringMem);
        }
        Marshal.FreeHGlobal(stringMem);
        return result;
    }
    public readonly RESULT unload() => FMOD_Studio_Bank_Unload(this.handle);
    public readonly RESULT loadSampleData() => FMOD_Studio_Bank_LoadSampleData(this.handle);
    public readonly RESULT unloadSampleData() => FMOD_Studio_Bank_UnloadSampleData(this.handle);
    public readonly RESULT getLoadingState(out LOADING_STATE state) => FMOD_Studio_Bank_GetLoadingState(this.handle, out state);
    public readonly RESULT getSampleLoadingState(out LOADING_STATE state) => FMOD_Studio_Bank_GetSampleLoadingState(this.handle, out state);

    // Enumeration
    public readonly RESULT getStringCount(out int count) => FMOD_Studio_Bank_GetStringCount(this.handle, out count);
    public readonly RESULT getStringInfo(int index, out GUID id, out string path)
    {
        path = null;
        id = new GUID();

        using StringHelper.ThreadSafeEncoding encoder = StringHelper.GetFreeHelper();
        IntPtr stringMem = Marshal.AllocHGlobal(256);
        RESULT result = FMOD_Studio_Bank_GetStringInfo(this.handle, index, out id, stringMem, 256, out int retrieved);

        if (result == RESULT.ERR_TRUNCATED)
        {
            Marshal.FreeHGlobal(stringMem);
            stringMem = Marshal.AllocHGlobal(retrieved);
            result = FMOD_Studio_Bank_GetStringInfo(this.handle, index, out id, stringMem, retrieved, out retrieved);
        }

        if (result == RESULT.OK)
        {
            path = encoder.stringFromNative(stringMem);
        }
        Marshal.FreeHGlobal(stringMem);
        return result;
    }

    public readonly RESULT getEventCount(out int count) => FMOD_Studio_Bank_GetEventCount(this.handle, out count);
    public readonly RESULT getEventList(out EventDescription[] array)
    {
        array = null;

        RESULT result;
        result = FMOD_Studio_Bank_GetEventCount(this.handle, out int capacity);
        if (result != RESULT.OK)
        {
            return result;
        }
        if (capacity == 0)
        {
            array = [];
            return result;
        }

        IntPtr[] rawArray = new IntPtr[capacity];
        result = FMOD_Studio_Bank_GetEventList(this.handle, rawArray, capacity, out int actualCount);
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
            array[i].handle = rawArray[i];
        }
        return RESULT.OK;
    }
    public readonly RESULT getBusCount(out int count) => FMOD_Studio_Bank_GetBusCount(this.handle, out count);
    public readonly RESULT getBusList(out Bus[] array)
    {
        array = null;

        RESULT result;
        result = FMOD_Studio_Bank_GetBusCount(this.handle, out int capacity);
        if (result != RESULT.OK)
        {
            return result;
        }
        if (capacity == 0)
        {
            array = [];
            return result;
        }

        IntPtr[] rawArray = new IntPtr[capacity];
        result = FMOD_Studio_Bank_GetBusList(this.handle, rawArray, capacity, out int actualCount);
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
    public readonly RESULT getVCACount(out int count) => FMOD_Studio_Bank_GetVCACount(this.handle, out count);
    public readonly RESULT getVCAList(out VCA[] array)
    {
        array = null;

        RESULT result;
        result = FMOD_Studio_Bank_GetVCACount(this.handle, out int capacity);
        if (result != RESULT.OK)
        {
            return result;
        }
        if (capacity == 0)
        {
            array = [];
            return result;
        }

        IntPtr[] rawArray = new IntPtr[capacity];
        result = FMOD_Studio_Bank_GetVCAList(this.handle, rawArray, capacity, out int actualCount);
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

    public readonly RESULT getUserData(out IntPtr userdata) => FMOD_Studio_Bank_GetUserData(this.handle, out userdata);

    public readonly RESULT setUserData(IntPtr userdata) => FMOD_Studio_Bank_SetUserData(this.handle, userdata);

    #region importfunctions
    [DllImport(STUDIO_VERSION.dll)]
    private static extern bool   FMOD_Studio_Bank_IsValid                   (IntPtr bank);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_Bank_GetID                     (IntPtr bank, out GUID id);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_Bank_GetPath                   (IntPtr bank, IntPtr path, int size, out int retrieved);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_Bank_Unload                    (IntPtr bank);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_Bank_LoadSampleData            (IntPtr bank);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_Bank_UnloadSampleData          (IntPtr bank);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_Bank_GetLoadingState           (IntPtr bank, out LOADING_STATE state);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_Bank_GetSampleLoadingState     (IntPtr bank, out LOADING_STATE state);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_Bank_GetStringCount            (IntPtr bank, out int count);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_Bank_GetStringInfo             (IntPtr bank, int index, out GUID id, IntPtr path, int size, out int retrieved);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_Bank_GetEventCount             (IntPtr bank, out int count);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_Bank_GetEventList              (IntPtr bank, IntPtr[] array, int capacity, out int count);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_Bank_GetBusCount               (IntPtr bank, out int count);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_Bank_GetBusList                (IntPtr bank, IntPtr[] array, int capacity, out int count);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_Bank_GetVCACount               (IntPtr bank, out int count);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_Bank_GetVCAList                (IntPtr bank, IntPtr[] array, int capacity, out int count);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_Bank_GetUserData               (IntPtr bank, out IntPtr userdata);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_Bank_SetUserData               (IntPtr bank, IntPtr userdata);
    #endregion

    #region wrapperinternal

    public IntPtr handle = ptr;

    public readonly bool hasHandle() => this.handle != IntPtr.Zero;
    public void clearHandle() => this.handle = IntPtr.Zero;

    public readonly bool isValid() => hasHandle() && FMOD_Studio_Bank_IsValid(this.handle);

    #endregion
}

public struct CommandReplay(IntPtr ptr)
{
    // Information query
    public readonly RESULT getSystem(out System system) => FMOD_Studio_CommandReplay_GetSystem(this.handle, out system.handle);

    public readonly RESULT getLength(out float length) => FMOD_Studio_CommandReplay_GetLength(this.handle, out length);
    public readonly RESULT getCommandCount(out int count) => FMOD_Studio_CommandReplay_GetCommandCount(this.handle, out count);
    public readonly RESULT getCommandInfo(int commandIndex, out COMMAND_INFO info) => FMOD_Studio_CommandReplay_GetCommandInfo(this.handle, commandIndex, out info);

    public readonly RESULT getCommandString(int commandIndex, out string buffer)
    {
        buffer = null;
        using StringHelper.ThreadSafeEncoding encoder = StringHelper.GetFreeHelper();
        int stringLength = 256;
        IntPtr stringMem = Marshal.AllocHGlobal(256);
        RESULT result = FMOD_Studio_CommandReplay_GetCommandString(this.handle, commandIndex, stringMem, stringLength);

        while (result == RESULT.ERR_TRUNCATED)
        {
            Marshal.FreeHGlobal(stringMem);
            stringLength *= 2;
            stringMem = Marshal.AllocHGlobal(stringLength);
            result = FMOD_Studio_CommandReplay_GetCommandString(this.handle, commandIndex, stringMem, stringLength);
        }

        if (result == RESULT.OK)
        {
            buffer = encoder.stringFromNative(stringMem);
        }
        Marshal.FreeHGlobal(stringMem);
        return result;
    }
    public readonly RESULT getCommandAtTime(float time, out int commandIndex) => FMOD_Studio_CommandReplay_GetCommandAtTime(this.handle, time, out commandIndex);
    // Playback
    public readonly RESULT setBankPath(string bankPath)
    {
        using StringHelper.ThreadSafeEncoding encoder = StringHelper.GetFreeHelper();
        return FMOD_Studio_CommandReplay_SetBankPath(this.handle, encoder.byteFromStringUTF8(bankPath));
    }
    public readonly RESULT start() => FMOD_Studio_CommandReplay_Start(this.handle);
    public readonly RESULT stop() => FMOD_Studio_CommandReplay_Stop(this.handle);
    public readonly RESULT seekToTime(float time) => FMOD_Studio_CommandReplay_SeekToTime(this.handle, time);
    public readonly RESULT seekToCommand(int commandIndex) => FMOD_Studio_CommandReplay_SeekToCommand(this.handle, commandIndex);
    public readonly RESULT getPaused(out bool paused) => FMOD_Studio_CommandReplay_GetPaused(this.handle, out paused);
    public readonly RESULT setPaused(bool paused) => FMOD_Studio_CommandReplay_SetPaused(this.handle, paused);
    public readonly RESULT getPlaybackState(out PLAYBACK_STATE state) => FMOD_Studio_CommandReplay_GetPlaybackState(this.handle, out state);
    public readonly RESULT getCurrentCommand(out int commandIndex, out float currentTime) => FMOD_Studio_CommandReplay_GetCurrentCommand(this.handle, out commandIndex, out currentTime);
    // Release
    public readonly RESULT release() => FMOD_Studio_CommandReplay_Release(this.handle);
    // Callbacks
    public readonly RESULT setFrameCallback(COMMANDREPLAY_FRAME_CALLBACK callback) => FMOD_Studio_CommandReplay_SetFrameCallback(this.handle, callback);
    public readonly RESULT setLoadBankCallback(COMMANDREPLAY_LOAD_BANK_CALLBACK callback) => FMOD_Studio_CommandReplay_SetLoadBankCallback(this.handle, callback);
    public readonly RESULT setCreateInstanceCallback(COMMANDREPLAY_CREATE_INSTANCE_CALLBACK callback) => FMOD_Studio_CommandReplay_SetCreateInstanceCallback(this.handle, callback);
    public readonly RESULT getUserData(out IntPtr userdata) => FMOD_Studio_CommandReplay_GetUserData(this.handle, out userdata);
    public readonly RESULT setUserData(IntPtr userdata) => FMOD_Studio_CommandReplay_SetUserData(this.handle, userdata);

    #region importfunctions
    [DllImport(STUDIO_VERSION.dll)]
    private static extern bool FMOD_Studio_CommandReplay_IsValid                    (IntPtr replay);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_CommandReplay_GetSystem                (IntPtr replay, out IntPtr system);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_CommandReplay_GetLength                (IntPtr replay, out float length);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_CommandReplay_GetCommandCount          (IntPtr replay, out int count);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_CommandReplay_GetCommandInfo           (IntPtr replay, int commandindex, out COMMAND_INFO info);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_CommandReplay_GetCommandString         (IntPtr replay, int commandIndex, IntPtr buffer, int length);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_CommandReplay_GetCommandAtTime         (IntPtr replay, float time, out int commandIndex);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_CommandReplay_SetBankPath              (IntPtr replay, byte[] bankPath);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_CommandReplay_Start                    (IntPtr replay);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_CommandReplay_Stop                     (IntPtr replay);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_CommandReplay_SeekToTime               (IntPtr replay, float time);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_CommandReplay_SeekToCommand            (IntPtr replay, int commandIndex);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_CommandReplay_GetPaused                (IntPtr replay, out bool paused);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_CommandReplay_SetPaused                (IntPtr replay, bool paused);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_CommandReplay_GetPlaybackState         (IntPtr replay, out PLAYBACK_STATE state);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_CommandReplay_GetCurrentCommand        (IntPtr replay, out int commandIndex, out float currentTime);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_CommandReplay_Release                  (IntPtr replay);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_CommandReplay_SetFrameCallback         (IntPtr replay, COMMANDREPLAY_FRAME_CALLBACK callback);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_CommandReplay_SetLoadBankCallback      (IntPtr replay, COMMANDREPLAY_LOAD_BANK_CALLBACK callback);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_CommandReplay_SetCreateInstanceCallback(IntPtr replay, COMMANDREPLAY_CREATE_INSTANCE_CALLBACK callback);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_CommandReplay_GetUserData              (IntPtr replay, out IntPtr userdata);
    [DllImport(STUDIO_VERSION.dll)]
    private static extern RESULT FMOD_Studio_CommandReplay_SetUserData              (IntPtr replay, IntPtr userdata);
    #endregion

    #region wrapperinternal

    public IntPtr handle = ptr;

    public readonly bool hasHandle() => this.handle != IntPtr.Zero;
    public void clearHandle() => this.handle = IntPtr.Zero;

    public readonly bool isValid() => hasHandle() && FMOD_Studio_CommandReplay_IsValid(this.handle);

    #endregion
}
// FMOD
