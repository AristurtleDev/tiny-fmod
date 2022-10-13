namespace TinyFmod;

/// <summary>
///     A tiny wrapper around the FMOD Studio API that exposes common utility
///     functions for using the FMOD Studio API.
/// </summary>
public sealed class FmodStudio
{
    //  All banks that have been loaded and told to cache.
    private Dictionary<string, FMOD.Studio.Bank> _cachedBanks = new();

    //  Cache reference to event descriptions that are loaded
    private Dictionary<string, FMOD.Studio.EventDescription> _events = new();

    //  The underlying FMOD Studio System instance.
    private readonly FMOD.Studio.System _studio;

    //  The underlying FMOD Core System instance.
    private readonly FMOD.System _core;

    //  A reusualbe ATTRIBUTE_32 struct that is used when setting the position
    //  of an event instance.
    private FMOD.ATTRIBUTES_3D _3dAttribute;

    /// <summary>
    ///     Gets a value that indicates whether this <see cref="FmodStudio"/>
    ///     instance has been disposed of.
    /// </summary>
    public bool IsDisposed { get; private set; }

    /// <summary>
    ///     Gets the reference to the underlying FMOD Studio System instance.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if the handle for the FMOD Studio System has been released.
    /// </exception>
    public FMOD.Studio.System StudioSystem
    {
        get
        {
            if (!_studio.hasHandle())
            {
                throw new InvalidOperationException("Underlying FMOD Studio System handle has been released");
            }

            return _studio;
        }
    }

    /// <summary>
    ///     Gets the reference to the underlying FMOD Core System instance.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if the handle for the FMOD Core System has been released.
    /// </exception>
    public FMOD.System CoreSystem
    {
        get
        {
            if (!_core.hasHandle())
            {
                throw new InvalidOperationException("Underlying FMOD System handle has been released");
            }

            return _core;
        }
    }

    /// <summary>
    ///     Gets the <see cref="FMOD.Studio.EventInstance"/> of the current
    ///     music that is playing, if any; otherwise, <see langword="null"/>.
    /// </summary>
    public FMOD.Studio.EventInstance? CurrentMusicEvent { get; private set; }

    /// <summary>
    ///     Gets the event path for the <see cref="CurrentMusicEvent"/>, if
    ///     there is currently music playing; otherwise, <see langword="null"/>.
    /// </summary>
    public string? CurrentMusicPath { get; private set; }

    /// <summary>
    ///     Creates a new <see cref="FmodStudio"/> class instance initialized
    ///     using default values.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         This constructor is for quick creating of this class. It will
    ///         set the max channels to 1024, initialize the core system using
    ///         <see cref="FMOD.INITFLAGS.NORMAL"/>, and set the extra driver
    ///         data initialization paramter to <see cref="IntPtr.Zero"/>.
    ///     </para>
    ///     <para>
    ///         To manually set all initialization parameters, use the alternate
    ///         <see cref="FmodStudio(FMOD.Studio.INITFLAGS, FMOD.INITFLAGS, int, IntPtr)"/>
    ///         constructor instead.
    ///     </para>
    /// </remarks>
    /// <param name="liveUpdate">
    ///     Whether to use the <see cref="FMOD.Studio.INITFLAGS.LIVEUPDATE"/>
    ///     flag when initializing the studio system.
    /// </param>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if the calls to the underlying studio system returns a
    ///     <see cref="FMOD.RESULT"/> value that is anything except
    ///     <see cref="FMOD.RESULT.OK"/>.
    /// </exception>
    public FmodStudio(bool liveUpdate) : this
    (
        studioInitFlags: liveUpdate ? FMOD.Studio.INITFLAGS.LIVEUPDATE : FMOD.Studio.INITFLAGS.NORMAL,
        coreInitFlags: FMOD.INITFLAGS.NORMAL,
        maxChannels: 1024,
        extraDriverData: IntPtr.Zero
    )
    { }

    /// <summary>
    ///     Creates a new <see cref="FmodStudio"/> class instance and
    ///     initializes the underlying studio and core systems using the
    ///     values provided.
    /// </summary>
    /// <param name="studioInitFlags">
    ///     The <see cref="FMOD.Studio.INITFLAGS"/> to use when initializing the
    ///     studio system.
    /// </param>
    /// <param name="coreInitFlags">
    ///     The <see cref="FMOD.INITFLAGS"/> to use when initializing the core
    ///     system.
    /// </param>
    /// <param name="maxChannels">
    ///     The maximum number of channels to be used.
    /// </param>
    /// <param name="extraDriverData">
    ///     Driver specific data to be passed to the output plugin.
    /// </param>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if the calls to the underlying studio system returns a
    ///     <see cref="FMOD.RESULT"/> value that is anything except
    ///     <see cref="FMOD.RESULT.OK"/>.
    /// </exception>
    public FmodStudio(FMOD.Studio.INITFLAGS studioInitFlags, FMOD.INITFLAGS coreInitFlags, int maxChannels, IntPtr extraDriverData)
    {
        //  Per the FMOD documentation, we need to make a Core System API call
        //  before we initialize the Studio System.
        //
        //  https://www.fmod.com/docs/2.02/api/studio-guide.html#getting-started
        //
        //  We can just call the FMOD::Memory_getStats. It has low overhead and
        //  will load the external library references
        FMOD.Memory.GetStats(out _, out _);

        //  Create the underlying studio system.
        ThrowIfNotOK(FMOD.Studio.System.create(out _studio));

        //  Get a reference to the underlying core system.
        ThrowIfNotOK(StudioSystem.getCoreSystem(out _core));

        //  Initialize the studio system. This will also internally initialize
        //  the core system
        StudioSystem.initialize(maxChannels, studioInitFlags, coreInitFlags, extraDriverData);

        //  Initialize the 3D Attribute for any 3D based events
        _3dAttribute.forward = new FMOD.VECTOR()
        {
            x = 0.0f,
            y = 0.0f,
            z = 1.0f
        };

        _3dAttribute.up = new FMOD.VECTOR()
        {
            x = 0.0f,
            y = 1.0f,
            z = 0.0f
        };
    }

    //  Finalizer implementation to internally call Dispose passing false.
    ~FmodStudio() => Dispose(false);

    /// <summary>
    ///     Load the bank from the given <paramref name="path"/>.
    /// </summary>
    /// <param name="path">
    ///     The absolute file path to the bank file to laod.
    /// </param>
    /// <param name="cacheKey">
    ///     When provided, will be used as the key to internally cache the bank,
    ///     which can be used to retrieve the bank using the
    /// </param>
    /// <returns>
    ///     The <see cref="FMOD.Studio.Bank"/> that was loaded.
    /// </returns>
    /// <exception cref="FileNotFoundException">
    ///     Thrown if no file exists at the <paramref name="path"/> given.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if the calls to the underlying studio system returns a
    ///     <see cref="FMOD.RESULT"/> value that is anything except
    ///     <see cref="FMOD.RESULT.OK"/>.
    /// </exception>
    public FMOD.Studio.Bank LoadBank(string path, string? cacheKey)
    {
        //  Ensure the path given is valid
        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"No file exists at the path given. Path given was '{path}'");
        }

        //  Load the bank into the studio system
        ThrowIfNotOK(StudioSystem.loadBankFile(path, FMOD.Studio.LOAD_BANK_FLAGS.NORMAL, out FMOD.Studio.Bank bank));

        //  Were we asked to cache the bank?
        if (!string.IsNullOrEmpty(cacheKey))
        {
            _cachedBanks.Add(cacheKey, bank);
        }

        return bank;
    }

    /// <summary>
    ///     Returns the <see cref="FMOD.Studio.Bank"/> from the internal cache
    ///     of loaded banks with the specified <paramref name="key"/>.
    /// </summary>
    /// <param name="key">
    ///     The key that was used to cache the bank when the bank was loaded.
    /// </param>
    /// <param name="bank">
    ///     When this method returns, contains the
    ///     <see cref="FMOD.Studio.Bank"/> associated with the given
    ///     <paramref name="key"/> if <see langword="true"/>, otherwise
    ///     <see langword="null"/>.
    /// </param>
    /// <returns>
    ///     <see langword="true"/> if the bank element exists in the underlying
    ///     bank cache; otherwise, <see langword="false"/>.
    /// </returns>
    public bool TryGetCachedBank(string key, out FMOD.Studio.Bank bank) =>
        _cachedBanks.TryGetValue(key, out bank);

    /// <summary>
    ///     Updates the underlying FMOD Studio System.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if the handle for the underlying FMOD Studio System has been
    ///     released, or if the call to update the system returns an
    ///     <see cref="FMOD.RESULT"/> value that is anything except
    ///     <see cref="FMOD.RESULT.OK"/>.
    /// </exception>
    public void Update() => ThrowIfNotOK(StudioSystem.update());

    /// <summary>
    ///     Gets the <see cref="FMOD.Studio.EventDescription"/> at the specified
    ///     event <paramref name="path"/> from the underlying studio system.
    /// </summary>
    /// <param name="path">
    ///     The event path for the <see cref="FMOD.Studio.EventDescription"/>
    ///     to retreive.  Event paths start with "event:/".  For example, if
    ///     retreiving the an event named "song" inside a folder called
    ///     "music", the event path woudl be "event:/music/song".
    /// </param>
    /// <returns>
    ///     The <see cref="FMOD.Studio.EventDescription"/> retreived.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if the calls to the underlying studio system returns a
    ///     <see cref="FMOD.RESULT"/> value that is anything except
    ///     <see cref="FMOD.RESULT.OK"/>.
    /// </exception>
    public FMOD.Studio.EventDescription GetEventDescription(string path)
    {
        //  Check if the event has been cached, if so, retrieve from there,
        //  otherwise, load from the studio system
        if (!_events.TryGetValue(path, out FMOD.Studio.EventDescription description))
        {
            ThrowIfNotOK(StudioSystem.getEvent(path, out description));
        }

        return description;
    }

    /// <summary>
    ///     Creates a new <see cref="FMOD.Studio.EventInstance"/> from the
    ///     <see cref="FMOD.Studio.EventDescription"/> at the specified
    ///     <paramref name="eventPath"/>.
    /// </summary>
    /// <param name="eventPath">
    ///     The event path for the <see cref="FMOD.Studio.EventDescription"/>
    ///     to retreive.  Event paths start with "event:/".  For example, if
    ///     retreiving the an event named "song" inside a folder called
    ///     "music", the event path woudl be "event:/music/song".
    /// </param>
    /// <param name="posX">
    ///     If the <see cref="FMOD.Studio.EventDescription"/> used to create the
    ///     instance is in 3D, this will set the x-coordinate position of the
    ///     instance created.
    /// </param>
    /// <param name="posY">
    ///     If the <see cref="FMOD.Studio.EventDescription"/> used to create the
    ///     instance is in 3D, this will set the y-coordinate position of the
    ///     instance created.
    /// </param>
    /// <param name="originX">
    ///     If the <see cref="FMOD.Studio.EventDescription"/> used to create the
    ///     instance is in 3D, this will set the x-coordinate origin position
    ///     of the instance created.
    /// </param>
    /// <param name="originY">
    ///     If the <see cref="FMOD.Studio.EventDescription"/> used to create the
    ///     instance is in 3D, this will set the y-coordinate origin position
    ///     of the instance created.
    /// </param>
    /// <returns>
    ///     The <see cref="FMOD.Studio.EventInstance"/> that is created.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if the calls to the underlying studio system returns a
    ///     <see cref="FMOD.RESULT"/> value that is anything except
    ///     <see cref="FMOD.RESULT.OK"/>.
    /// </exception>
    public FMOD.Studio.EventInstance CreateInstance(string eventPath, float posX = 0.0f, float posY = 0.0f, float originX = 0.0f, float originY = 0.0f)
    {
        //  In order to create an instance of an event, we first have to get the
        //  event description.
        FMOD.Studio.EventDescription description = GetEventDescription(eventPath);

        //  Now create an instance of that event using the studio system.
        ThrowIfNotOK(description.createInstance(out FMOD.Studio.EventInstance instance));

        //  If the event has any properties that make it a 3D event, then we
        //  need to update the parameters for that
        ThrowIfNotOK(description.is3D(out bool is3d));
        if (is3d)
        {
            SetEventInstancePosition(instance, posX, posY, originX, originY);
        }

        return instance;
    }

    /// <summary>
    ///     Sets the position of the given
    ///     <see cref="FMOD.Studio.EventInstance"/>.
    /// </summary>
    /// <param name="instance">
    ///     The <see cref="FMOD.Studio.EventInstance"/> to set the position of.
    /// </param>
    /// <param name="posX">
    ///     The x-coordinate position to set.
    /// </param>
    /// <param name="posY">
    ///     The y-coordinate position to set.
    /// </param>
    /// <param name="originX">
    ///     The x-coordinate origin point.  This could be something like a
    ///     camera.
    /// </param>
    /// <param name="originY">
    ///     The y-coordinate origin point.  This could be something like a
    ///     camera.
    /// </param>
    public void SetEventInstancePosition(FMOD.Studio.EventInstance instance, float posX, float posY, float originX, float originY)
    {
        _3dAttribute.position.x = posX - originX;
        _3dAttribute.position.y = posY - originY;
        _3dAttribute.position.z = 0.0f;

        ThrowIfNotOK(instance.set3DAttributes(_3dAttribute));
    }

    /// <summary>
    ///     Sets the parameter with the specified <paramref name="name"/> to
    ///     the specified <paramref name="value"/> for the specified
    ///     <see cref="FMOD.Studio.EventInstance"/>.
    /// </summary>
    /// <param name="instance">
    ///     The <see cref="FMOD.Studio.EventInstance"/> to set the parameter
    ///     value of.
    /// </param>
    /// <param name="name">
    ///     The name of the parameter to set the value of.
    /// </param>
    /// <param name="value">
    ///     The value to set the paramter to.
    /// </param>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if the calls to the underlying studio system returns a
    ///     <see cref="FMOD.RESULT"/> value that is anything except
    ///     <see cref="FMOD.RESULT.OK"/>.
    /// </exception>
    public void SetEventInstanceParameter(FMOD.Studio.EventInstance instance, string name, float value) =>
        ThrowIfNotOK(instance.setParameterByName(name, value));

    /// <summary>
    ///     Sets multiple parameter values for the specified
    ///     <see cref="FMOD.Studio.EventInstance"/>
    /// </summary>
    /// <param name="instance">
    ///     The <see cref="FMOD.Studio.EventInstance"/> to set the parameter
    ///     value of.
    /// </param>
    /// <param name="parameters">
    ///     A <see cref="Dictionary{TKey, TValue}"/> where each element the
    ///     key is the name of a paramter and the value is the value to set
    ///     the parameter to.
    /// </param>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if the calls to the underlying studio system returns a
    ///     <see cref="FMOD.RESULT"/> value that is anything except
    ///     <see cref="FMOD.RESULT.OK"/>.
    /// </exception>
    public void SetEventInstanceParameters(FMOD.Studio.EventInstance instance, Dictionary<string, float> parameters)
    {
        foreach (KeyValuePair<string, float> kvp in parameters)
        {
            ThrowIfNotOK(instance.setParameterByName(kvp.Key, kvp.Value));
        }
    }

    /// <summary>
    ///     Plays the <see cref="FMOD.Studio.EventInstance"/> at the specified
    ///     <paramref name="eventPath"/> as a one shot sound effect.
    /// </summary>
    /// <param name="eventPath">
    ///     The event path for the <see cref="FMOD.Studio.EventDescription"/>
    ///     to retreive.  Event paths start with "event:/".  For example, if
    ///     retreiving the an event named "song" inside a folder called
    ///     "music", the event path woudl be "event:/music/song".
    /// </param>
    /// <returns>
    ///     The <see cref="FMOD.Studio.EventInstance"/> that is created.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if the calls to the underlying studio system returns a
    ///     <see cref="FMOD.RESULT"/> value that is anything except
    ///     <see cref="FMOD.RESULT.OK"/>.
    /// </exception>
    public FMOD.Studio.EventInstance PlaySoundEffect(string eventPath)
    {
        //  Create a new instance of the event
        FMOD.Studio.EventInstance instance = CreateInstance(eventPath);

        //  Start the instance (this start playback)
        ThrowIfNotOK(instance.start());

        ///  Per FMOD Documentation, Studio::EventInstance::release should be
        //  called immediately after Studio::EventInstance::start if we do
        //  not need to all it again later, which we don't for sound effects
        //  which are one shot plays
        //
        //  https://www.fmod.com/docs/2.02/api/studio-api-eventinstance.html#studio_eventinstance_start
        ThrowIfNotOK(instance.release());

        //  Return the instance
        return instance;
    }

    /// <summary>
    ///     Plays the <see cref="FMOD.Studio.EventInstance"/> at the specified
    ///     <paramref name="eventPath"/> as a one shot sound effect and sets
    ///     a paramter value of the instance with the <paramref name="param"/>
    ///     name given, to the <paramref name="value"/> specified before
    ///     staring the playback.
    /// </summary>
    /// <param name="eventPath">
    ///     The event path for the <see cref="FMOD.Studio.EventDescription"/>
    ///     to retreive.  Event paths start with "event:/".  For example, if
    ///     retreiving the an event named "song" inside a folder called
    ///     "music", the event path woudl be "event:/music/song".
    /// </param>
    /// <param name="param">
    ///     The name of the paramter to set the value of.
    /// </param>
    /// <param name="value">
    ///     The value to set the parameter too.
    /// </param>
    /// <returns>
    ///     The <see cref="FMOD.Studio.EventInstance"/> that is created.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if the calls to the underlying studio system returns a
    ///     <see cref="FMOD.RESULT"/> value that is anything except
    ///     <see cref="FMOD.RESULT.OK"/>.
    /// </exception>
    public FMOD.Studio.EventInstance PlaySoundEffect(string eventPath, string param, float value)
    {
        //  Create a new instance of the event
        FMOD.Studio.EventInstance instance = CreateInstance(eventPath);

        //  Set the provided parameter
        SetEventInstanceParameter(instance, param, value);

        //  Start the instance (this start playback)
        ThrowIfNotOK(instance.start());

        ///  Per FMOD Documentation, Studio::EventInstance::release should be
        //  called immediately after Studio::EventInstance::start if we do
        //  not need to all it again later, which we don't for sound effects
        //  which are one shot plays
        //
        //  https://www.fmod.com/docs/2.02/api/studio-api-eventinstance.html#studio_eventinstance_start
        ThrowIfNotOK(instance.release());

        //  Return the instance
        return instance;
    }

    /// <summary>
    ///     Plays the <see cref="FMOD.Studio.EventInstance"/> at the specified
    ///     <paramref name="eventPath"/> as a one shot sound effect and sets
    ///     the parameters values specified before starting playback.
    /// </summary>
    /// <param name="eventPath">
    ///     The event path for the <see cref="FMOD.Studio.EventDescription"/>
    ///     to retreive.  Event paths start with "event:/".  For example, if
    ///     retreiving the an event named "song" inside a folder called
    ///     "music", the event path woudl be "event:/music/song".
    /// </param>
    /// <param name="parameters">
    ///     A <see cref="Dictionary{TKey, TValue}"/> where each element the
    ///     key is the name of a paramter and the value is the value to set
    ///     the parameter to.
    /// </param>
    /// <returns>
    ///     The <see cref="FMOD.Studio.EventInstance"/> that is created.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if the calls to the underlying studio system returns a
    ///     <see cref="FMOD.RESULT"/> value that is anything except
    ///     <see cref="FMOD.RESULT.OK"/>.
    /// </exception>
    public FMOD.Studio.EventInstance PlaySoundEffect(string eventPath, Dictionary<string, float> parameters)
    {
        //  Create a new instance of the event
        FMOD.Studio.EventInstance instance = CreateInstance(eventPath);

        //  Set the provided parameters
        SetEventInstanceParameters(instance, parameters);

        //  Start the instance (this start playback)
        ThrowIfNotOK(instance.start());

        ///  Per FMOD Documentation, Studio::EventInstance::release should be
        //  called immediately after Studio::EventInstance::start if we do
        //  not need to all it again later, which we don't for sound effects
        //  which are one shot plays
        //
        //  https://www.fmod.com/docs/2.02/api/studio-api-eventinstance.html#studio_eventinstance_start
        ThrowIfNotOK(instance.release());

        //  Return the instance
        return instance;
    }

    /// <summary>
    ///     Plays the <see cref="FMOD.Studio.EventInstance"/> at the specified
    ///     <paramref name="eventPath"/> and sets the 3D attribuetes of the
    ///     instance.
    /// </summary>
    /// <param name="eventPath">
    ///     The event path for the <see cref="FMOD.Studio.EventDescription"/>
    ///     to retreive.  Event paths start with "event:/".  For example, if
    ///     retreiving the an event named "song" inside a folder called
    ///     "music", the event path woudl be "event:/music/song".
    /// </param>
    /// <param name="x">
    ///     If the <see cref="FMOD.Studio.EventDescription"/> used to create the
    ///     instance is in 3D, this will set the x-coordinate position of the
    ///     instance created.
    /// </param>
    /// <param name="y">
    ///     If the <see cref="FMOD.Studio.EventDescription"/> used to create the
    ///     instance is in 3D, this will set the y-coordinate position of the
    ///     instance created.
    /// </param>
    /// <param name="originX">
    ///     If the <see cref="FMOD.Studio.EventDescription"/> used to create the
    ///     instance is in 3D, this will set the x-coordinate origin position
    ///     of the instance created.
    /// </param>
    /// <param name="originY">
    ///     If the <see cref="FMOD.Studio.EventDescription"/> used to create the
    ///     instance is in 3D, this will set the y-coordinate origin position
    ///     of the instance created.
    /// </param>
    /// <returns>
    ///     The <see cref="FMOD.Studio.EventInstance"/> that is created.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if the calls to the underlying studio system returns a
    ///     <see cref="FMOD.RESULT"/> value that is anything except
    ///     <see cref="FMOD.RESULT.OK"/>.
    /// </exception>
    public FMOD.Studio.EventInstance PlaySoundEffect(string eventPath, float x, float y, float originX, float originY)
    {
        //  Create a new instance of the event
        FMOD.Studio.EventInstance instance = CreateInstance(eventPath, x, y, originX, originY);

        //  Start the instance (this start playback)
        ThrowIfNotOK(instance.start());

        ///  Per FMOD Documentation, Studio::EventInstance::release should be
        //  called immediately after Studio::EventInstance::start if we do
        //  not need to all it again later, which we don't for sound effects
        //  which are one shot plays
        //
        //  https://www.fmod.com/docs/2.02/api/studio-api-eventinstance.html#studio_eventinstance_start
        ThrowIfNotOK(instance.release());

        //  Return the instance
        return instance;
    }

    /// <summary>
    ///     Plays the <see cref="FMOD.Studio.EventInstance"/> at the specified
    ///     <paramref name="eventPath"/> as a one shot sound effect and sets
    ///     a paramter value of the instance with the <paramref name="param"/>
    ///     name given, to the <paramref name="value"/> specified before
    ///     staring the playback, and sets the 3D attributes of the instance.
    /// </summary>
    /// <param name="eventPath">
    ///     The event path for the <see cref="FMOD.Studio.EventDescription"/>
    ///     to retreive.  Event paths start with "event:/".  For example, if
    ///     retreiving the an event named "song" inside a folder called
    ///     "music", the event path woudl be "event:/music/song".
    /// </param>
    /// <param name="param">
    ///     The name of the paramter to set the value of.
    /// </param>
    /// <param name="value">
    ///     The value to set the parameter too.
    /// </param>
    /// <param name="x">
    ///     If the <see cref="FMOD.Studio.EventDescription"/> used to create the
    ///     instance is in 3D, this will set the x-coordinate position of the
    ///     instance created.
    /// </param>
    /// <param name="y">
    ///     If the <see cref="FMOD.Studio.EventDescription"/> used to create the
    ///     instance is in 3D, this will set the y-coordinate position of the
    ///     instance created.
    /// </param>
    /// <param name="originX">
    ///     If the <see cref="FMOD.Studio.EventDescription"/> used to create the
    ///     instance is in 3D, this will set the x-coordinate origin position
    ///     of the instance created.
    /// </param>
    /// <param name="originY">
    ///     If the <see cref="FMOD.Studio.EventDescription"/> used to create the
    ///     instance is in 3D, this will set the y-coordinate origin position
    ///     of the instance created.
    /// </param>
    /// <returns>
    ///     The <see cref="FMOD.Studio.EventInstance"/> that is created.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if the calls to the underlying studio system returns a
    ///     <see cref="FMOD.RESULT"/> value that is anything except
    ///     <see cref="FMOD.RESULT.OK"/>.
    /// </exception>
    public FMOD.Studio.EventInstance PlaySoundEffect(string eventPath, string parameter, float value, float x, float y, float originX, float originY)
    {
        //  Create a new instance of the event
        FMOD.Studio.EventInstance instance = CreateInstance(eventPath, x, y, originX, originY);

        //  Set the provided parameters
        SetEventInstanceParameter(instance, parameter, value);

        //  Start the instance (this start playback)
        ThrowIfNotOK(instance.start());

        ///  Per FMOD Documentation, Studio::EventInstance::release should be
        //  called immediately after Studio::EventInstance::start if we do
        //  not need to all it again later, which we don't for sound effects
        //  which are one shot plays
        //
        //  https://www.fmod.com/docs/2.02/api/studio-api-eventinstance.html#studio_eventinstance_start
        ThrowIfNotOK(instance.release());

        //  Return the instance
        return instance;
    }

    /// <summary>
    ///     Plays the <see cref="FMOD.Studio.EventInstance"/> at the specified
    ///     <paramref name="eventPath"/> as a one shot sound effect and sets
    ///     the parameters values specified before starting playback, and sets
    ///     the 3D attribute of the instance.
    /// </summary>
    /// <param name="eventPath">
    ///     The event path for the <see cref="FMOD.Studio.EventDescription"/>
    ///     to retreive.  Event paths start with "event:/".  For example, if
    ///     retreiving the an event named "song" inside a folder called
    ///     "music", the event path woudl be "event:/music/song".
    /// </param>
    /// <param name="parameters">
    ///     A <see cref="Dictionary{TKey, TValue}"/> where each element the
    ///     key is the name of a paramter and the value is the value to set
    ///     the parameter to.
    /// </param>
    /// <param name="x">
    ///     If the <see cref="FMOD.Studio.EventDescription"/> used to create the
    ///     instance is in 3D, this will set the x-coordinate position of the
    ///     instance created.
    /// </param>
    /// <param name="y">
    ///     If the <see cref="FMOD.Studio.EventDescription"/> used to create the
    ///     instance is in 3D, this will set the y-coordinate position of the
    ///     instance created.
    /// </param>
    /// <param name="originX">
    ///     If the <see cref="FMOD.Studio.EventDescription"/> used to create the
    ///     instance is in 3D, this will set the x-coordinate origin position
    ///     of the instance created.
    /// </param>
    /// <param name="originY">
    ///     If the <see cref="FMOD.Studio.EventDescription"/> used to create the
    ///     instance is in 3D, this will set the y-coordinate origin position
    ///     of the instance created.
    /// </param>
    /// <returns>
    ///     The <see cref="FMOD.Studio.EventInstance"/> that is created.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if the calls to the underlying studio system returns a
    ///     <see cref="FMOD.RESULT"/> value that is anything except
    ///     <see cref="FMOD.RESULT.OK"/>.
    /// </exception>
    public FMOD.Studio.EventInstance PlaySoundEffect(string eventPath, Dictionary<string, float> parameters, float x, float y, float originX, float originY)
    {
        //  Create a new instance of the event
        FMOD.Studio.EventInstance instance = CreateInstance(eventPath, x, y, originX, originY);

        //  Set the provided parameters
        SetEventInstanceParameters(instance, parameters);

        //  Start the instance (this start playback)
        ThrowIfNotOK(instance.start());

        ///  Per FMOD Documentation, Studio::EventInstance::release should be
        //  called immediately after Studio::EventInstance::start if we do
        //  not need to all it again later, which we don't for sound effects
        //  which are one shot plays
        //
        //  https://www.fmod.com/docs/2.02/api/studio-api-eventinstance.html#studio_eventinstance_start
        ThrowIfNotOK(instance.release());

        //  Return the instance
        return instance;
    }

    /// <summary>
    ///     Plays the music at the specified <paramref name="eventPath"/>.
    /// </summary>
    /// <param name="eventPath">
    ///     The event path for the <see cref="FMOD.Studio.EventDescription"/>
    ///     to retreive.  Event paths start with "event:/".  For example, if
    ///     retreiving the an event named "song" inside a folder called
    ///     "music", the event path woudl be "event:/music/song".
    /// </param>
    /// <param name="start">
    ///     Whether to immediatly start playback of the music after the instance
    ///     has been created.
    /// </param>
    /// <param name="fadeCurrent">
    ///     Whether to fade out any current music that is playing.  If
    ///     <see langword="false"/>, any current music is stopped immediatly
    ///     insted of faded.
    /// </param>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if the calls to the underlying studio system returns a
    ///     <see cref="FMOD.RESULT"/> value that is anything except
    ///     <see cref="FMOD.RESULT.OK"/>.
    /// </exception>
    public void PlayMusic(string eventPath, bool start = true, bool fadeCurrent = true)
    {
        //  If we are bein asked to play the music that is already playing,
        //  then just return back early.
        if (!string.IsNullOrEmpty(CurrentMusicPath) && CurrentMusicPath.Equals(eventPath))
        {
            return;
        }

        //  Stop the current music that is playing
        if (CurrentMusicEvent is not null)
        {
            Stop(CurrentMusicEvent.Value, fadeCurrent);
        }

        //  Create a new instance from the event path given.
        FMOD.Studio.EventInstance instance = CreateInstance(eventPath);

        //  If we were asked to immediatly start playback, then start it now.
        if (start)
        {
            ThrowIfNotOK(instance.start());
        }

        //  Set the properties
        CurrentMusicEvent = instance;
        CurrentMusicPath = eventPath;
    }

    /// <summary>
    ///     Stops the playback of the given
    ///     <see cref="FMOD.Studio.EventInstance"/> and immediatly releases it.
    /// </summary>
    /// <param name="instance">
    ///     The <see cref="FMOD.Studio.EventInstance"/> to stop the playback of.
    /// </param>
    /// <param name="fade">
    ///     When <see langword="true"/>,
    ///     <see cref="FMOD.Studio.STOP_MODE.ALLOWFADEOUT"/> is used when
    ///     stopping the instance; otherwise,
    ///     <see cref="FMOD.Studio.STOP_MODE.IMMEDIATE"/> is used.
    /// </param>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if the calls to the underlying studio system returns a
    ///     <see cref="FMOD.RESULT"/> value that is anything except
    ///     <see cref="FMOD.RESULT.OK"/>.
    /// </exception>
    public void Stop(FMOD.Studio.EventInstance instance, bool fade)
    {
        FMOD.Studio.STOP_MODE mode = fade ? FMOD.Studio.STOP_MODE.ALLOWFADEOUT : FMOD.Studio.STOP_MODE.IMMEDIATE;

        ThrowIfNotOK(instance.stop(mode));
        ThrowIfNotOK(instance.release());
    }

    /// <summary>
    ///     Sets the pause state of the given
    ///     <see cref="FMOD.Studio.EventInstance"/>.
    /// </summary>
    /// <param name="instance">
    ///     The <see cref="FMOD.Studio.EventInstance"/> to set the puase state
    ///     of.
    /// </param>
    /// <param name="pause">
    ///     <see langword="true"/> if it shoudl be paused; otherwise,
    ///     <see langword="false"/>.
    /// </param>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if the calls to the underlying studio system returns a
    ///     <see cref="FMOD.RESULT"/> value that is anything except
    ///     <see cref="FMOD.RESULT.OK"/>.
    /// </exception>
    public void SetPause(FMOD.Studio.EventInstance instance, bool pause) => ThrowIfNotOK(instance.setPaused(pause));

    /// <summary>
    ///     Retreives a value that indicates if the
    ///     <see cref="FMOD.Studio.EventInstance"/> given is currently
    ///     playing.
    /// </summary>
    /// <remarks>
    ///     A Studio::EventInstance is condered "playing" if when it's current
    ///     playback state is either PLAYING or STARTING.
    /// </remarks>
    /// <param name="instance">
    ///     The <see cref="FMOD.Studio.EventInstance"/> to check.
    /// </param>
    /// <returns>
    ///     <see cref="true"/> if the given
    ///     <see cref="FMOD.Studio.EventInstance"/> is currently playing;
    ///     otherwise, <see cref="false"/>.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if the calls to the underlying studio system returns a
    ///     <see cref="FMOD.RESULT"/> value that is anything except
    ///     <see cref="FMOD.RESULT.OK"/>.
    /// </exception>
    public bool IsPlaying(FMOD.Studio.EventInstance instance)
    {
        //  Get hte playback state of the instance
        ThrowIfNotOK(instance.getPlaybackState(out FMOD.Studio.PLAYBACK_STATE state));

        //  If the state is "playing" or if the state is "starting" then
        //  it is considered playing
        if (state == FMOD.Studio.PLAYBACK_STATE.PLAYING || state == FMOD.Studio.PLAYBACK_STATE.STARTING)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    ///     Gets the <see cref="FMOD.Studio.VCA"/> at the specified
    ///     <paramref name="vcaPath"/>.
    /// </summary>
    /// <param name="vcaPath">
    ///     The vca path for the <see cref="FMOD.Studio.VCA"/> to retrieve.
    ///     VCA paths start with "vca:/".  For example, if retreiving a VCA
    ///     named "music", the path would be "vca:/music".
    /// </param>
    /// <returns>
    ///     The <see cref="FMOD.Studio.VCA"/> from the Studio System at the
    ///     <paramref name="vcaPath"/> given.
    /// </returns>
    // <exception cref="InvalidOperationException">
    ///     Thrown if the calls to the underlying studio system returns a
    ///     <see cref="FMOD.RESULT"/> value that is anything except
    ///     <see cref="FMOD.RESULT.OK"/>.
    /// </exception>
    public FMOD.Studio.VCA GetVCA(string vcaPath)
    {
        ThrowIfNotOK(StudioSystem.getVCA(vcaPath, out FMOD.Studio.VCA vca));
        return vca;
    }

    /// <summary>
    ///     Sets the volume of the <see cref="FMOD.Studio.VCA"/> at the
    ///     specified <paramref name="vcaPath"/>.
    /// </summary>
    /// <param name="vcaPath">
    ///     The vca path for the <see cref="FMOD.Studio.VCA"/> to retrieve.
    ///     VCA paths start with "vca:/".  For example, if retreiving a VCA
    ///     named "music", the path would be "vca:/music".
    /// </param>
    /// <param name="volume">
    ///     The volume value to set.
    /// </param>
    public void SetVCAVolume(string vcaPath, float volume) => ThrowIfNotOK(GetVCA(vcaPath).setVolume(volume));


    /// <summary>
    ///     Gets the volume of the <see cref="FMOD.Studio.VCA"/> at the
    ///     specified <paramref name="vcaPath"/>.
    /// </summary>
    /// <param name="vcaPath">
    ///     The vca path for the <see cref="FMOD.Studio.VCA"/> to retrieve.
    ///     VCA paths start with "vca:/".  For example, if retreiving a VCA
    ///     named "music", the path would be "vca:/music".
    /// </param>
    /// <returns>
    ///     The volume value.
    /// </returns>
    public float GetVCAVolume(string vcaPath)
    {
        ThrowIfNotOK(GetVCA(vcaPath).getVolume(out float volume));
        return volume;
    }

    /// <summary>
    ///     Gets the <see cref="FMOD.Studio.Bus"/> at the specified
    ///     <paramref name="busPath"/>.
    /// </summary>
    /// <param name="busPath">
    ///     The bus path for the <see cref="FMOD.Studio.Bus"/> to retrieve.
    ///     Bus paths start with "bus:/".  For example, if retreiving a bus
    ///     named "music", the path would be "bus:/music".
    /// </param>
    /// <returns>
    ///     The <see cref="FMOD.Studio.Bus"/> from the Studio System at the
    ///     <paramref name="busPath"/> given.
    /// </returns>
    // <exception cref="InvalidOperationException">
    ///     Thrown if the calls to the underlying studio system returns a
    ///     <see cref="FMOD.RESULT"/> value that is anything except
    ///     <see cref="FMOD.RESULT.OK"/>.
    /// </exception>
    public FMOD.Studio.Bus GetBus(string busPath)
    {
        ThrowIfNotOK(StudioSystem.getBus(busPath, out FMOD.Studio.Bus bus));
        return bus;
    }

    /// <summary>
    ///     Pauses or unpauses all audio routed to the
    ///     <see cref="FMOD.Studio.Bus"/> at the specified
    ///     <paramref name="busPath"/>.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Per FMOD Documentation at
    ///         <see href="https://www.fmod.com/docs/2.00/api/studio-api-bus.html#studio_bus_setpaused">
    ///         https://www.fmod.com/docs/2.00/api/studio-api-bus.html#studio_bus_setpaused
    ///         </see>
    ///     </para>
    ///     <para>
    ///         An individual pause state is kept for each bus.  Pausing a bus
    ///         will override the pause state of its inputs (meaning they return
    ///         <see langword="true"/> from Studio::Bus::getPaused), while
    ///         unpausing a bus will cause all inputs to obey their individual
    ///         pause states. The pause state is processed in the Studio system
    ///         update, so Studio::Bus::getPaused will return the state as
    ///         determined by the last update.
    ///     </para>
    /// </remarks>
    /// <param name="busPath">
    ///     The bus path for the <see cref="FMOD.Studio.Bus"/> to retrieve.
    ///     Bus paths start with "bus:/".  For example, if retreiving a bus
    ///     named "music", the path would be "bus:/music".
    /// </param>
    /// <param name="paused">
    ///     Whether to pause (<see langword="true"/>) or unpause
    ///     (<see langword="false"/>) the bus.
    /// </param>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if the calls to the underlying studio system returns a
    ///     <see cref="FMOD.RESULT"/> value that is anything except
    ///     <see cref="FMOD.RESULT.OK"/>.
    /// </exception>
    public void SetBusPause(string busPath, bool paused) => ThrowIfNotOK(GetBus(busPath).setPaused(paused));

    /// <summary>
    ///     Returns a value that indicates whether the
    ///     <see cref="FMOD.Studio.Bus"/> at the specified
    ///     <paramref name="busPath"/> is paused.
    /// </summary>
    /// <param name="busPath">
    ///     The bus path for the <see cref="FMOD.Studio.Bus"/> to retrieve.
    ///     Bus paths start with "bus:/".  For example, if retreiving a bus
    ///     named "music", the path would be "bus:/music".
    /// </param>
    /// <returns>
    ///     <see langword="true"/> if the <see cref="FMOD.Studio.Bus"/> at the
    ///     given <paramref name="busPath"/> is paused; otherwise,
    ///     <see langword="false"/>.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if the calls to the underlying studio system returns a
    ///     <see cref="FMOD.RESULT"/> value that is anything except
    ///     <see cref="FMOD.RESULT.OK"/>.
    /// </exception>
    public bool IsBusPaused(string busPath)
    {
        ThrowIfNotOK(GetBus(busPath).getPaused(out bool paused));
        return paused;
    }

    /// <summary>
    ///     Returns the volume of the <see cref="FMOD.Studio.Bus"/> at the
    ///     specified <paramref name="busPath"/>.
    /// </summary>
    /// <param name="busPath">
    ///     The bus path for the <see cref="FMOD.Studio.Bus"/> to retrieve.
    ///     Bus paths start with "bus:/".  For example, if retreiving a bus
    ///     named "music", the path would be "bus:/music".
    /// </param>
    /// <returns>
    ///     The volume of the <see cref="FMOD.Studio.Bus"/> fromt he given
    ///     <paramref name="busPath"/>.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if the calls to the underlying studio system returns a
    ///     <see cref="FMOD.RESULT"/> value that is anything except
    ///     <see cref="FMOD.RESULT.OK"/>.
    /// </exception>
    public float GetBusVolume(string busPath)
    {
        ThrowIfNotOK(GetBus(busPath).getVolume(out float volume));
        return volume;
    }

    /// <summary>
    ///     Sets the volume of the <see cref="FMOD.Studio.Bus"/> at the
    ///     specified path to the <paramref name="volume"/> given.
    /// </summary>
    /// <param name="busPath">
    ///     The bus path for the <see cref="FMOD.Studio.Bus"/> to retrieve.
    ///     Bus paths start with "bus:/".  For example, if retreiving a bus
    ///     named "music", the path would be "bus:/music".
    /// </param>
    /// <param name="volume">
    ///     The volume to set the bus too.
    /// </param>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if the calls to the underlying studio system returns a
    ///     <see cref="FMOD.RESULT"/> value that is anything except
    ///     <see cref="FMOD.RESULT.OK"/>.
    /// </exception>
    public void SetBusVolume(string busPath, float volume) => ThrowIfNotOK(GetBus(busPath).setVolume(volume));

    /// <summary>
    ///     Mutes or unmutes the <see cref="FMOD.Studio.Bus"/> at the specified
    ///     <paramref name="busPath"/>.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Per FMOD Documentation at
    ///         <see href="https://www.fmod.com/docs/2.00/api/studio-api-bus.html#studio_bus_setmute"/>
    ///         https://www.fmod.com/docs/2.00/api/studio-api-bus.html#studio_bus_setmute
    ///     </para>
    ///     <para>
    ///         Mute is an additional control for volume, the effect of which is
    ///         equivalent to setting the volume to zero.
    ///     </para>
    ///     <para>
    ///         An individual mute state is kept for each bus.  Muting a bus
    ///         will override the mute state of its inputs (meaning they return
    ///         <see langword="true"/> from Studio::Bus::getMute), while
    ///         unmuting a bus will cause its inputs to obey their individual
    ///         mute state.  The mute state is processed in the Studio system
    ///         update, so Studio::Bus::getMute will return the state as
    ///         determined by the last update.
    ///     </para>
    /// </remarks>
    /// <param name="busPath">
    ///     The bus path for the <see cref="FMOD.Studio.Bus"/> to retrieve.
    ///     Bus paths start with "bus:/".  For example, if retreiving a bus
    ///     named "music", the path would be "bus:/music".
    /// </param>
    /// <param name="mute">
    ///     Whether to mute (<see langword="true"/>) or unmute
    ///     (<see langword="false"/>) the bus.
    /// </param>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if the calls to the underlying studio system returns a
    ///     <see cref="FMOD.RESULT"/> value that is anything except
    ///     <see cref="FMOD.RESULT.OK"/>.
    /// </exception>
    public void SetBusMute(string busPath, bool mute) => ThrowIfNotOK(GetBus(busPath).setMute(mute));

    /// <summary>
    ///     Returns a value that indicates whether the
    ///     <see cref="FMOD.Studio.Bus"/> at the specified
    ///     <paramref name="busPath"/> is muted.
    /// </summary>
    /// <param name="busPath">
    ///     The bus path for the <see cref="FMOD.Studio.Bus"/> to retrieve.
    ///     Bus paths start with "bus:/".  For example, if retreiving a bus
    ///     named "music", the path would be "bus:/music".
    /// </param>
    /// <returns>
    ///     <see langword="true"/> if the <see cref="FMOD.Studio.Bus"/> at the
    ///     given <paramref name="busPath"/> is muted; otherwise,
    ///     <see langword="false"/>.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if the calls to the underlying studio system returns a
    ///     <see cref="FMOD.RESULT"/> value that is anything except
    ///     <see cref="FMOD.RESULT.OK"/>.
    /// </exception>
    public bool IsBusMuted(string busPath)
    {
        ThrowIfNotOK(GetBus(busPath).getMute(out bool muted));
        return muted;
    }

    /// <summary>
    ///     Stops all <see cref="FMOD.Studio.EventInstance"/> elements that are
    ///     routed into the <see cref="FMOD.Studio.Bus"/> at the specified
    ///     <paramref name="busPath"/>.
    /// </summary>
    /// <param name="busPath">
    ///     The bus path for the <see cref="FMOD.Studio.Bus"/> to retrieve.
    ///     Bus paths start with "bus:/".  For example, if retreiving a bus
    ///     named "music", the path would be "bus:/music".
    /// </param>
    /// <param name="fade">
    ///     Specifies whether the event instances should fade out
    ///     (<see langword="true"/>) or stop immediatly
    ///     (<see langword="false"/>).
    /// </param>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if the calls to the underlying studio system returns a
    ///     <see cref="FMOD.RESULT"/> value that is anything except
    ///     <see cref="FMOD.RESULT.OK"/>.
    /// </exception>
    public void StopAllEventsForBus(string busPath, bool fade)
    {
        FMOD.Studio.STOP_MODE mode = fade ? FMOD.Studio.STOP_MODE.ALLOWFADEOUT : FMOD.Studio.STOP_MODE.IMMEDIATE;
        ThrowIfNotOK(GetBus(busPath).stopAllEvents(mode));
    }

    /// <summary>
    ///     Releases all <see cref="FMOD.Studio.EventDescription"/> instances
    ///     that have no active <see cref="FMOD.Studio.EventInstance"/>
    ///     instances active.
    /// </summary>
    public void ReleaseUnusedEventDescriptions()
    {
        //  We can't remove items from the dictionary while we also iterate the
        //  dictionary, so we'll use this list as a temporary holder for all
        //  event descriptions that have no active instances
        List<string> unused = new();

        //  Go through all event descriptions in the cache dictionary...
        foreach (KeyValuePair<string, FMOD.Studio.EventDescription> kvp in _events)
        {
            //  ... Get the number of instances of that description
            ThrowIfNotOK(kvp.Value.getInstanceCount(out int count));

            //  If there are no active instanes of that description, add the key
            //  to the collection of unused ones
            if (count <= 0)
            {
                unused.Add(kvp.Key);

                //  Adding this not here since I was originally calling
                //  Studio::EventInstance::clearHandle() here
                //  The handle of the event description does not have to be
                //  released here.  The memory has already been released since
                //  there are no more instances, and called
                //  Studio::EventInstance::clearHandle just nulls the pointer
                //
                //  https://qa.fmod.com/t/undocumented-methods-eventinstance-clearhandle-eventinstance-isvalid/14419/2

            }
        }

        //  Now that we have all the unused ones, we can remove them from the
        //  cache dictionary
        for (int i = 0; i < unused.Count; i++)
        {
            _events.Remove(unused[i]);
        }
    }

    /// <summary>
    ///     Releases resources held by this instance.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    ///     Releases resources held by this instance.
    /// </summary>
    /// <param name="disposeManaged">
    ///     Whether managed resources should also be disposed of.
    /// </param>
    private void Dispose(bool disposeManaged)
    {
        //  Return early if already disposed.
        if (IsDisposed) { return; }

        //  This will unload all currently loaded banks
        ThrowIfNotOK(StudioSystem.release());

        if (disposeManaged)
        {
            //  Clear the banks dictionary
            _cachedBanks.Clear();

            //  Clear the events dictionary
            _events.Clear();
        }

        IsDisposed = true;
    }

    /// <summary>
    ///     Given an <see cref="FMOD.RESULT"/> value, will throw an
    ///     <see cref="InvalidOperationException"/> if the value is anything
    ///     except <see cref="FMOD.RESULT.OK"/>.
    /// </summary>
    /// <param name="result">
    ///     The <see cref="FMOD.RESULT"/> value to check.
    /// </param>
    /// <exception cref="InvalidOperationException">
    ///     Throw if the <paramref name="result"/> value is anything except
    ///     <see cref="FMOD.RESULT.OK"/>
    /// </exception>
    public static void ThrowIfNotOK(FMOD.RESULT result)
    {
        if (result != FMOD.RESULT.OK)
        {
            throw new InvalidOperationException(FMOD.Error.String(result));
        }
    }
}
