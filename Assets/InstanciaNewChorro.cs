using UnityEngine;

public class InstanciaNewChorro : MonoBehaviour
{
    public LineRenderer lineRenderer;      // Referencia al Line Renderer
    public GameObject objectPrefab;       // Prefab del objeto a instanciar
    public Transform target;              // Referencia al Target (asignado desde el Inspector)
    public LayerMask collisionLayers;     // Capas contra las que se detendrá el Raycast
    public float extensionSpeed = 5f;     // Velocidad de extensión en Z
    public float maxDistance = 50f;       // Distancia máxima del Raycast
    public float scaleSpeedConstant = 0.5f; // Constante para escalar X e Y
    public float rScale = 1f;             // Escala máxima en X y Z

    private GameObject instantiatedObject; // Referencia al objeto instanciado
    private float currentDistance = 0f;    // Distancia actual de extensión
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
            Debug.LogError("El Target no está asignado. Por favor, asigna un objeto desde el inspector.");
            return;
        }

        // Inicializar el Line Renderer
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, transform.position); // Punto inicial
        lineRenderer.SetPosition(1, transform.position); // Punto final inicial

        // Instanciar el objeto con su rotación original
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
            Debug.LogWarning("El Target no está asignado. El Raycast no se puede calcular.");
            return;
        }

        // Extender el Line Renderer y verificar colisiones progresivamente
        ExtendLineRendererProgressively();
    }

    private void ExtendLineRendererProgressively()
    {
        // Dirección hacia el Target
        Vector3 direction = (target.position - transform.position).normalized;

        // Incrementar la distancia actual progresivamente según la velocidad
        float step = extensionSpeed * Time.deltaTime;
        currentDistance = Mathf.MoveTowards(currentDistance, maxDistance, step);

        // Calcular el punto actual de la extensión
        Vector3 currentPoint = transform.position + direction * currentDistance;

        // Hacer un raycast hasta el punto actual de la extensión
        RaycastHit hit;
        if (Physics.Raycast(transform.position, direction, out hit, currentDistance, collisionLayers))
        {
            // Si hay colisión, detener el avance en el punto de impacto
            currentPoint = hit.point;
            currentDistance = Vector3.Distance(transform.position, hit.point);

            // Registrar que hubo una obstrucción
            if (!wasObstructed)
            {
                Debug.Log($"Colisión detectada con: {hit.collider.name}. Punto de impacto: {hit.point}");
                wasObstructed = true;
            }
        }
        else
        {
            // Si no hay colisión, continuar extendiéndose
            if (wasObstructed)
            {
                Debug.Log("Camino despejado. Reanudando el movimiento gradual hacia el Target.");
                wasObstructed = false;
            }
        }

        // Actualizar el punto final del Line Renderer
        lineRenderer.SetPosition(1, currentPoint);

        // Actualizar la posición, rotación y escala del objeto instanciado
        UpdateObjectTransform(direction, currentDistance);
    }

    private void UpdateObjectTransform(Vector3 direction, float distance)
    {
        if (instantiatedObject == null) return;

        // Ajustar la posición del objeto para que su base esté en la posición inicial del Line Renderer
        instantiatedObject.transform.position = transform.position;

        // Corregir la rotación: Asegúrate de que el eje Y apunta hacia la dirección
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
