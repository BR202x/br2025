using System.Collections.Generic;
using UnityEngine;

// EDITADO: 29/01/2025
public class MoverJugadorPorTambor : MonoBehaviour
{
    [Header("depuracion")]
    [Tooltip("Activa o desactiva los mensajes de depuracion en este script")]
    public bool mostrarDebug;

    #region Variables

    private Rigidbody rigidJugador;

    [Header("Referencias")]
    [Tooltip("Transform del tambor que afecta al jugador")]
    public Transform tambor;
    [Tooltip("Punto inicial del Raycast en el eje Y")]
    public Vector3 inicioRay;
    [Tooltip("Desplazamiento en Y para el inicio del Raycast")]
    public float offsetY;
    [Tooltip("Referencia al script que detecta la rotacion del tambor")]
    public DetectarRotacionTambor detectarRotacionTambor;

    [Header("Configuracion de Fuerzas")]
    [Tooltip("Fuerza tangencial aplicada al jugador")]
    public float fuerzaTangencial = 1f;
    [Tooltip("Modo de aplicacion de la fuerza")]
    public ForceMode modoFuerza = ForceMode.Acceleration;

    [Header("Configuracion del Raycast")]
    [Tooltip("Distancia del Raycast para detectar superficies")]
    public float distanciaRaycast = 1f;
    [Tooltip("Capas que representan las superficies validas")]
    public LayerMask layerSuperficie;
    [Tooltip("Capas que representan los objetos flotantes")]
    public LayerMask layerObjeto;

    private List<FlotacionObjetos> listaObjetosFlotantes;

    #endregion

    private void Start()
    {
        rigidJugador = GetComponent<Rigidbody>();

        listaObjetosFlotantes = new List<FlotacionObjetos>(
            Object.FindObjectsByType<FlotacionObjetos>(FindObjectsSortMode.None)
        );

        if (mostrarDebug)
        {
            Debug.Log($"[MoverJugadorPorTambor]: Componente del objeto {gameObject.name}");
            Debug.Log($"[MoverJugadorPorTambor]: Se han registrado {listaObjetosFlotantes.Count} objetos flotantes.");
        }
    }

    private void Update()
    {
        inicioRay = new Vector3(transform.position.x, transform.position.y + offsetY, transform.position.z);

        Ray ray = new Ray(inicioRay, Vector3.down);
        RaycastHit hit;

        foreach (var objeto in listaObjetosFlotantes)
        {
            objeto.jugadorParado = false;
        }

        if (Physics.Raycast(ray, out hit, distanciaRaycast, layerObjeto))
        {
            FlotacionObjetos objetoFlotante = hit.collider.GetComponent<FlotacionObjetos>();

            if (objetoFlotante != null)
            {
                objetoFlotante.jugadorParado = true;
                if (mostrarDebug) { Debug.Log($"[MoverJugadorPorTambor]: Jugador parado sobre: {hit.collider.name}"); }
            }
        }

        if (mostrarDebug)
        {
            Debug.DrawRay(inicioRay, Vector3.down * distanciaRaycast, Color.green);
        }
    }

    private void FixedUpdate()
    {
        if (detectarRotacionTambor != null && tambor != null && TocandoSuperficieValida())
        {
            if (detectarRotacionTambor.girandoSentidoHorario)
            {
                AplicarFuerzaTangencial(true);
                if (mostrarDebug) { Debug.Log("[MoverJugadorPorTambor]: Tambor girando en sentido horario."); }
            }
            else if (detectarRotacionTambor.girandoSentidoContrario)
            {
                AplicarFuerzaTangencial(false);
                if (mostrarDebug) { Debug.Log("[MoverJugadorPorTambor]: Tambor girando en sentido antihorario."); }
            }
            else
            {
                if (mostrarDebug) { Debug.Log("[MoverJugadorPorTambor]: Tambor está quieto. No se aplica fuerza."); }
            }
        }
    }

    private bool TocandoSuperficieValida()
    {
        Ray ray = new Ray(transform.position, Vector3.down);
        bool resultado = Physics.Raycast(ray, out RaycastHit hit, distanciaRaycast, layerSuperficie);

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

        if (mostrarDebug) { Debug.Log($"[MoverJugadorPorTambor]: Fuerza tangencial aplicada: {fuerzaTangencialAplicada} (Modo: {modoFuerza})"); }
    }
}
