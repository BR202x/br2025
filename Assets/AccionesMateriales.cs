using FMOD.Studio;
using FMODUnity;
using UnityEngine;
public class AccionesMateriales : MonoBehaviour
{
    // Singleton
    public static AccionesMateriales Instance { get; private set; }

    [Header("Detección Material")]
    public CaminarDeteccionMaterial material;

    [Header("Información del Evento")]
    public EventReference eventoSalto;
    private EventInstance saltoInstance;
    public EventReference eventoDash;
    private EventInstance dashInstance;
    public string nombreParametro;
    

    void Awake()
    {
        // Configuración del singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Elimina duplicados si ya existe una instancia
            return;
        }

        Instance = this;
        // DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        saltoInstance = RuntimeManager.CreateInstance(eventoSalto);
        dashInstance = RuntimeManager.CreateInstance(eventoDash);

        // Encontrar el script `CaminarDeteccionMaterial`
        material = GameObject.Find("RATON  JACK").GetComponent<CaminarDeteccionMaterial>();
    }

    void Update()
    {
        ActualizarMaterial();
    }

    private void ActualizarMaterial()
    {
        #region Salto 

        if (material.enMetal)
        {
            saltoInstance.setParameterByName(nombreParametro, 0);
        }
        else if (material.enAgua && !material.enRopa && !material.enRopaM)
        {
            saltoInstance.setParameterByName(nombreParametro, 2);
        }
        else if (material.enRopa)
        {
            saltoInstance.setParameterByName(nombreParametro, 1);
        }
        else if (material.enRopaM)
        {
            saltoInstance.setParameterByName(nombreParametro, 3);
        }

        #endregion

        #region Dash 

        if (material.enMetal)
        {
            dashInstance.setParameterByName(nombreParametro, 0);
        }
        else if (material.enAgua && !material.enRopa && !material.enRopaM)
        {
            dashInstance.setParameterByName(nombreParametro, 2);
        }
        else if (material.enRopa)
        {
            dashInstance.setParameterByName(nombreParametro, 1);
        }
        else if (material.enRopaM)
        {
            dashInstance.setParameterByName(nombreParametro, 3);
        }

        #endregion

    }

    public void ReproducirSalto()
    {
        saltoInstance.start();
    }

    public void ReproducirDash()
    { 
        dashInstance.start();
    }
}
