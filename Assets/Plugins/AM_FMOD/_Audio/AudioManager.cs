using UnityEngine;
using FMOD.Studio;
using FMODUnity;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    [Header("Depuración")]
    public bool mostrarLogMusic = false;
    public bool mostrarLogSFX = false;

    [Header("Ubicación de Reproducción por defecto")]
    [Tooltip("Si en las instancias no se especifica la ubicacion de reproducccion, sera sobre este Transform")]
    public GameObject jugador;

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

        public List<EventInstance> ambienceEvents = new List<EventInstance>();

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

    #region Reproducir Musica
    public void InitializeMusic(EventReference musicEventReference, bool reproducir)
    {
        if (reproducir)
        {
            if (musicEventInstance.isValid())
            {
                CleanUpMusic(musicEventInstance, musicEventReference); // Limpiar instancia previa si coincide la referencia
            }

            musicEventInstance = CreateEventInstance(musicEventReference);

            if (musicEventInstance.isValid())
            {
                musicEventInstance.start();
                if (mostrarLogMusic) Debug.Log($"Música iniciada: {musicEventReference.Path}.");
            }
        }
        else
        {
            if (musicEventInstance.isValid())
            {
                CleanUpMusic(musicEventInstance, musicEventReference);
            }
        }
    }

    // --- CREACION DE EVENTOS
    public EventInstance CreateEventInstance(EventReference eventReference, Transform followTransform = null)
    {
        EventInstance eventInstance = RuntimeManager.CreateInstance(eventReference);

        // Verificar si followTransform es nulo
        if (followTransform != null)
        {
            // Asignar atributos 3D al evento usando followTransform
            eventInstance.set3DAttributes(RuntimeUtils.To3DAttributes(followTransform.position));
            eventosActivos[eventInstance] = followTransform;
        }
        else if (jugador != null)
        {
            // Usar la posición del jugador si followTransform es nulo
            eventInstance.set3DAttributes(RuntimeUtils.To3DAttributes(jugador.transform.position));
            eventosActivos[eventInstance] = jugador.transform;
        }
        else
        {
            // Si jugador también es nulo, usar Vector3.zero
            eventInstance.set3DAttributes(RuntimeUtils.To3DAttributes(Vector3.zero));
            Debug.LogWarning("No se especificó followTransform ni jugador. Usando Vector3.zero como posición predeterminada.");
        }

        return eventInstance;
    }

    public void CleanUpMusic(EventInstance eventInstance, EventReference targetReference)
    {
        if (eventInstance.isValid())
        {
            // Obtener la descripción del evento para comparar con la referencia
            eventInstance.getDescription(out var description);
            description.getPath(out string eventPath);

            // Comparar el path del evento activo con el del targetReference
            string targetPath = targetReference.Path; // Este es el acceso correcto
            if (eventPath == targetPath)
            {
                eventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE); // Detener de inmediato
                eventInstance.release(); // Liberar recursos
                eventInstance.clearHandle(); // Limpiar la referencia

                if (mostrarLogMusic) Debug.Log($"Evento musical {eventPath} liberado y recursos limpiados.");
            }
            else
            {
                if (mostrarLogMusic) Debug.LogWarning($"El evento activo ({eventPath}) no coincide con la referencia proporcionada ({targetPath}). No se realizó la limpieza.");
            }
        }
        else
        {
            if (mostrarLogMusic) Debug.LogWarning("No se encontró una instancia válida de música para limpiar.");
        }
    }
    public void SetMusicArea(MusicArea area)
    {
        musicEventInstance.setParameterByName("area", (float)area);
    }

    #endregion

    #region Reproducir Ambiente y Cambios en el ambiente
    public void InitializeAmbienceEvent(EventReference ambienceEventReference, bool reproducir, Transform transform = null)
    {
        if (reproducir)
        {
            // Limpiar si ya hay un evento activo con la misma referencia
            if (ambienceEventInstance.isValid())
            {
                CleanUpAmbience(ambienceEventInstance, ambienceEventReference);
            }

            // Crear nueva instancia y reproducir
            ambienceEventInstance = CreateEventAmbientInstance(ambienceEventReference, transform);

            if (ambienceEventInstance.isValid())
            {
                ambienceEventInstance.start();
                ambienceEvents.Add(ambienceEventInstance);
                if (mostrarLogSFX) Debug.Log($"Evento de ambiente {ambienceEventReference.Path} reproducido en {(transform != null ? transform.name : "jugador")}");
            }
        }
        else
        {
            // Limpiar solo la instancia que coincide con la referencia proporcionada
            if (ambienceEventInstance.isValid())
            {
                CleanUpAmbience(ambienceEventInstance, ambienceEventReference);
            }
        }
    }

    private void CleanUpAmbience(EventInstance eventInstance, EventReference targetReference)
    {
        if (eventInstance.isValid())
        {
            // Obtener la descripción del evento para comparar con la referencia
            eventInstance.getDescription(out var description);
            description.getPath(out string eventPath);

            // Comparar el path del evento activo con el del targetReference
            string targetPath = targetReference.Path;
            if (eventPath == targetPath)
            {
                // Detener y liberar la instancia si coincide
                eventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                eventInstance.release();
                eventInstance.clearHandle();

                ambienceEvents.Remove(eventInstance);
                if (mostrarLogSFX) Debug.Log($"Evento de ambiente {eventPath} detenido y recursos liberados.");
            }
            else
            {
                if (mostrarLogSFX) Debug.LogWarning($"El evento activo ({eventPath}) no coincide con la referencia proporcionada ({targetPath}). No se realizó la limpieza.");
            }
        }
        else
        {
            if (mostrarLogSFX) Debug.LogWarning("No se encontró una instancia válida de ambiente para limpiar.");
        }
    }
    public EventInstance CreateEventAmbientInstance(EventReference eventReference, Transform followTransform)
    {
        followTransform ??= jugador.transform;

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
    public void SetAmbienceParameter(string parameterName, float parameterValue)
    {
        ambienceEventInstance.setParameterByName(parameterName, parameterValue);
    }
    #endregion

    #region Reproducir Eventos de una sola vez "Play One Shot"

    public void PlayOneShot(EventReference sound, GameObject posicion = null)
    {
        Vector3 finalPos = posicion != null
            ? posicion.transform.position
            : (jugador != null
                ? jugador.transform.position
                : Vector3.zero);

        // Debug.Log($"PlayOneShot sound: {sound.Path}, position: {finalPos}");

        if (!string.IsNullOrEmpty(sound.Path))
        {
            RuntimeManager.PlayOneShot(sound, finalPos);
        }
        else
        {
            Debug.LogWarning("Sound event reference is invalid.");
        }
    }




    #endregion

    #region Reproducir Eventos Timeline 3D - Tipo Pasos

    public void HandleEvent(EventReference eventReference, Transform followTransform = null, bool start = true, bool allowFadeOut = true)
    {
        followTransform ??= jugador.transform;

        if (start)
        {
            EventInstance eventInstance = RuntimeManager.CreateInstance(eventReference);

            if (followTransform != null)
            {
                eventInstance.set3DAttributes(RuntimeUtils.To3DAttributes(followTransform.position));
                eventosActivos[eventInstance] = followTransform;
            }

            PLAYBACK_STATE playbackState;
            eventInstance.getPlaybackState(out playbackState);

            if (playbackState == PLAYBACK_STATE.STOPPED)
            {
                eventInstance.start();
                if (mostrarLogSFX) Debug.Log($"Evento {eventReference.Path} iniciado.");
            }
        }
        else
        {
            EventInstance targetEvent = default;

            // Buscar el evento específico usando el nombre (path)
            foreach (var evento in eventosActivos)
            {
                string eventPath;
                evento.Key.getDescription(out var description);
                description.getPath(out eventPath);

                if (evento.Key.isValid() && eventPath == eventReference.Path)
                {
                    targetEvent = evento.Key;
                    break;
                }
            }

            if (targetEvent.isValid())
            {
                targetEvent.stop(allowFadeOut ? FMOD.Studio.STOP_MODE.ALLOWFADEOUT : FMOD.Studio.STOP_MODE.IMMEDIATE);
                eventosActivos.Remove(targetEvent);
                ReleaseEvent(targetEvent);
                if (mostrarLogSFX) Debug.Log($"Evento {eventReference.Path} detenido.");
            }
            else
            {
                Debug.LogWarning($"No se encontró ninguna instancia activa para el evento: {eventReference.Path}");
            }
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

}
