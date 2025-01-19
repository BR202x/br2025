using UnityEngine;
using FMOD.Studio;
using FMODUnity;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    [Header("Depuración")]
    public bool mostrarLogMusic;
    public bool mostrarLogSFX;

    [Header("Ubicación de Reproducción por defecto")]
    [Tooltip("Si en las instancias no se especifica la ubicacion de reproducccion, sera sobre este Transform")]
    public Transform jugadorPos;

    #region Volumen

    [Header("Volume")]
    [Range(0, 1)]
    public float masterVolume = 1;
    [Range(0, 1)]
    public float musicVolume = 1;
    [Range(0, 1)]
    public float SFXVolume = 1;

    private Bus masterBus;
    private Bus musicBus;
    private Bus SFXBus;

    #endregion

    public static AudioManager instance { get; private set; }

        public Dictionary<EventInstance, Transform> eventosActivos = new Dictionary<EventInstance, Transform>();

        public List<StudioEventEmitter> eventEmitters;

        public EventInstance ambienceEventInstance;

        public EventInstance musicEventInstance;



    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Hay más de un AudioManager en la escena");
            return;
        }

        instance = this;

        masterBus = RuntimeManager.GetBus("bus:/");
        musicBus = RuntimeManager.GetBus("bus:/Musica");
        SFXBus = RuntimeManager.GetBus("bus:/SFX");

        eventEmitters = new List<StudioEventEmitter>();
    }

    private void Start()
    {
        //InitializeAmbienceEvent(FMODEvents.instance.ambiente);
        //InitializeMusic(FMODEvents.instance.musica);
    }

    private void Update()
    {
        masterBus.setVolume(masterVolume);
        musicBus.setVolume(musicVolume);
        SFXBus.setVolume(SFXVolume);


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

    // --- CREACION DE EVENTOS

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

    public EventInstance CreateEventAmbientInstance(EventReference eventReference, Transform followTransform)
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

    // ----------------------

    #region Reproducir Eventos Timeline 3D - Tipo Pasos

    public void HandleEvent(EventReference eventReference, Transform followTransform = null, bool start = true, bool allowFadeOut = true)
    {
        followTransform ??= jugadorPos;

        // Si el booleano `start` es verdadero, crear e iniciar el evento.
        if (start)
        {
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

            PLAYBACK_STATE playbackState;
            eventInstance.getPlaybackState(out playbackState);

            if (playbackState == PLAYBACK_STATE.STOPPED)
            {
                eventInstance.start();
                if (mostrarLogSFX) Debug.Log("Evento iniciado.");
            }
        }
        else
        {
            foreach (var evento in eventosActivos)
            {
                if (evento.Key.isValid())
                {
                    evento.Key.stop(allowFadeOut ? FMOD.Studio.STOP_MODE.ALLOWFADEOUT : FMOD.Studio.STOP_MODE.IMMEDIATE);
                    eventosActivos.Remove(evento.Key);
                    ReleaseEvent(evento.Key);
                    if (mostrarLogSFX) Debug.Log("Evento detenido.");
                    return;
                }
            }

            Debug.LogWarning("No se encontró ninguna instancia activa para el evento.");
        }
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

    #region Reproducir Emisores de Sonido

    public StudioEventEmitter InitializeEventEmitter(EventReference eventReference, GameObject emitterGameObject)
    { 
        StudioEventEmitter emitter = emitterGameObject.GetComponent<StudioEventEmitter>();
        emitter.EventReference = eventReference;
        eventEmitters.Add(emitter);
        
        emitter.Play();
        return emitter;
    }

    public void CleanUpEmitters()
    {
        foreach (StudioEventEmitter emitter in eventEmitters)
        {
            emitter.Stop();
            Debug.Log(emitter);
        }
    }

    #endregion

    #region Reproducir Ambiente y Cambios en el ambiente
    public void InitializeAmbienceEvent(EventReference ambienceEventReference, Transform transform)
    {
        ambienceEventInstance = CreateEventAmbientInstance(ambienceEventReference, transform);
        ambienceEventInstance.start();    
    }

    public void SetAmbienceParameter(string parameterName, float parameterValue)
    { 
        ambienceEventInstance.setParameterByName(parameterName, parameterValue);
    }

    #endregion

    #region Reproducir Musica

    public void SetMusicArea(MusicArea area)
    {
        musicEventInstance.setParameterByName("area", (float) area);
    }

    public void InitializeMusic(EventReference musicEventReference)
    {
        musicEventInstance = CreateEventInstance(musicEventReference);
        musicEventInstance.start();
        
    }

    #endregion

}
