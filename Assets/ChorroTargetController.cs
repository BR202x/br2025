using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChorroTargetController : MonoBehaviour
{
    public List<Transform> objetivosChorro = new List<Transform>();
    public Transform targetChorro; // El objeto que se moverá
    public Transform player;
    public float moveSpeed = 2f; // Velocidad de movimiento
    private Transform currentTarget; // El objetivo actual al que se está moviendo

    [Header("Configuración del Chorro")]
    public float tiempoEntreAperturas = 5f; // X segundos entre activaciones
    public float duracionApertura = 2f; // Y segundos de duración en estado "abrirChorro"
    private bool abrirChorro = false; // Controla si el chorro está activo o no

    private SeguirTarget seguirTarget;

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

        // Iniciar corrutinas
        StartCoroutine(MoverAleatoriamente());
        StartCoroutine(ControlarChorro());
    }

    // Corrutina: Movimiento aleatorio continuo
    private IEnumerator MoverAleatoriamente()
    {
        while (true) // Bucle infinito, pero puedes detenerlo llamando a StopCoroutine()
        {
            if (currentTarget != null)
            {
                // Mover el objeto hacia el objetivo actual
                targetChorro.position = Vector3.MoveTowards(targetChorro.position, currentTarget.position, moveSpeed * Time.deltaTime);

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

    // Corrutina: Controlar el estado de "abrirChorro"
    private IEnumerator ControlarChorro()
    {
        while (true) // Bucle infinito, pero puedes detenerlo llamando a StopCoroutine()
        {
            // Espera X segundos antes de abrir el chorro
            yield return new WaitForSeconds(tiempoEntreAperturas);

            // Abrir el chorro
            abrirChorro = true;
            seguirTarget.CrearChorro(); // Llamar al método cuando se abre
            Debug.Log("Chorro abierto.");

            // Mantén el chorro abierto durante Y segundos
            yield return new WaitForSeconds(duracionApertura);

            // Cerrar el chorro
            abrirChorro = false;
            seguirTarget.DestruirChorro(); // Llamar al método cuando se cierra
            Debug.Log("Chorro cerrado.");
        }
    }

    private Transform GetRandomTarget()
    {
        // Verifica que la lista tenga elementos
        if (objetivosChorro.Count == 0)
        {
            Debug.LogWarning("La lista objetivosChorro está vacía. No hay objetivos para seleccionar.");
            return null;
        }

        // Seleccionar un objetivo aleatorio
        int randomIndex = Random.Range(0, objetivosChorro.Count);
        return objetivosChorro[randomIndex];
    }
}
