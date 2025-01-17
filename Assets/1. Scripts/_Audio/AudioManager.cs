using UnityEngine;
using FMOD.Studio;
using FMODUnity;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance { get; private set; }

    [Header("Depuración")]
    public bool mostrarLogMusic;
    public bool mostrarLogSFX;

    [Header("Ubicación de Reproducción")]
    public Transform jugadorPos;

    private Dictionary<EventInstance, Transform> eventosActivos = new Dictionary<EventInstance, Transform>();

    private List<StudioEventEmitter> eventEmitters;

    private EventInstance ambienceEventInstance;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Hay más de un AudioManager en la escena");
            return;
        }

        instance = this;

        eventEmitters = new List<StudioEventEmitter>();

    }

    private void Start()
    {
        InitializeAmbienceEvent(FMODEvents.instance.ambiente);
    }

    private void Update()
    {
        foreach (var pair in eventosActivos)
        {
            EventInstance evento = pair.Key;
            Transform objeto = pair.Value;

            if (objeto != null)
            {
                evento.set3DAttributes(RuntimeUtils.To3DAttributes(objeto.position));
            }
        }
    }

    #region Reproducir Eventos de una sola vez "Play One Shot"

    public void PlayOneShot(EventReference sound, Vector3? worldPos = null)
    {
        Vector3 finalPos = worldPos ?? (jugadorPos != null ? jugadorPos.position : Vector3.zero);

        if (worldPos == null && jugadorPos == null)
        {
            Debug.LogWarning("No se especificó una posición y 'jugadorPos' no está asignado. Usando Vector3.zero.");
        }

        RuntimeManager.PlayOneShot(sound, finalPos);

        if (mostrarLogSFX && sound.Path.Contains("SFX"))
        {
            Debug.Log($"Reproduciendo evento SFX: {sound.Path} en posición: {finalPos}");
        }

        if (mostrarLogMusic && sound.Path.Contains("Music"))
        {
            Debug.Log($"Reproduciendo evento Music: {sound.Path} en posición: {finalPos}");
        }
    }

    #endregion

    #region Reproducir Eventos Timeline 3D - Tipo Pasos

    public EventInstance PlayEventInstance(EventReference eventReference, Transform followTransform = null)
    {
        followTransform ??= jugadorPos;
        EventInstance eventInstance = CreateEventInstance(eventReference, followTransform);
        StartEvent(eventInstance);
        return eventInstance;
    }

    public EventInstance CreateEventInstance(EventReference eventReference, Transform followTransform = null)
    {
        followTransform ??= jugadorPos;
        EventInstance eventInstance = RuntimeManager.CreateInstance(eventReference);

        if (followTransform != null)
        {
            eventInstance.set3DAttributes(RuntimeUtils.To3DAttributes(followTransform.position));
            eventosActivos[eventInstance] = followTransform;
        }
        else
        {
            Debug.LogWarning("No se especificó un followTransform y jugadorPos no está asignado.");
        }

        return eventInstance;
    }

    public void StartEvent(EventInstance eventInstance)
    {
        PLAYBACK_STATE playbackState;
        eventInstance.getPlaybackState(out playbackState);

        if (playbackState == PLAYBACK_STATE.STOPPED)
        {
            eventInstance.start();
            if (mostrarLogSFX) Debug.Log("Evento iniciado.");
        }
    }

    public void StopEvent(EventInstance eventInstance, bool allowFadeOut = true)
    {
        if (eventosActivos.ContainsKey(eventInstance))
        {
            eventosActivos.Remove(eventInstance);
        }

        eventInstance.stop(allowFadeOut ? FMOD.Studio.STOP_MODE.ALLOWFADEOUT : FMOD.Studio.STOP_MODE.IMMEDIATE);
        ReleaseEvent(eventInstance);
    }

    public void ReleaseEvent(EventInstance eventInstance)
    {
        if (eventosActivos.ContainsKey(eventInstance))
        {
            eventosActivos.Remove(eventInstance);
        }

        eventInstance.release();
        if (mostrarLogSFX) Debug.Log("Evento liberado.");
    }

    #endregion

    public StudioEventEmitter InitializeEventEmitter(EventReference eventReference, GameObject emitterGameObject)
    { 
        StudioEventEmitter emitter = emitterGameObject.GetComponent<StudioEventEmitter>();
        emitter.EventReference = eventReference;
        eventEmitters.Add(emitter);
        
        emitter.Play();
        return emitter;
    }

    private void InitializeAmbienceEvent(EventReference ambienceEventReference)
    {
        ambienceEventInstance = CreateEventInstance(ambienceEventReference);
        ambienceEventInstance.start();
    
    }

    public void SetAmbienceParameter(string parameterName, float parameterValue)
    { 
        ambienceEventInstance.setParameterByName(parameterName, parameterValue);
    }


    public void CleanUpEmitters()
    { 
        foreach (StudioEventEmitter emitter in eventEmitters)
        {
            emitter.Stop();            
            Debug.Log(emitter);
        }
    }

}
