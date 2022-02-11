using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD.Studio;
using FMODUnity;

/// <summary>
/// Handles all the audio & music in the game
/// Used as a singleton because the script exists in multiple scenes
/// and the first instance should continue when transitioning
/// </summary>
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [SerializeField] private Transform player;
    [SerializeField] private StudioEventEmitter emitter;
    [SerializeField] private AnimationCurve speedCurve;
    [SerializeField] private bool groundCollisionCooldown = false;
    [SerializeField] [Range(0.01f, 0.2f)] float groundCollisionInterval = 0.1f;
    
    private Bus soundBus;
    private Bus musicBus;
    private PARAMETER_ID speedParameter;

    private bool isSucking = false;
    private float saxCooldown = 1f;
    private bool onSaxCooldown = false;
    private int currentSoundVolume = 10; // scale 1-10 for main menu
    private int currentMusicVolume = 10;

    #region FMOD events
    private List<EventInstance> collisionEvents = new List<EventInstance>();
    private EventInstance suckingEvent;
    private EventInstance successEvent;
    private EventInstance shootingEvent;
    private EventInstance absorbEvent;
    private EventInstance musicEvent;
    private EventInstance screenFXEvent;
    private static EventInstance warningSoundEvent;

    private const string suckingEventPath = "event:/MachineSucking";
    private const string successEventPath = "event:/WashedLaundry";
    private const string shootingEventPath = "event:/Shooting";
    private const string collisionEventPath = "event:/MachineCollision";
    private const string absorbEventPath = "event:/Absorb";
    private const string musicEventPath = "event:/Music";
    private const string screenFXEventPath = "event:/ScreenEffect";
    private const string warningSoundEventPath = "event:/WarningSound";
    private const string notificationEventPath = "event:/NotificationSound";
    private const string winningEventPath = "event:/WinningSound";
    #endregion

    #region Initializing references
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Initialize();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        musicEvent.start();
        RuntimeManager.StudioSystem.getParameterDescriptionByName("Speed", out PARAMETER_DESCRIPTION speedParameterDescription);

        speedParameter = speedParameterDescription.id;
        DontDestroyOnLoad(gameObject);
    }
    private void OnEnable()
    {
        DestructibleObject.objectDestroyedEvent += RaiseDestructionParameter;
    }
    private void Initialize()
    {
        suckingEvent = RuntimeManager.CreateInstance(suckingEventPath);
        screenFXEvent = RuntimeManager.CreateInstance(screenFXEventPath);
        successEvent = RuntimeManager.CreateInstance(successEventPath);
        shootingEvent = RuntimeManager.CreateInstance(shootingEventPath);
        absorbEvent = RuntimeManager.CreateInstance(absorbEventPath);
        musicEvent = RuntimeManager.CreateInstance(musicEventPath);
        warningSoundEvent = RuntimeManager.CreateInstance(warningSoundEventPath);

        soundBus = RuntimeManager.GetBus("bus:/Sound");
        musicBus = RuntimeManager.GetBus("bus:/MusicGroup");

        if (player == null) player = GameObject.Find("Washing machine")?.transform;
        if (emitter == null && player != null) emitter = player.GetComponentInChildren<StudioEventEmitter>();
    }
    public void InitializePlayer(Transform playerTransform)
    {
        player = playerTransform;
        emitter = player.GetComponentInChildren<StudioEventEmitter>();

        suckingEvent.set3DAttributes(RuntimeUtils.To3DAttributes(player));
        shootingEvent.set3DAttributes(RuntimeUtils.To3DAttributes(player));
        absorbEvent.set3DAttributes(RuntimeUtils.To3DAttributes(player));
    }
    #endregion

    #region Volume settings
    public int GetScaledSoundVolume()
    {
        return currentSoundVolume;
    }
    public int GetScaledMusicVolume()
    {
        return currentMusicVolume;
    }

    public void SetSoundVolume(int scaledVolume)
    {
        currentSoundVolume = scaledVolume;
        float newVolume = Mathf.Clamp(scaledVolume * 0.1f, 0, 1);
        soundBus.setVolume(newVolume);
    }
    public void SetMusicVolume(int scaledVolume)
    {
        currentMusicVolume = scaledVolume;
        float newVolume = Mathf.Clamp(scaledVolume * 0.1f, 0, 1);
        musicBus.setVolume(newVolume);
    }
    #endregion

    #region Set FMOD parameters
    public void StartGameplayMusic()
    {
        musicEvent.setParameterByName("GameplayStarted", 1);
    }
    public void UpdateSpeed(float speed)
    {
        float curvedSpeed = speedCurve.Evaluate(speed);
        RuntimeManager.StudioSystem.setParameterByID(speedParameter, curvedSpeed);
    }
    private void RaiseDestructionParameter()
    {
        RuntimeManager.StudioSystem.setParameterByName("Destruction", 1);
    }
    #endregion

    #region Play / start events 
    public void PlayCollisionSound(float velocity, Vector3 position, bool isGroundCollision = false)
    {
        if (isGroundCollision && groundCollisionCooldown) return;

        foreach (EventInstance events in collisionEvents)
        {
            events.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

        }
        collisionEvents.Clear();

        int roundedVelocity = velocity >= 0.8f ? 100 :
                              velocity >= 0.6f ? 80 :
                              velocity >= 0.4f ? 60 :
                              velocity >= 0.2f ? 40 : 20;

        string path = collisionEventPath + roundedVelocity.ToString();
        EventInstance collisionEvent = RuntimeManager.CreateInstance(path);
        collisionEvent.setParameterByName("Velocity", velocity /** 100*/);

        collisionEvent.set3DAttributes(RuntimeUtils.To3DAttributes(position));
        collisionEvent.start();

        collisionEvents.Add(collisionEvent);

        if (isGroundCollision)
        {
            StartCoroutine(StartCollisionCooldown());
        }
    }
    public void PlayScreenSoundEffect()
    {
        screenFXEvent.start();
    }
    public void PlaySuckingSound(GameObject player)
    {
        isSucking = true;
        suckingEvent.set3DAttributes(RuntimeUtils.To3DAttributes(player.transform.position));
        suckingEvent.start();

        StartCoroutine(Update3DAttributesWhileSucking());
    }
    public void StopPlayingSuckingSound()
    {
        isSucking = false;
        suckingEvent.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

    public void PlaySuccessSound()
    {
        successEvent.start();

        if (!onSaxCooldown)
        {
            musicEvent.setParameterByName("Chaos", 1);
            RuntimeManager.StudioSystem.setParameterByName("ChaosTimer", 1.3f);
            musicEvent.setParameterByName("DuringSuccess", 1);
            StartCoroutine(SaxCooldownRoutine());
        }
    }
    public void PlayShootingSound(Vector3 position)
    {
        shootingEvent.set3DAttributes(RuntimeUtils.To3DAttributes(position));
        shootingEvent.start();
    }
    public void PlayAbsorbSound(Vector3 position)
    {
        absorbEvent.set3DAttributes(RuntimeUtils.To3DAttributes(position));
        absorbEvent.start();
    }

    public static void PlayWinningSound()
    {
        RuntimeManager.PlayOneShot(winningEventPath);
    }

    public static void PlayWarningSound()
    {
        warningSoundEvent.start();
    }
    public static void PlayNotificationSound()
    {
        RuntimeManager.PlayOneShot(notificationEventPath);
    }
    #endregion

    #region Coroutines
    private IEnumerator Update3DAttributesWhileSucking()
    {
        while (isSucking)
        {
            suckingEvent.set3DAttributes(RuntimeUtils.To3DAttributes(player.position));
            yield return new WaitForSeconds(0.05f);
        }
    }
    private IEnumerator StartCollisionCooldown()
    {
        groundCollisionCooldown = true;

        yield return new WaitForSeconds(groundCollisionInterval);
        groundCollisionCooldown = false;
    }
    private IEnumerator SaxCooldownRoutine()
    {
        onSaxCooldown = true;
        yield return new WaitForSeconds(saxCooldown);
        onSaxCooldown = false;
    }
    #endregion

    #region collision sound system backup
    //public static void PlayWoodCollisionSound(SoundType soundType, float velocity, Vector3 position)
    //{
    //    string eventPath = "event:/ObjectCollision/";
    //    eventPath += soundType == SoundType.dark ? "LowWood" :
    //                 soundType == SoundType.mid ? "MidWood" :
    //                 soundType == SoundType.bright ? "BrightWood" : "BrighterWood";
    //
    //    EventInstance collisionEvent = RuntimeManager.CreateInstance(eventPath);
    //    collisionEvent.setParameterByName("Velocity", velocity);
    //    collisionEvent.set3DAttributes(RuntimeUtils.To3DAttributes(position));
    //    collisionEvent.start();
    //    collisionEvent.getParameterByName("Velocity", out float velocity2);
    //    Debug.Log(velocity2);
    //}
    #endregion

    private void OnDisable()
    {
        DestructibleObject.objectDestroyedEvent -= RaiseDestructionParameter;
    }
}
