using UnityEngine;

public class MoverObjetosTambor : MonoBehaviour
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

    [Header("Configuración de Colisiones")]
    [Tooltip("Capa de objetos que pueden activar la fuerza")]
    public LayerMask layerObjetivo;

    private bool tocandoSuperficieValida;

    #endregion

    private void Start()
    {
        rigidJugador = GetComponent<Rigidbody>();
        rigidJugador.useGravity = true;

        DLog("Jugador inicializado y preparado para ser afectado por el tambor.");
    }

    private void FixedUpdate()
    {
        if (detectarRotacionTambor != null && tambor != null && tocandoSuperficieValida)
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

    private void OnCollisionStay(Collision collision)
    {
        // Verificar si la capa del objeto colisionado es válida
        if (((1 << collision.gameObject.layer) & layerObjetivo) != 0)
        {
            tocandoSuperficieValida = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        // Al salir de la colisión, dejar de aplicar fuerza
        if (((1 << collision.gameObject.layer) & layerObjetivo) != 0)
        {
            tocandoSuperficieValida = false;
        }
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

    private void OnTriggerEnter(Collider other)
    {
        
    }

    private void DLog(string texto)
    {
        if (mostrarDebug)
        {
            Debug.Log($"[MoverJugadorPorTambor]: {texto}");
        }
    }
}
