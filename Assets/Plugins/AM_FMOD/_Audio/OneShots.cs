using UnityEngine;
using FMODUnity;

public class Oneshots : MonoBehaviour
{
    [Header("FMOD Event")]
    public EventReference Event; // Aqui toca cambiarlo, cambia 'string' por 'EventReference'
    public bool PlayOnAwake;

    public void PlayOneShot()
    {
        RuntimeManager.PlayOneShotAttached(Event, gameObject);      
    }

    private void Start()
    {
        if (PlayOnAwake)
        {
            PlayOneShot();
        }
    }
}
