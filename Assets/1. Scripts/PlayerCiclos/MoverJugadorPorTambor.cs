using System.Collections.Generic;
using Unity.VisualScripting;
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
    public LayerMask layerObjeto;

    public List<FlotacionObjetos> listaObjetosFlotantes;

    #endregion

    private void Start()
    {
        // Obtener todos los objetos del tipo FlotacionObjetos en la escena
        listaObjetosFlotantes = new List<FlotacionObjetos>(
            Object.FindObjectsByType<FlotacionObjetos>(FindObjectsSortMode.None)
        );

        Debug.Log($"Se han registrado {listaObjetosFlotantes.Count} objetos flotantes.");
    }


    private void Update()
    {
        Ray ray = new Ray(transform.position, Vector3.down);
        RaycastHit hit;

        // Actualizar el estado de todos los objetos
        foreach (var objeto in listaObjetosFlotantes)
        {
            objeto.jugadorParado = false;
        }

        // Si el raycast impacta un objeto, actualizar su estado
        if (Physics.Raycast(ray, out hit, distanciaRaycast, layerObjeto))
        {
            FlotacionObjetos objetoFlotante = hit.collider.GetComponent<FlotacionObjetos>();

            if (objetoFlotante != null)
            {
                objetoFlotante.jugadorParado = true;
                Debug.Log($"Jugador parado sobre: {hit.collider.name}");
            }
        }
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
