using UnityEngine;
using FMODUnity;

using FMOD.Studio;

public class TestAudio : MonoBehaviour
{    
    public Transform jugador;
    public Transform vectorZero;

    public GameObject objetoEmision1;
    public GameObject objetoEmision2;

    public bool rangoInstanciar;
    public float distance;

    private void Start()
    {
        FMODEvents.instance.EventEmitter("Emisor1","Test1");

        FMODEvents.instance.EventEmitter("Emisor2","Test2");

        FMODEvents.instance.EventAmbient("MusicaFondo");

        FMODEvents.instance.EventAmbient("ReverbStates");

        
    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.E))
        {          
            //FMODEvents.instance.EventPlayOneShot3D("BotonMenu");
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            FMODEvents.instance.EventPlayOneShot3DUbicacion("BotonMenu", vectorZero.position);
        }

        
        if (Input.GetKeyDown(KeyCode.T))
        {
            FMODEvents.instance.EventTimeline3DUbicacion("Pasos", vectorZero, true);
        }
        if (Input.GetKeyUp(KeyCode.T))
        {            
            FMODEvents.instance.EventTimeline3DUbicacion("Pasos", vectorZero, false, true);
        }


        if (Input.GetKeyDown(KeyCode.Y))
        {
            FMODEvents.instance.EventTimeline3D("Musica1", true);
        }
        if (Input.GetKeyUp(KeyCode.Y))
        {
            FMODEvents.instance.EventTimeline3D("Musica1", false, true);
        }

    }
}
