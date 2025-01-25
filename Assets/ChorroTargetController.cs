using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChorroTargetController : MonoBehaviour
{
    [Header("Comportamiento por Defecto")]
    public List<Transform> objetivosChorro = new List<Transform>();
    public Transform targetChorro; // El objeto que se moverá
    public float moveSpeed = 2f; // Velocidad de movimiento hacia los objetivos
    private Transform currentTarget; // El objetivo actual al que se está moviendo

    [Header("Configuración del Chorro")]
    public float tiempoEntreAperturas = 5f; // X segundos entre activaciones
    public float duracionApertura = 2f; // Y segundos de duración en estado "abrirChorro"
    private bool abrirChorro = false; // Controla si el chorro está activo o no

    [Header("Comportamiento Seguir al Jugador")]
    public Transform player; // Referencia al jugador
    public float followPlayerSpeed = 3f; // Velocidad de seguimiento al jugador
    public float followDuration = 5f; // Duración del seguimiento configurable desde el Inspector
    public float idleAfterFollowDuration = 2f; // Tiempo que el objeto se queda quieto tras seguir al jugador

    [Header("Disparo Independiente")]
    public float tiempoEntreDisparos = 3f; // Tiempo entre cada disparo
    public float tiempoDisparoQuieto = 2f; // Tiempo que se queda quieto mientras dispara

    private SeguirTarget seguirTarget;

    // Referencias a las corrutinas activas
    private Coroutine moverCorrutina;
    private Coroutine chorroCorrutina;
    private Coroutine seguirPlayerCorrutina;
    private Coroutine disparoCorrutina;

    void Start()
    {
        seguirTarget = GameObject.Find("Valvula").GetComponent<SeguirTarget>();

        // Comienza seleccionando un objetivo aleatorio de la lista
        if (objetivosChorro.Count > 0)
        {
            currentTarget = GetRandomTarget();
        }
        else
        {
            Debug.LogWarning("La lista objetivosChorro está vacía. Agrega objetos para mover el target.");
        }

        // Puedes elegir iniciar alguna de las corrutinas directamente al inicio si lo deseas
        StartFollowPlayerBehavior();
        StartDisparoBehavior();
    }

    // --- Métodos para iniciar/ detener diferentes comportamientos ---

    // Inicia las corrutinas por defecto (mover aleatoriamente y controlar el chorro)
    public void StartDefaultBehavior()
    {
        moverCorrutina = StartCoroutine(MoverAleatoriamente());
        chorroCorrutina = StartCoroutine(ControlarChorro());
    }

    // Detiene las corrutinas por defecto
    public void StopDefaultBehavior()
    {
        if (moverCorrutina != null)
        {
            StopCoroutine(moverCorrutina);
            moverCorrutina = null;
        }

        if (chorroCorrutina != null)
        {
            StopCoroutine(chorroCorrutina);
            chorroCorrutina = null;
        }

        Debug.Log("Comportamiento por defecto detenido.");
    }

    // Inicia el comportamiento de seguir al jugador
    public void StartFollowPlayerBehavior()
    {
        seguirPlayerCorrutina = StartCoroutine(SeguirPlayer());
    }

    // Detiene el comportamiento de seguir al jugador
    public void StopFollowPlayerBehavior()
    {
        if (seguirPlayerCorrutina != null)
        {
            StopCoroutine(seguirPlayerCorrutina);
            seguirPlayerCorrutina = null;
        }

        Debug.Log("Comportamiento de seguir al jugador detenido.");
    }

    // Inicia el comportamiento de disparo independiente
    public void StartDisparoBehavior()
    {
        disparoCorrutina = StartCoroutine(Disparar());
    }

    // Detiene el comportamiento de disparo independiente
    public void StopDisparoBehavior()
    {
        if (disparoCorrutina != null)
        {
            StopCoroutine(disparoCorrutina);
            disparoCorrutina = null;
        }

        Debug.Log("Comportamiento de disparo detenido.");
    }

    // --- Corrutinas ---

    // Corrutina: Movimiento aleatorio continuo
    private IEnumerator MoverAleatoriamente()
    {
        while (true)
        {
            if (currentTarget != null)
            {
                // Mover el objeto hacia el objetivo actual
                targetChorro.position = Vector3.MoveTowards(
                    targetChorro.position,
                    currentTarget.position,
                    moveSpeed * Time.deltaTime
                );

                // Si alcanza el objetivo, selecciona uno nuevo
                if (Vector3.Distance(targetChorro.position, currentTarget.position) < 0.1f)
                {
                    currentTarget = GetRandomTarget();
                }
            }

            // Esperar hasta el próximo frame
            yield return null;
        }
    }

    private IEnumerator ControlarChorro()
    {
        while (true)
        {
            // Espera X segundos antes de abrir el chorro
            yield return new WaitForSeconds(tiempoEntreAperturas);

            // Abrir el chorro
            abrirChorro = true;
            seguirTarget.CrearChorro();
            Debug.Log("Chorro abierto.");

            // Mantén el chorro abierto durante Y segundos
            yield return new WaitForSeconds(duracionApertura);

            // Cerrar el chorro
            abrirChorro = false;
            seguirTarget.DestruirChorro();
            Debug.Log("Chorro cerrado.");
        }
    }


    // Corrutina: Seguir al jugador
    private IEnumerator SeguirPlayer()
    {
        while (true)
        {
            // Fase 1: Seguir al jugador durante el tiempo configurado en el Inspector
            float timer = 0f;

            while (timer < followDuration)
            {
                Vector3 targetPosition = new Vector3(
                    player.position.x,
                    player.position.y, // Offset en Y
                    player.position.z
                );

                targetChorro.position = Vector3.MoveTowards(
                    targetChorro.position,
                    targetPosition,
                    followPlayerSpeed * Time.deltaTime
                );

                timer += Time.deltaTime;
                yield return null;
            }

            // Fase 2: Quedarse quieto durante el tiempo configurado en el Inspector
            Debug.Log("Quieto después de seguir al jugador.");
            yield return new WaitForSeconds(idleAfterFollowDuration);
        }
    }

    // Corrutina: Disparo independiente
    private IEnumerator Disparar()
    {
        while (true)
        {
            // Esperar hasta el próximo disparo
            yield return new WaitForSeconds(tiempoEntreDisparos);

            // Abrir el chorro y disparar
            abrirChorro = true;
            seguirTarget.CrearChorro();
            Debug.Log("Disparando...");

            // Mantener el disparo durante el tiempo configurado
            yield return new WaitForSeconds(tiempoDisparoQuieto);

            // Cerrar el chorro
            abrirChorro = false;
            seguirTarget.DestruirChorro();
            Debug.Log("Deteniendo disparo...");
        }
    }

    // --- Utilidad ---

    private Transform GetRandomTarget()
    {
        if (objetivosChorro.Count == 0)
        {
            Debug.LogWarning("La lista objetivosChorro está vacía. No hay objetivos para seleccionar.");
            return null;
        }

        int randomIndex = Random.Range(0, objetivosChorro.Count);
        return objetivosChorro[randomIndex];
    }
}
