using System.Collections.Generic;
using UnityEngine;
using FMODUnity;


public class FMODEvents : MonoBehaviour
{
    [System.Serializable]
    public class FMODEvent
    {
        public string nombreEvento;
        public EventReference eventReference;
    }

    [SerializeField] private List<FMODEvent> musica = new List<FMODEvent>();

    [SerializeField] private List<FMODEvent> sfx = new List<FMODEvent>();

    [SerializeField] private List<FMODEvent> ambiente = new List<FMODEvent>();

    public static FMODEvents instance { get; private set; }

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Más de un FMODEvents activo en la escena.");
            return;
        }

        instance = this;
    }

    // Método para obtener un evento por nombre
    public EventReference GetEventReference(string eventName)
    {
        foreach (var musicaEvent in musica)
        {
            if (musicaEvent.nombreEvento == eventName)
            {
                return musicaEvent.eventReference;
            }
        }

        foreach (var sfxEvent in sfx)
        {
            if (sfxEvent.nombreEvento == eventName)
            {
                return sfxEvent.eventReference;
            }
        }

        foreach (var ambienceEvent in ambiente)
        {
            if (ambienceEvent.nombreEvento == eventName)
            {
                return ambienceEvent.eventReference;
            }
        }

        Debug.LogError($"Evento no encontrado: {eventName}");
        return default;
    }

    public void EventPlayOneShot3DUbicacion(string eventReference, Vector3 vector3)
    {     
        AudioManager.instance.PlayOneShot(GetEventReference(eventReference), vector3);
    }

    public void EventPlayOneShot3D(string eventReference)
    {
        AudioManager.instance.PlayOneShot(GetEventReference(eventReference));
    }


    public void EventTimeline3DUbicacion(string eventReference, Transform transform, bool start, bool fade = true)
    {
        AudioManager.instance.HandleEvent(GetEventReference(eventReference), transform, start, fade);
    }

    public void EventTimeline3D(string eventReference, bool start, bool fade = true)
    {
        AudioManager.instance.HandleEvent(GetEventReference(eventReference), null, start, fade);
    }

    public void EventEmitter(string eventReference, string nombreEmisor)
    {
        AudioManager.instance.InitializeEventEmitter(GetEventReference(eventReference), FMODEmisores.instance.GetEmisor(nombreEmisor));
    }

    public void EventAmbient(string eventReference, Transform transform = null)
    {
        AudioManager.instance.InitializeAmbienceEvent(GetEventReference(eventReference), transform);
    }
}
