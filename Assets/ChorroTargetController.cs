using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChorroTargetController : MonoBehaviour
{
    #region Variables

    [Header("depuracion")]
    public bool mostrarLog = false;

    [Header("Configuracion de Estados")]
    public Estado estadoSeleccionado; // Dropdown en el Inspector para seleccionar el estado
    public bool abrirChorro = false;

    [Header("Comportamiento por Defecto")]
    public List<Transform> objetivosChorro = new List<Transform>();
    public Transform targetChorro;
    public float moveSpeed = 2f;

    [Header("Estado 1: Movimiento Aleatorio y Disparo")]
    public float tiempoEntreDisparosEstado1 = 4f;
    public float tiempoDisparoQuietoEstado1 = 3f;

    private float originalFollowDuration = 10f;
    private float originalIdleAfterFollowDuration = 6f;
    private float originalTiempoEntreDisparos = 6f;
    private float originalTiempoDisparoQuieto = 10f;

    [Header("Estado 2: Seguir al Jugador y Disparo")]
    [Range(0f, 3f)]
    public float escalaDeTiempos = 1f;
    public Transform player;
    public float followPlayerSpeed = 5.5f;

    public float followDuration;
    public float idleAfterFollowDuration;
    public float tiempoEntreDisparos;
    public float tiempoDisparoQuieto;

    private SeguirTarget seguirTarget;

    private Coroutine moverCorrutina;
    private Coroutine disparoCorrutina;
    private Coroutine seguirPlayerCorrutina;

    private Estado estadoActual = Estado.SinEstado;

    public enum Estado
    {
        SinEstado,
        Estado1,
        Estado2
    }

    #endregion

    private void Start()
    {
        ActualizarValoresEscalados();

        seguirTarget = GameObject.Find("Valvula").GetComponent<SeguirTarget>();

        if (objetivosChorro.Count == 0 && mostrarLog)
        {
            Debug.LogWarning("La lista objetivosChorro esta vacia. Agrega objetos para mover el target.");
        }

        CambiarEstado(estadoSeleccionado);
    }

    private void Update()
    {
        ActualizarValoresEscalados();

        if (estadoSeleccionado != estadoActual)
        {
            CambiarEstado(estadoSeleccionado);
        }
    }

    private void ActualizarValoresEscalados()
    {
        followDuration = originalFollowDuration * escalaDeTiempos;
        idleAfterFollowDuration = originalIdleAfterFollowDuration * escalaDeTiempos;
        tiempoEntreDisparos = originalTiempoEntreDisparos * escalaDeTiempos;
        tiempoDisparoQuieto = originalTiempoDisparoQuieto * escalaDeTiempos;
    }

    private void CambiarEstado(Estado nuevoEstado)
    {
        StopAllCoroutinesForState();
        estadoActual = nuevoEstado;

        switch (estadoActual)
        {
            case Estado.SinEstado:
                StartSinEstado();
                break;
            case Estado.Estado1:
                StartEstado1();
                break;
            case Estado.Estado2:
                StartEstado2();
                break;
            default:
                if (mostrarLog) { Debug.LogWarning("Estado desconocido: " + estadoActual); }
                break;
        }
    }

    private void StartSinEstado()
    {
        if (mostrarLog) { Debug.Log("Estado 0 iniciado: Sin Corutinas"); }
    }

    private void StartEstado1()
    {
        moverCorrutina = StartCoroutine(MoverAleatoriamente());
        disparoCorrutina = StartCoroutine(Disparo1());
        if (mostrarLog) { Debug.Log("Estado 1 iniciado: Movimiento aleatorio y disparo."); }
    }

    private void StartEstado2()
    {
        seguirPlayerCorrutina = StartCoroutine(SeguirPlayer());
        disparoCorrutina = StartCoroutine(Disparo2());
        if (mostrarLog) { Debug.Log("Estado 2 iniciado: Seguir al jugador y disparo."); }
    }

    private void StopAllCoroutinesForState()
    {
        if (moverCorrutina != null)
        {
            StopCoroutine(moverCorrutina);
            moverCorrutina = null;
        }
        if (disparoCorrutina != null)
        {
            StopCoroutine(disparoCorrutina);
            disparoCorrutina = null;
        }
        if (seguirPlayerCorrutina != null)
        {
            StopCoroutine(seguirPlayerCorrutina);
            seguirPlayerCorrutina = null;
        }

        if (mostrarLog) { Debug.Log("Todas las corrutinas asociadas al estado actual han sido detenidas."); }
    }

    private IEnumerator MoverAleatoriamente()
    {
        while (true)
        {
            if (objetivosChorro.Count > 0)
            {
                Transform currentTarget = GetRandomTarget();
                while (Vector3.Distance(targetChorro.position, currentTarget.position) > 0.1f)
                {
                    targetChorro.position = Vector3.MoveTowards(
                        targetChorro.position,
                        currentTarget.position,
                        moveSpeed * Time.deltaTime
                    );
                    yield return null;
                }
            }
            yield return null;
        }
    }

    private IEnumerator Disparo1()
    {
        while (true)
        {
            yield return new WaitForSeconds(tiempoEntreDisparosEstado1);

            abrirChorro = true;
            seguirTarget.CrearChorro();
            if (mostrarLog) { Debug.Log("Estado 1: Disparando..."); }

            yield return new WaitForSeconds(tiempoDisparoQuietoEstado1);

            abrirChorro = false;
            seguirTarget.DestruirChorro();
            if (mostrarLog) { Debug.Log("Estado 1: Deteniendo disparo..."); }
        }
    }

    private IEnumerator SeguirPlayer()
    {
        while (true)
        {
            float timer = 0f;
            while (timer < followDuration)
            {
                targetChorro.position = Vector3.MoveTowards(
                    targetChorro.position,
                    new Vector3(player.position.x, player.position.y, player.position.z),
                    followPlayerSpeed * Time.deltaTime
                );

                timer += Time.deltaTime;
                yield return null;
            }

            if (mostrarLog) { Debug.Log("Estado 2: Quieto despues de seguir al jugador."); }
            yield return new WaitForSeconds(idleAfterFollowDuration);
        }
    }

    private IEnumerator Disparo2()
    {
        while (true)
        {
            yield return new WaitForSeconds(tiempoEntreDisparos);

            abrirChorro = true;
            seguirTarget.CrearChorro();
            if (mostrarLog) { Debug.Log("Estado 2: Disparando..."); }

            yield return new WaitForSeconds(tiempoDisparoQuieto);

            abrirChorro = false;
            seguirTarget.DestruirChorro();
            if (mostrarLog) { Debug.Log("Estado 2: Deteniendo disparo..."); }
        }
    }

    private Transform GetRandomTarget()
    {
        if (objetivosChorro.Count == 0) return null;
        return objetivosChorro[Random.Range(0, objetivosChorro.Count)];
    }
}
