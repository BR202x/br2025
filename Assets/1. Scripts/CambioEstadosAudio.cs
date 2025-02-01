using UnityEngine;

public class CambioEstadosAudio : MonoBehaviour
{
    [Header("depuracion")]
    public bool mostrarLog = false;
    public bool gizmosDebug = false;

    #region Variables

    [Header("Configuración de Suelo")]
    [Tooltip("Capas que cuentan como suelo")]
    public LayerMask sueloLayer;
    [Tooltip("Radio de detección para saber si el jugador está en el suelo")]
    public float radioDeteccion = 0.5f;
    public Transform player;

    [Header("Estados del Jugador")]
    public bool estaPiso;
    public bool estaSaltando;
    public bool puedeSonar = false;

    private bool estadoAnterior;

    #endregion

    private void OnEnable()
    {
        puedeSonar = false;
    }

    private void Start()
    {
        if (mostrarLog) { Debug.Log("[CambioEstadosAudio]: Componente del objeto " + gameObject.name); }
    }

    private void Update()
    {
        DetectarSuelo();
        VerificarCambiosDeEstado();

        if(Input.GetKeyDown(KeyCode.Space))
        {
            puedeSonar = true;
        }
    }

    private void DetectarSuelo()
    {
        Vector3 posicionEsfera = player.position;
        estaPiso = Physics.CheckSphere(posicionEsfera, radioDeteccion, sueloLayer);
        estaSaltando = !estaPiso;
    }

    private void VerificarCambiosDeEstado()
    {
        if (!estaPiso && estadoAnterior != false)
        {
            if (mostrarLog) { Debug.Log("[CambioEstadosAudio]: Jugador comenzó a saltar."); }
        }

        if (estaPiso && estadoAnterior != true && puedeSonar)
        {
            if (mostrarLog) { Debug.Log("[CambioEstadosAudio]: Jugador aterrizó."); }
            AudioImp.Instance.Reproducir("PlayerEffort");
        }

        estadoAnterior = estaPiso;
    }

    private void OnDrawGizmos()
    {
        if (!gizmosDebug) return;

        Gizmos.color = estaPiso ? Color.green : Color.red;
        Vector3 posicionEsfera = player.position;
        Gizmos.DrawWireSphere(posicionEsfera, radioDeteccion);
    }
}
