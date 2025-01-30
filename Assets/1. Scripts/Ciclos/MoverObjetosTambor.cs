using UnityEngine;

public class MoverObjetosTambor : MonoBehaviour
{
    #region Variables

    [Header("depuracion")]
    public bool mostrarDebug;

    [Header("Referencias")]
    public Transform tambor;
    public DetectarRotacionTambor detectarRotacionTambor;

    [Header("Configuración de Fuerzas")]
    public float fuerzaTangencial = 1f;
    public float factorCentripeto = 0.5f; // Factor para ajustar la fuerza hacia el centro
    public ForceMode modoFuerza = ForceMode.Acceleration;

    [Header("Configuración de Colisiones")]
    [Tooltip("Capa de objetos que pueden activar la fuerza")]
    public LayerMask layerObjetivo;

    private bool tocandoSuperficieValida;
    private Rigidbody rigidJugador;

    #endregion

    private void Start()
    {
        rigidJugador = GetComponent<Rigidbody>();
        rigidJugador.useGravity = true;

        if (mostrarDebug) { Debug.Log("Jugador inicializado y preparado para ser afectado por el tambor."); }
    }

    private void FixedUpdate()
    {
        if (detectarRotacionTambor != null && tambor != null && tocandoSuperficieValida)
        {
            if (detectarRotacionTambor.girandoSentidoHorario)
            {
                AplicarFuerzaTangencial(true);
                if (mostrarDebug) { Debug.Log("Tambor girando en sentido horario."); }
            }
            else if (detectarRotacionTambor.girandoSentidoContrario)
            {
                AplicarFuerzaTangencial(false);
                if (mostrarDebug) { Debug.Log("Tambor girando en sentido antihorario."); }
            }
            else
            {
                if (mostrarDebug) { Debug.Log("Tambor está quieto. No se aplica fuerza."); }
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (((1 << collision.gameObject.layer) & layerObjetivo) != 0)
        {
            tocandoSuperficieValida = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (((1 << collision.gameObject.layer) & layerObjetivo) != 0)
        {
            tocandoSuperficieValida = false;
        }
    }

    private void AplicarFuerzaTangencial(bool sentidoHorario)
    {
        // Fuerza Tangencial
        Vector3 posicionRelativa = transform.position - tambor.position;
        Vector3 direccionTangencial = Vector3.Cross(posicionRelativa.normalized, Vector3.up);

        if (sentidoHorario)
        {
            direccionTangencial = -direccionTangencial;
        }

        Vector3 fuerzaTangencialAplicada = direccionTangencial * fuerzaTangencial * Mathf.Abs(detectarRotacionTambor.velocidadRotacionTambor);

        // Fuerza Centrípeta
        Vector3 direccionCentripeta = -posicionRelativa.normalized;
        Vector3 fuerzaCentripetaAplicada = direccionCentripeta * factorCentripeto * posicionRelativa.magnitude;

        // Suma de fuerzas
        Vector3 fuerzaTotal = fuerzaTangencialAplicada + fuerzaCentripetaAplicada;

        rigidJugador.AddForce(fuerzaTotal, modoFuerza);

        if (mostrarDebug)
        {
            Debug.Log($"Fuerza aplicada: Tangencial {fuerzaTangencialAplicada}, Centrípeta {fuerzaCentripetaAplicada}, Total {fuerzaTotal}.");
        }
    }
}
