/* ==============================================================================================  */
/* FMOD Core / Studio API - Error string header file.                                              */
/* Copyright (c), Firelight Technologies Pty, Ltd. 2004-2022.                                      */
/*                                                                                                 */
/* Use this header if you want to store or display a string version / english explanation          */
/* of the FMOD error codes.                                                                        */
/*                                                                                                 */
/* For more detail visit:                                                                          */
/* https://fmod.com/docs/2.02/api/core-api-common.html#fmod_result                                 */
/* =============================================================================================== */

#nullable disable

namespace FMOD;

public class Error
{
    public static string String(FMOD.RESULT errcode) => errcode switch
    {
        FMOD.RESULT.OK => "No errors.",
        FMOD.RESULT.ERR_BADCOMMAND => "Tried to call a function on a data type that does not allow this type of functionality (ie calling Sound::lock on a streaming sound).",
        FMOD.RESULT.ERR_CHANNEL_ALLOC => "Error trying to allocate a channel.",
        FMOD.RESULT.ERR_CHANNEL_STOLEN => "The specified channel has been reused to play another sound.",
        FMOD.RESULT.ERR_DMA => "DMA Failure.  See debug output for more information.",
        FMOD.RESULT.ERR_DSP_CONNECTION => "DSP connection error.  Connection possibly caused a cyclic dependency or connected dsps with incompatible buffer counts.",
        FMOD.RESULT.ERR_DSP_DONTPROCESS => "DSP return code from a DSP process query callback.  Tells mixer not to call the process callback and therefore not consume CPU.  Use this to optimize the DSP graph.",
        FMOD.RESULT.ERR_DSP_FORMAT => "DSP Format error.  A DSP unit may have attempted to connect to this network with the wrong format, or a matrix may have been set with the wrong size if the target unit has a specified channel map.",
        FMOD.RESULT.ERR_DSP_INUSE => "DSP is already in the mixer's DSP network. It must be removed before being reinserted or released.",
        FMOD.RESULT.ERR_DSP_NOTFOUND => "DSP connection error.  Couldn't find the DSP unit specified.",
        FMOD.RESULT.ERR_DSP_RESERVED => "DSP operation error.  Cannot perform operation on this DSP as it is reserved by the system.",
        FMOD.RESULT.ERR_DSP_SILENCE => "DSP return code from a DSP process query callback.  Tells mixer silence would be produced from read, so go idle and not consume CPU.  Use this to optimize the DSP graph.",
        FMOD.RESULT.ERR_DSP_TYPE => "DSP operation cannot be performed on a DSP of this type.",
        FMOD.RESULT.ERR_FILE_BAD => "Error loading file.",
        FMOD.RESULT.ERR_FILE_COULDNOTSEEK => "Couldn't perform seek operation.  This is a limitation of the medium (ie netstreams) or the file format.",
        FMOD.RESULT.ERR_FILE_DISKEJECTED => "Media was ejected while reading.",
        FMOD.RESULT.ERR_FILE_EOF => "End of file unexpectedly reached while trying to read essential data (truncated?).",
        FMOD.RESULT.ERR_FILE_ENDOFDATA => "End of current chunk reached while trying to read data.",
        FMOD.RESULT.ERR_FILE_NOTFOUND => "File not found.",
        FMOD.RESULT.ERR_FORMAT => "Unsupported file or audio format.",
        FMOD.RESULT.ERR_HEADER_MISMATCH => "There is a version mismatch between the FMOD header and either the FMOD Studio library or the FMOD Low Level library.",
        FMOD.RESULT.ERR_HTTP => "A HTTP error occurred. This is a catch-all for HTTP errors not listed elsewhere.",
        FMOD.RESULT.ERR_HTTP_ACCESS => "The specified resource requires authentication or is forbidden.",
        FMOD.RESULT.ERR_HTTP_PROXY_AUTH => "Proxy authentication is required to access the specified resource.",
        FMOD.RESULT.ERR_HTTP_SERVER_ERROR => "A HTTP server error occurred.",
        FMOD.RESULT.ERR_HTTP_TIMEOUT => "The HTTP request timed out.",
        FMOD.RESULT.ERR_INITIALIZATION => "FMOD was not initialized correctly to support this function.",
        FMOD.RESULT.ERR_INITIALIZED => "Cannot call this command after System::init.",
        FMOD.RESULT.ERR_INTERNAL => "An error occurred that wasn't supposed to.  Contact support.",
        FMOD.RESULT.ERR_INVALID_FLOAT => "Value passed in was a NaN, Inf or denormalized float.",
        FMOD.RESULT.ERR_INVALID_HANDLE => "An invalid object handle was used.",
        FMOD.RESULT.ERR_INVALID_PARAM => "An invalid parameter was passed to this function.",
        FMOD.RESULT.ERR_INVALID_POSITION => "An invalid seek position was passed to this function.",
        FMOD.RESULT.ERR_INVALID_SPEAKER => "An invalid speaker was passed to this function based on the current speaker mode.",
        FMOD.RESULT.ERR_INVALID_SYNCPOINT => "The syncpoint did not come from this sound handle.",
        FMOD.RESULT.ERR_INVALID_THREAD => "Tried to call a function on a thread that is not supported.",
        FMOD.RESULT.ERR_INVALID_VECTOR => "The vectors passed in are not unit length, or perpendicular.",
        FMOD.RESULT.ERR_MAXAUDIBLE => "Reached maximum audible playback count for this sound's soundgroup.",
        FMOD.RESULT.ERR_MEMORY => "Not enough memory or resources.",
        FMOD.RESULT.ERR_MEMORY_CANTPOINT => "Can't use FMOD_OPENMEMORY_POINT on non PCM source data, or non mp3/xma/adpcm data if FMOD_CREATECOMPRESSEDSAMPLE was used.",
        FMOD.RESULT.ERR_NEEDS3D => "Tried to call a command on a 2d sound when the command was meant for 3d sound.",
        FMOD.RESULT.ERR_NEEDSHARDWARE => "Tried to use a feature that requires hardware support.",
        FMOD.RESULT.ERR_NET_CONNECT => "Couldn't connect to the specified host.",
        FMOD.RESULT.ERR_NET_SOCKET_ERROR => "A socket error occurred.  This is a catch-all for socket-related errors not listed elsewhere.",
        FMOD.RESULT.ERR_NET_URL => "The specified URL couldn't be resolved.",
        FMOD.RESULT.ERR_NET_WOULD_BLOCK => "Operation on a non-blocking socket could not complete immediately.",
        FMOD.RESULT.ERR_NOTREADY => "Operation could not be performed because specified sound/DSP connection is not ready.",
        FMOD.RESULT.ERR_OUTPUT_ALLOCATED => "Error initializing output device, but more specifically, the output device is already in use and cannot be reused.",
        FMOD.RESULT.ERR_OUTPUT_CREATEBUFFER => "Error creating hardware sound buffer.",
        FMOD.RESULT.ERR_OUTPUT_DRIVERCALL => "A call to a standard soundcard driver failed, which could possibly mean a bug in the driver or resources were missing or exhausted.",
        FMOD.RESULT.ERR_OUTPUT_FORMAT => "Soundcard does not support the specified format.",
        FMOD.RESULT.ERR_OUTPUT_INIT => "Error initializing output device.",
        FMOD.RESULT.ERR_OUTPUT_NODRIVERS => "The output device has no drivers installed.  If pre-init, FMOD_OUTPUT_NOSOUND is selected as the output mode.  If post-init, the function just fails.",
        FMOD.RESULT.ERR_PLUGIN => "An unspecified error has been returned from a plugin.",
        FMOD.RESULT.ERR_PLUGIN_MISSING => "A requested output, dsp unit type or codec was not available.",
        FMOD.RESULT.ERR_PLUGIN_RESOURCE => "A resource that the plugin requires cannot be allocated or found. (ie the DLS file for MIDI playback)",
        FMOD.RESULT.ERR_PLUGIN_VERSION => "A plugin was built with an unsupported SDK version.",
        FMOD.RESULT.ERR_RECORD => "An error occurred trying to initialize the recording device.",
        FMOD.RESULT.ERR_REVERB_CHANNELGROUP => "Reverb properties cannot be set on this channel because a parent channelgroup owns the reverb connection.",
        FMOD.RESULT.ERR_REVERB_INSTANCE => "Specified instance in FMOD_REVERB_PROPERTIES couldn't be set. Most likely because it is an invalid instance number or the reverb doesn't exist.",
        FMOD.RESULT.ERR_SUBSOUNDS => "The error occurred because the sound referenced contains subsounds when it shouldn't have, or it doesn't contain subsounds when it should have.  The operation may also not be able to be performed on a parent sound.",
        FMOD.RESULT.ERR_SUBSOUND_ALLOCATED => "This subsound is already being used by another sound, you cannot have more than one parent to a sound.  Null out the other parent's entry first.",
        FMOD.RESULT.ERR_SUBSOUND_CANTMOVE => "Shared subsounds cannot be replaced or moved from their parent stream, such as when the parent stream is an FSB file.",
        FMOD.RESULT.ERR_TAGNOTFOUND => "The specified tag could not be found or there are no tags.",
        FMOD.RESULT.ERR_TOOMANYCHANNELS => "The sound created exceeds the allowable input channel count.  This can be increased using the 'maxinputchannels' parameter in System::setSoftwareFormat.",
        FMOD.RESULT.ERR_TRUNCATED => "The retrieved string is too long to fit in the supplied buffer and has been truncated.",
        FMOD.RESULT.ERR_UNIMPLEMENTED => "Something in FMOD hasn't been implemented when it should be! contact support!",
        FMOD.RESULT.ERR_UNINITIALIZED => "This command failed because System::init or System::setDriver was not called.",
        FMOD.RESULT.ERR_UNSUPPORTED => "A command issued was not supported by this object.  Possibly a plugin without certain callbacks specified.",
        FMOD.RESULT.ERR_VERSION => "The version number of this file format is not supported.",
        FMOD.RESULT.ERR_EVENT_ALREADY_LOADED => "The specified bank has already been loaded.",
        FMOD.RESULT.ERR_EVENT_LIVEUPDATE_BUSY => "The live update connection failed due to the game already being connected.",
        FMOD.RESULT.ERR_EVENT_LIVEUPDATE_MISMATCH => "The live update connection failed due to the game data being out of sync with the tool.",
        FMOD.RESULT.ERR_EVENT_LIVEUPDATE_TIMEOUT => "The live update connection timed out.",
        FMOD.RESULT.ERR_EVENT_NOTFOUND => "The requested event, bus or vca could not be found.",
        FMOD.RESULT.ERR_STUDIO_UNINITIALIZED => "The Studio::System object is not yet initialized.",
        FMOD.RESULT.ERR_STUDIO_NOT_LOADED => "The specified resource is not loaded, so it can't be unloaded.",
        FMOD.RESULT.ERR_INVALID_STRING => "An invalid string was passed to this function.",
        FMOD.RESULT.ERR_ALREADY_LOCKED => "The specified resource is already locked.",
        FMOD.RESULT.ERR_NOT_LOCKED => "The specified resource is not locked, so it can't be unlocked.",
        FMOD.RESULT.ERR_RECORD_DISCONNECTED => "The specified recording driver has been disconnected.",
        FMOD.RESULT.ERR_TOOMANYSAMPLES => "The length provided exceed the allowable limit.",
        _ => "Unknown error.",
    };
}
