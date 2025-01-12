using System.Collections;
using UnityEngine;

public class Stream : MonoBehaviour
{
    private LineRenderer lineRenderer = null;
    public ParticleSystem splashParticle = null;

    private Coroutine pourRoutine = null;

    private Vector3 targetPosition = Vector3.zero;

    public float velocidadAnimacion = 1.75f;

    public bool estaTocando = false;

    [Header("Configuración de Dirección")]
    public Vector3 direccionLinea = Vector3.down; // Dirección configurable desde el Inspector

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        splashParticle = GetComponentInChildren<ParticleSystem>();        
    }

    private void Start()
    {
        // Inicializar puntos de la línea en la posición inicial
        MoveToPosition(0, transform.position);
        MoveToPosition(1, transform.position);
    }

    public void Begin()
    {
        // Inicializar `targetPosition` antes de comenzar a animar
        targetPosition = FindEndPoint();

        // Iniciar las rutinas
        StartCoroutine(UpdateParticle());
        pourRoutine = StartCoroutine(BeginPour());
    }

    private IEnumerator BeginPour()
    {
        while (gameObject.activeSelf)
        {
            targetPosition = FindEndPoint();

            MoveToPosition(0, transform.position);
            AnimateToPosition(1, targetPosition);

            yield return null;
        }
    }

    public void End()
    {
        if (pourRoutine != null)
        {
            StopCoroutine(pourRoutine);
        }

        StartCoroutine(EndPour());
    }

    private IEnumerator EndPour()
    {
        while (!HasReachPosition(0, targetPosition))
        {
            AnimateToPosition(0, targetPosition);
            AnimateToPosition(1, targetPosition);
            yield return null;
        }

        estaTocando = false;
        gameObject.SetActive(false);
    }

    private Vector3 FindEndPoint()
    {
        RaycastHit hit;
        Ray ray = new Ray(transform.position, direccionLinea.normalized);

        // Usa la dirección configurada desde el inspector
        Physics.Raycast(ray, out hit, 50f);
        Vector3 endPoint = hit.collider ? hit.point : ray.GetPoint(2);

        return endPoint;
    }

    private void MoveToPosition(int index, Vector3 targetPosition)
    {
        lineRenderer.SetPosition(index, targetPosition);
    }

    private void AnimateToPosition(int index, Vector3 targetPosition)
    {
        Vector3 currentPoint = lineRenderer.GetPosition(index);
        Vector3 newPosition = Vector3.MoveTowards(currentPoint, targetPosition, Time.deltaTime * velocidadAnimacion);

        lineRenderer.SetPosition(index, newPosition);
    }

    private bool HasReachPosition(int index, Vector3 targetPosition)
    {
        Vector3 currentPosition = lineRenderer.GetPosition(index);
        return currentPosition == targetPosition;
    }

    private IEnumerator UpdateParticle()
    {
        // Esperar hasta que `targetPosition` sea diferente del punto inicial
        yield return new WaitForEndOfFrame();

        while (gameObject.activeSelf)
        {
            // Actualizar la posición de las partículas al final del flujo
            splashParticle.gameObject.transform.position = targetPosition;

            // Verificar si el flujo está tocando el suelo
            bool isHitting = HasReachPosition(1, targetPosition);

            if (isHitting && !estaTocando)
            {
                estaTocando = true;
            }
            else if (!isHitting && estaTocando)
            {
                estaTocando = false;
            }

            splashParticle.gameObject.SetActive(isHitting);

            yield return null;
        }
    }

    private void OnDisable()
    {
        estaTocando = false;
        Destroy(gameObject);
    }
}
