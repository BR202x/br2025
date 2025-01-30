using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using System.Collections.Generic;

// EDITADO: 29/01/2025 - 18:50
public class CaminarDeteccionMaterial : MonoBehaviour
{
    [Header("depuracion")]
    public bool mostrarDebug = false;
    public bool gizmosDebug = false;

    #region Variables

    [Header("Configuracion del Raycast")]
    public float raycastDistance = 1.5f;
    public LayerMask raycastLayerMask;

    [Header("informacion del material detectado")]
    public string material;
    public int materialIndex;
    public DeteccionAguaPies deteccionAguaPies;
    public bool enMetal = true;
    public bool enAgua = false;
    public bool enRopa = false;
    public bool enRopaM = false;

    [Header("informacion del evento")]
    public EventReference Event;
    public string nombreParametro;
    private EventInstance eventInstance;

    [Header("Lista de objetos detectados")]
    public List<GameObject> objetosDetectados = new List<GameObject>();

    #endregion

    private void Start()
    {
        eventInstance = RuntimeManager.CreateInstance(Event);
        deteccionAguaPies = GameObject.Find("OV_DeteccionPiesAgua").GetComponent<DeteccionAguaPies>();

        if (mostrarDebug) { Debug.Log("[CaminarDeteccionMaterial]: Componente del objeto " + gameObject.name); }
    }

    private void Update()
    {
        DetectarMaterial();
        ActualizarParametrosFMOD();
    }

    private void DetectarMaterial()
    {
        if (mostrarDebug) { Debug.Log("[CaminarDeteccionMaterial]: Detectando material..."); }

        RaycastHit hit;

        if (Physics.Raycast(transform.position, Vector3.down, out hit, raycastDistance, raycastLayerMask))
        {
            GameObject objetoDetectado = hit.collider.gameObject;

            if (!objetosDetectados.Contains(objetoDetectado))
            {
                objetosDetectados.Add(objetoDetectado);
            }

            if (hit.collider.CompareTag("Metal") && !deteccionAguaPies.playerPiesAgua)
            {
                enMetal = true;
                enAgua = false;
                enRopa = false;
                enRopaM = false;
                material = "Metal";
                materialIndex = 0;

                if (mostrarDebug) { Debug.Log("[CaminarDeteccionMaterial]: Metal detectado."); }
            }
            else if (hit.collider.CompareTag("Ropa"))
            {
                enMetal = false;
                enAgua = false;

                FlotacionObjetos flotacion = hit.collider.gameObject.GetComponent<FlotacionObjetos>();
                if (flotacion != null)
                {
                    if (flotacion.estadoMaterial == 0)
                    {
                        enRopa = true;
                        material = "Ropa";
                        materialIndex = 1;
                        if (mostrarDebug) { Debug.Log("[CaminarDeteccionMaterial]: Ropa detectada."); }
                    }
                    else if (flotacion.estadoMaterial == 1)
                    {
                        enRopaM = true;
                        material = "RopaMojada";
                        materialIndex = 3;
                        if (mostrarDebug) { Debug.Log("[CaminarDeteccionMaterial]: Ropa mojada detectada."); }
                    }
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

                if (mostrarDebug) { Debug.Log("[CaminarDeteccionMaterial]: Agua detectada."); }
            }
        }
        else
        {
            objetosDetectados.Clear();
        }
    }

    private void ActualizarParametrosFMOD()
    {
        if (mostrarDebug) { Debug.Log("[CaminarDeteccionMaterial]: Actualizando parametros FMOD..."); }

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
        if (mostrarDebug) { Debug.Log($"[CaminarDeteccionMaterial]: SetParameter llamado con indice {materialIndex}."); }
    }

    public void ReproducirEvento()
    {
        eventInstance.start();
        if (mostrarDebug) { Debug.Log("[CaminarDeteccionMaterial]: Reproduciendo evento de sonido."); }
    }

    private void OnDrawGizmos()
    {
        if (!gizmosDebug) return;

        Gizmos.color = Color.black;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * raycastDistance);
    }
}
