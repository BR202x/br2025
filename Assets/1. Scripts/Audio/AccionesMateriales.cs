using FMOD.Studio;
using FMODUnity;
using UnityEngine;

// EDITADO: 29/01/2025 - 18:20
public class AccionesMateriales : MonoBehaviour
{
    public static AccionesMateriales Instance { get; private set; }

    #region Variables

    [Header("depuracion")]
    public bool mostrarDebug = false;

    [Header("deteccion material")]
    public CaminarDeteccionMaterial material;

    [Header("informacion del evento")]
    public EventReference eventoSalto;
    private EventInstance saltoInstance;
    public EventReference eventoDash;
    private EventInstance dashInstance;
    public string nombreParametro;

    #endregion

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (mostrarDebug) { Debug.Log("[AccionesMateriales]: Awake ejecutado."); }
    }

    private void Start()
    {
        saltoInstance = RuntimeManager.CreateInstance(eventoSalto);
        dashInstance = RuntimeManager.CreateInstance(eventoDash);

        material = GameObject.Find("RATON  JACK").GetComponent<CaminarDeteccionMaterial>();

        if (mostrarDebug) { Debug.Log("[AccionesMateriales]: Start ejecutado. Instancias de eventos creadas y material detectado."); }
    }

    private void Update()
    {
        ActualizarMaterial();
    }

    private void ActualizarMaterial()
    {
        if (mostrarDebug) { Debug.Log("[AccionesMateriales]: Actualizando material..."); }

        #region Salto 

        if (material.enMetal)
        {
            saltoInstance.setParameterByName(nombreParametro, 0);
            if (mostrarDebug) { Debug.Log("[AccionesMateriales]: Salto en metal."); }
        }
        else if (material.enAgua && !material.enRopa && !material.enRopaM)
        {
            saltoInstance.setParameterByName(nombreParametro, 2);
            if (mostrarDebug) { Debug.Log("[AccionesMateriales]: Salto en agua."); }
        }
        else if (material.enRopa)
        {
            saltoInstance.setParameterByName(nombreParametro, 1);
            if (mostrarDebug) { Debug.Log("[AccionesMateriales]: Salto en ropa."); }
        }
        else if (material.enRopaM)
        {
            saltoInstance.setParameterByName(nombreParametro, 3);
            if (mostrarDebug) { Debug.Log("[AccionesMateriales]: Salto en ropa mojada."); }
        }

        #endregion

        #region Dash 

        if (material.enMetal)
        {
            dashInstance.setParameterByName(nombreParametro, 0);
            if (mostrarDebug) { Debug.Log("[AccionesMateriales]: Dash en metal."); }
        }
        else if (material.enAgua && !material.enRopa && !material.enRopaM)
        {
            dashInstance.setParameterByName(nombreParametro, 2);
            if (mostrarDebug) { Debug.Log("[AccionesMateriales]: Dash en agua."); }
        }
        else if (material.enRopa)
        {
            dashInstance.setParameterByName(nombreParametro, 1);
            if (mostrarDebug) { Debug.Log("[AccionesMateriales]: Dash en ropa."); }
        }
        else if (material.enRopaM)
        {
            dashInstance.setParameterByName(nombreParametro, 3);
            if (mostrarDebug) { Debug.Log("[AccionesMateriales]: Dash en ropa mojada."); }
        }

        #endregion
    }

    public void ReproducirSalto()
    {
        saltoInstance.start();
        if (mostrarDebug) { Debug.Log("[AccionesMateriales]: Reproduciendo sonido de salto."); }
    }

    public void ReproducirDash()
    {
        dashInstance.start();
        if (mostrarDebug) { Debug.Log("[AccionesMateriales]: Reproduciendo sonido de dash."); }
    }
}
