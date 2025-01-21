using UnityEngine;

public class MoverJugadorPorTambor : MonoBehaviour
{
    #region Variables

    public bool mostrarDebug;
    [Space]
    private Rigidbody rigidJugador;
    [Header("Referencias")]
    public Transform tambor;
    public DetectarRotacionTambor detectarRotacionTambor;

    [Header("Configuración de Fuerzas")]
    public float fuerzaTangencial = 1f;
    public ForceMode modoFuerza = ForceMode.Acceleration;

    [Header("Configuración del Raycast")]
    [Tooltip("Distancia máxima del Raycast hacia abajo")]
    public float distanciaRaycast = 1f;
    [Tooltip("Capa de objetos que puede detectar el Raycast")]
    public LayerMask layerObjetivo;

    #endregion

    private void Start()
    {
        rigidJugador = GetComponent<Rigidbody>();
        rigidJugador.useGravity = true;

        DLog("Jugador inicializado y preparado para ser afectado por el tambor.");
    }

    private void FixedUpdate()
    {
        if (detectarRotacionTambor != null && tambor != null && TocandoSuperficieValida())
        {
            if (detectarRotacionTambor.girandoSentidoHorario)
            {
                AplicarFuerzaTangencial(true);
                DLog("Tambor girando en sentido horario.");
            }
            else if (detectarRotacionTambor.girandoSentidoContrario)
            {
                AplicarFuerzaTangencial(false);
                DLog("Tambor girando en sentido antihorario.");
            }
            else
            {
                DLog("Tambor está quieto. No se aplica fuerza.");
            }
        }
    }

    private bool TocandoSuperficieValida()
    {
        Ray ray = new Ray(transform.position, Vector3.down);
        bool resultado = Physics.Raycast(ray, out RaycastHit hit, distanciaRaycast, layerObjetivo);

        if (mostrarDebug)
        {
            Debug.DrawRay(transform.position, Vector3.down * distanciaRaycast, resultado ? Color.green : Color.red);
        }

        return resultado;
    }

    private void AplicarFuerzaTangencial(bool sentidoHorario)
    {
        Vector3 posicionRelativa = transform.position - tambor.position;
        Vector3 direccionTangencial = Vector3.Cross(posicionRelativa.normalized, Vector3.up);

        if (sentidoHorario)
        {
            direccionTangencial = -direccionTangencial;
        }

        Vector3 fuerzaTangencialAplicada = direccionTangencial * fuerzaTangencial * Mathf.Abs(detectarRotacionTambor.velocidadRotacionTambor);

        rigidJugador.AddForce(fuerzaTangencialAplicada, modoFuerza);

        DLog($"Fuerza tangencial aplicada: {fuerzaTangencialAplicada} (Modo: {modoFuerza})");
    }

    private void DLog(string texto)
    {
        if (mostrarDebug)
        {
            Debug.Log($"[MoverJugadorPorTambor]: {texto}");
        }
    }
}
