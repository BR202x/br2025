using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class DeteccionMateriales : MonoBehaviour
{
    public EventReference Event;
    private EventInstance eventInstance;
    public bool played = false;

    private void Start()
    {
        // Crear una instancia del evento al inicio
        eventInstance = RuntimeManager.CreateInstance(Event);
    }

    private void OnCollisionEnter(Collision collision)
    {        
        if (collision.gameObject.CompareTag("Agua") && !played)
        {
            eventInstance.setParameterByName("Parameter 1", 2);
            eventInstance.start();
            played = true;            
        }

        if (collision.gameObject.CompareTag("Metal") && !played)
        {
            eventInstance.setParameterByName("Parameter 1", 0);
            eventInstance.start();
            played = true;            
        }

        if (collision.gameObject.CompareTag("Ropa") && !played)
        {
            eventInstance.setParameterByName("Parameter 1", 1);
            eventInstance.start();
            played = true;            
        }

        if (collision.gameObject.CompareTag("RopaMojada") && !played)
        {
            eventInstance.setParameterByName("Parameter 1", 3);
            eventInstance.start();
            played = true;            
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        // Reiniciar el estado "played" al salir de la colisión
        if (collision.gameObject.CompareTag("Agua") ||
            collision.gameObject.CompareTag("Metal") ||
            collision.gameObject.CompareTag("Ropa") ||
            collision.gameObject.CompareTag("RopaMojada"))
        {
            played = false;
        }
    }
}
