using UnityEngine;

// EDITADO: 29/01/2025
public class SeguirTarget : MonoBehaviour
{
    [Header("depuracion")]
    [Tooltip("Activa o desactiva los mensajes de depuracion en este script")]
    public bool mostrarLog = false;

    #region Variables

    [Header("Configuracion del Target")]
    [Tooltip("Transform del objetivo al que debe mirar el objeto")]
    public Transform target;
    [Tooltip("Transform de la salida del chorro")]
    public Transform salidaChorro;
    private GameObject refInstancia;

    [Header("Configuracion del Chorro")]
    [Tooltip("Prefab del objeto a instanciar como chorro")]
    public GameObject objectoPrefab;
    public GameObject particula;
    [Tooltip("Referencia al script del chorro")]
    public InstanciaNewChorro chorro;

    [Header("Controles")]
    [Tooltip("Tecla para abrir el chorro")]
    public KeyCode botonAbrir;
    [Tooltip("Tecla para cerrar el chorro")]
    public KeyCode botonCerrar;

    #endregion

    private void Start()
    {
        if (mostrarLog) { Debug.Log($"[SeguirTarget]: Componente del objeto {gameObject.name}"); }
    }

    private void Update()
    {
        if (Input.GetKeyDown(botonAbrir))
        {
            CrearChorro();
        }

        if (Input.GetKeyDown(botonCerrar))
        {
            DestruirChorro();
        }

        chorro = FindFirstObjectByType<InstanciaNewChorro>();

        #region Voltear Salida llave al objetivo
        transform.LookAt(target);
        transform.rotation = Quaternion.LookRotation(target.position - transform.position) * Quaternion.Euler(90f, 0f, 0f);
        #endregion
    }

    public void CrearChorro() // instancia LineRenderer Objeto
    {
        if (mostrarLog) { Debug.Log("[SeguirTarget]: Intentando crear chorro."); }

        if (refInstancia == null)
        {
            AudioImp.Instance.Reproducir("ChorroStart");
            particula.gameObject.SetActive(true);
            refInstancia = Instantiate(objectoPrefab, salidaChorro.transform.position, Quaternion.identity);

            if (mostrarLog) { Debug.Log("[SeguirTarget]: Chorro creado exitosamente."); }
        }
        else
        {
            if (mostrarLog) { Debug.Log("[SeguirTarget]: Chorro ya existe, no se instancia otro."); }
        }
    }

    public void DestruirChorro()
    {
        if (mostrarLog) { Debug.Log("[SeguirTarget]: Intentando destruir chorro."); }

        if (chorro != null)
        {            
            chorro.CerrarChorro();
            particula.gameObject.SetActive(false);
            if (mostrarLog) { Debug.Log("[SeguirTarget]: Chorro destruido."); }
        }
        else
        {
            if (mostrarLog) { Debug.Log("[SeguirTarget]: No hay chorro activo para destruir."); }
        }
    }
}
