using UnityEngine;
using FMOD.Studio;
using FMODUnity;

// [RequireComponent(typeof(StudioEventEmitter))]

public class TestAudio : MonoBehaviour
{
    private EventInstance pasosTest1;
    private EventInstance pasosTest2;

    private StudioEventEmitter emitter;
    public GameObject jugador;

    public GameObject objetoEmision1;
    public GameObject objetoEmision2;

    public bool rangoInstanciar;

    public float distance;

    private void Start()
    {

    }

    void Update()
    {
        distance = Vector3.Distance(jugador.transform.position, objetoEmision1.transform.position);

        if (distance <= 10 && !rangoInstanciar)
        {
            AudioManager.instance.InitializeEventEmitter(FMODEvents.instance.cercaniaObjeto1, objetoEmision1);
            AudioManager.instance.InitializeEventEmitter(FMODEvents.instance.cercaniaObjeto2, objetoEmision2);

            rangoInstanciar = true;
        }

        if (distance >= 15)
        {
            AudioManager.instance.CleanUpEmitters();
            rangoInstanciar = false;
        }
        

        if (Input.GetKeyDown(KeyCode.T))
        {
            pasosTest1 = AudioManager.instance.PlayEventInstance(FMODEvents.instance.pasosTest1);
        }
        if (Input.GetKeyUp(KeyCode.T))
        {
            AudioManager.instance.StopEvent(pasosTest1, true);
        }

        if (Input.GetKeyDown(KeyCode.Y))
        {
            pasosTest2 = AudioManager.instance.PlayEventInstance(FMODEvents.instance.pasosTest2, this.transform);
        }
        if (Input.GetKeyUp(KeyCode.Y))
        {
            AudioManager.instance.StopEvent(pasosTest2, true);
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            AudioManager.instance.PlayOneShot(FMODEvents.instance.test1);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            AudioManager.instance.PlayOneShot(FMODEvents.instance.test1, Vector3.zero);
        }
    }
}
