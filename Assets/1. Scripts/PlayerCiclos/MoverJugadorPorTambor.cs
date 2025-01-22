using System.Collections.Generic;
using UnityEngine;

public class MoverJugadorPorTambor : MonoBehaviour
{
    [Header("depuracion")]
    public bool mostrarDebug;

    #region Variables

    private Rigidbody rigidJugador;

    [Header("Referencias")]
    public Transform tambor;
    public DetectarRotacionTambor detectarRotacionTambor;

    [Header("Configuracion de Fuerzas")]
    public float fuerzaTangencial = 1f;
    public ForceMode modoFuerza = ForceMode.Acceleration;

    [Header("Configuracion del Raycast")]
    public float distanciaRaycast = 1f;
    public LayerMask layerObjetivo;
    public LayerMask layerObjeto;

    private List<FlotacionObjetos> listaObjetosFlotantes;

    #endregion

    private void Start()
    {
        rigidJugador = GetComponent<Rigidbody>();

        listaObjetosFlotantes = new List<FlotacionObjetos>(
            Object.FindObjectsByType<FlotacionObjetos>(FindObjectsSortMode.None)
        );

        if (mostrarDebug) { Debug.Log($"Se han registrado {listaObjetosFlotantes.Count} objetos flotantes."); }
    }

    private void Update()
    {
        Ray ray = new Ray(transform.position, Vector3.down);
        RaycastHit hit;

        foreach (var objeto in listaObjetosFlotantes)
        {
            objeto.jugadorParado = false;
        }

        #region Logica Hundir Objetos

        if (Physics.Raycast(ray, out hit, distanciaRaycast, layerObjeto))
        {
            FlotacionObjetos objetoFlotante = hit.collider.GetComponent<FlotacionObjetos>();

            if (objetoFlotante != null)
            {
                objetoFlotante.jugadorParado = true;
                if (mostrarDebug) { Debug.Log($"Jugador parado sobre: {hit.collider.name}"); }
            }
        }

        #endregion
    }

    private void FixedUpdate()
    {
        if (detectarRotacionTambor != null && tambor != null && TocandoSuperficieValida())
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
                

        if (mostrarDebug) { Debug.Log($"Fuerza tangencial aplicada: {fuerzaTangencialAplicada} (Modo: {modoFuerza})"); }
    }
}
