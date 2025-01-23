using UnityEngine;

public class InstanciaNewChorro : MonoBehaviour
{
    public LineRenderer lineRenderer;      // Referencia al Line Renderer
    public GameObject objectPrefab;       // Prefab del objeto a instanciar
    public Transform target;              // Referencia al Target (asignado desde el Inspector)
    public LayerMask collisionLayers;     // Capas contra las que se detendr� el Raycast
    public float extensionSpeed = 5f;     // Velocidad de extensi�n en Z
    public float maxDistance = 50f;       // Distancia m�xima del Raycast
    public float scaleSpeedConstant = 0.5f; // Constante para escalar X e Y
    public float rScale = 1f;             // Escala m�xima en X y Z

    private GameObject instantiatedObject; // Referencia al objeto instanciado
    private float currentDistance = 0f;    // Distancia actual de extensi�n
    private Vector3 currentScale;          // Escala actual del objeto instanciado
    private bool wasObstructed = false;    // Estado previo del raycast

    void Start()
    {
        if (lineRenderer == null || objectPrefab == null)
        {
            Debug.LogError("Faltan referencias en el script.");
            return;
        }

        if (target == null)
        {
            Debug.LogError("El Target no est� asignado. Por favor, asigna un objeto desde el inspector.");
            return;
        }

        // Inicializar el Line Renderer
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, transform.position); // Punto inicial
        lineRenderer.SetPosition(1, transform.position); // Punto final inicial

        // Instanciar el objeto con su rotaci�n original
        instantiatedObject = Instantiate(objectPrefab, transform.position, Quaternion.identity);

        // Inicializar las escalas en 0 (X, Y, Z)
        currentScale = Vector3.zero;
        instantiatedObject.transform.localScale = currentScale;
    }

    void Update()
    {
        // Actualizar el punto inicial del Line Renderer
        lineRenderer.SetPosition(0, transform.position);

        if (target == null)
        {
            Debug.LogWarning("El Target no est� asignado. El Raycast no se puede calcular.");
            return;
        }

        // Extender el Line Renderer y verificar colisiones progresivamente
        ExtendLineRendererProgressively();
    }

    private void ExtendLineRendererProgressively()
    {
        // Direcci�n hacia el Target
        Vector3 direction = (target.position - transform.position).normalized;

        // Incrementar la distancia actual progresivamente seg�n la velocidad
        float step = extensionSpeed * Time.deltaTime;
        currentDistance = Mathf.MoveTowards(currentDistance, maxDistance, step);

        // Calcular el punto actual de la extensi�n
        Vector3 currentPoint = transform.position + direction * currentDistance;

        // Hacer un raycast hasta el punto actual de la extensi�n
        RaycastHit hit;
        if (Physics.Raycast(transform.position, direction, out hit, currentDistance, collisionLayers))
        {
            // Si hay colisi�n, detener el avance en el punto de impacto
            currentPoint = hit.point;
            currentDistance = Vector3.Distance(transform.position, hit.point);

            // Registrar que hubo una obstrucci�n
            if (!wasObstructed)
            {
                Debug.Log($"Colisi�n detectada con: {hit.collider.name}. Punto de impacto: {hit.point}");
                wasObstructed = true;
            }
        }
        else
        {
            // Si no hay colisi�n, continuar extendi�ndose
            if (wasObstructed)
            {
                Debug.Log("Camino despejado. Reanudando el movimiento gradual hacia el Target.");
                wasObstructed = false;
            }
        }

        // Actualizar el punto final del Line Renderer
        lineRenderer.SetPosition(1, currentPoint);

        // Actualizar la posici�n, rotaci�n y escala del objeto instanciado
        UpdateObjectTransform(direction, currentDistance);
    }

    private void UpdateObjectTransform(Vector3 direction, float distance)
    {
        if (instantiatedObject == null) return;

        // Ajustar la posici�n del objeto para que su base est� en la posici�n inicial del Line Renderer
        instantiatedObject.transform.position = transform.position;

        // Corregir la rotaci�n: Aseg�rate de que el eje Y apunta hacia la direcci�n
        instantiatedObject.transform.rotation = Quaternion.LookRotation(direction) * Quaternion.Euler(90f, 0f, 0f);

        // Progresivamente ajustar la escala del objeto
        currentScale.y = distance / 2f; // Escala en Y proporcional a la distancia

        // Incrementar X y Z hacia rScale usando Mathf.MoveTowards
        float scaleStep = scaleSpeedConstant * Time.deltaTime;
        currentScale.x = Mathf.MoveTowards(currentScale.x, rScale, scaleStep);
        currentScale.z = Mathf.MoveTowards(currentScale.z, rScale, scaleStep);

        // Aplicar la nueva escala al objeto
        instantiatedObject.transform.localScale = currentScale;

        Debug.Log($"Escala actual: X={currentScale.x:F2}, Y={currentScale.y:F2}, Z={currentScale.z:F2}");
    }
}
