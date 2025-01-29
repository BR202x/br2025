using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using System.Collections.Generic;

public class CaminarDeteccionMaterial : MonoBehaviour
{
    [Header("Configuración del Raycast")]
    public float raycastDistance = 1.5f; // Distancia del raycast (editable desde el Inspector)
    public LayerMask raycastLayerMask; // Capas específicas que el raycast debe considerar

    [Header("Información del Material Detectado")]
    public string material;
    public int materialIndex;
    public DeteccionAguaPies deteccionAguaPies;
    public bool enMetal = true;
    public bool enAgua = false;
    public bool enRopa = false;
    public bool enRopaM = false;

    [Header("Información del Evento")]
    public EventReference Event;
    public string nombreParametro;
    private EventInstance eventInstance;    

    [Header("Lista de Objetos Detectados")]
    public List<GameObject> objetosDetectados = new List<GameObject>(); // Lista de objetos detectados

    private void Start()
    {
        // Inicializar el evento de FMOD
        eventInstance = RuntimeManager.CreateInstance(Event);
        deteccionAguaPies = GameObject.Find("OV_DeteccionPiesAgua").GetComponent<DeteccionAguaPies>();
    }

    private void Update()
    {
        DetectarMaterial();
        ActualizarParametrosFMOD();        
    }

    private void DetectarMaterial()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, Vector3.down, out hit, raycastDistance, raycastLayerMask))
        {
            GameObject objetoDetectado = hit.collider.gameObject;

            // Agregar el objeto a la lista si no está ya
            if (!objetosDetectados.Contains(objetoDetectado))
            {
                objetosDetectados.Add(objetoDetectado);
            }

            // Comparar las etiquetas y ajustar las variables según corresponda
            if (hit.collider.CompareTag("Metal") && !deteccionAguaPies.playerPiesAgua)
            {
                enMetal = true;
                enAgua = false;
                enRopa = false;
                enRopaM = false;
                material = "Metal";
                materialIndex = 0;
            }
            else if (hit.collider.CompareTag("Ropa"))
            {
                enMetal = false;
                enAgua = false;

                if (hit.collider.gameObject.GetComponent<FlotacionObjetos>().estadoMaterial == 0)
                {
                    enRopa = true;
                    material = "Ropa";
                    materialIndex = 1;
                }

                if (hit.collider.gameObject.GetComponent<FlotacionObjetos>().estadoMaterial == 1)
                {
                    enRopaM = true;
                    material = "RopaMojada";
                    materialIndex = 3;
                }
            }
            else if (deteccionAguaPies.playerPiesAgua)
            {
                enAgua = true;
                enMetal = false;
                enRopa = false;
                enRopaM = false;
                material = "Agua";
                materialIndex = 2;
            }
        }
        else
        {
            // Si el raycast no detecta nada, limpiar la lista
            objetosDetectados.Clear();
        }
    }

    private void ActualizarParametrosFMOD()
    {

        if (enMetal && !deteccionAguaPies.playerPiesAgua)
        {
            eventInstance.setParameterByName(nombreParametro, 0);
        }
        else if (enAgua && deteccionAguaPies.playerPiesAgua && !enRopa && !enRopaM)
        {
            eventInstance.setParameterByName(nombreParametro, 2);
        }
        else if (enRopa)
        {
            eventInstance.setParameterByName(nombreParametro, 1);
        }
        else if (enRopaM)
        {
            eventInstance.setParameterByName(nombreParametro, 3);
        }
    }

    private void SetParameter(int materialIndex)
    {
        // eventInstance.setParameterByName(nombreParametro, materialIndex);    
    }

    public void ReproducirEvento()
    {
        eventInstance.start();
    }


    private void OnDrawGizmos()
    {
        // Dibujar el raycast en el editor para depuración
        Gizmos.color = Color.black;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * raycastDistance);
    }
}
