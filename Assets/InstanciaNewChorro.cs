using UnityEngine;

public class RaycastLineExtender : MonoBehaviour
{
    public LineRenderer lineRenderer;      // Referencia al Line Renderer
    public GameObject objectPrefab;       // Prefab del objeto a instanciar
    public float extensionSpeed = 5f;     // Velocidad de extensión en Z (configurable en el inspector)
    public float maxDistance = 50f;       // Distancia máxima del Raycast
    public float scaleSpeedConstant = 0.5f; // Constante para escalar X e Y
    public float maxScaleXY = 1f;         // Escala máxima en X e Y (configurable en el inspector)
    public float shrinkSpeed = 1f;        // Velocidad para reducir las escalas X e Y (configurable en el inspector)

    private GameObject instantiatedObject; // Referencia al objeto instanciado
    private Vector3 targetPoint;           // Punto objetivo del Line Renderer
    private bool hitDetected = false;      // Indica si el Raycast ha colisionado con algo
    private float currentDistance = 0f;    // Distancia actual de extensión
    private Vector3 currentScale;          // Escala actual del objeto
    private bool isActive = false;         // Indica si el sistema está activo
    private bool isStopping = false;       // Indica si el sistema está en proceso de detenerse

    void Start()
    {
        if (lineRenderer == null || objectPrefab == null)
        {
            Debug.LogError("Faltan referencias en el script.");
            return;
        }

        // Inicializar el Line Renderer
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, transform.position); // Punto inicial
        lineRenderer.SetPosition(1, transform.position); // Punto final inicial

        // Instanciar el objeto en el punto inicial con escala inicial en X e Y = 0
        instantiatedObject = Instantiate(objectPrefab, transform.position, Quaternion.identity);
        currentScale = new Vector3(0f, 0f, 0f);
        instantiatedObject.transform.localScale = currentScale;
    }

    void Update()
    {
        // Detectar entrada del jugador
        if (Input.GetKeyDown(KeyCode.E))
        {
            StartExtending(); // Iniciar el chorro
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            StopExtending(); // Detener el chorro
        }

        if (!isActive) return;

        if (isStopping)
        {
            ShrinkAndDestroy();
            return;
        }

        // Lanzar un Raycast desde el objeto en la dirección hacia adelante
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxDistance))
        {
            // Si colisiona, actualizar el punto objetivo inmediatamente
            if (!hitDetected || targetPoint != hit.point)
            {
                UpdateLineRendererInstantly(hit.point);
            }

            hitDetected = true;

            // Debug en consola
            Debug.Log($"Colisión detectada a una distancia de {hit.distance:F2} unidades.");
        }
        else
        {
            // Si no colisiona, volver a extender gradualmente hacia el máximo alcance
            if (hitDetected)
            {
                targetPoint = ray.origin + ray.direction * maxDistance;
                hitDetected = false;
            }

            ExtendLineRendererAndObject(targetPoint);
        }

        UpdateScaleAndPosition();
    }

    public void StartExtending()
    {
        isActive = true;
        isStopping = false;
        Debug.Log("Chorro iniciado.");
    }

    public void StopExtending()
    {
        isStopping = true;
        Debug.Log("Chorro detenido.");
    }

    private void ShrinkAndDestroy()
    {
        if (instantiatedObject == null) return;

        // Reducir las escalas X e Y gradualmente a 0
        float shrinkStep = shrinkSpeed * Time.deltaTime;

        currentScale.x = Mathf.MoveTowards(currentScale.x, 0f, shrinkStep);
        currentScale.y = Mathf.MoveTowards(currentScale.y, 0f, shrinkStep);

        // Aplicar la nueva escala al objeto
        Vector3 newScale = instantiatedObject.transform.localScale;
        newScale.x = currentScale.x;
        newScale.y = currentScale.y;
        instantiatedObject.transform.localScale = newScale;

        // Ajustar la posición del objeto solo en el eje Y
        Vector3 currentPosition = instantiatedObject.transform.position;
        currentPosition.y = transform.position.y - (currentScale.y);
        instantiatedObject.transform.position = currentPosition;

        // Destruir el prefab cuando ambas escalas lleguen a 0
        if (currentScale.x <= 0f && currentScale.y <= 0f)
        {
            Destroy(instantiatedObject);
            instantiatedObject = null;
            isActive = false;
            Debug.Log("Prefab destruido tras reducir las escalas a 0.");
        }
    }

    private void UpdateLineRendererInstantly(Vector3 target)
    {
        // Actualizar el Line Renderer instantáneamente
        Vector3 direction = (target - transform.position).normalized;
        currentDistance = Vector3.Distance(transform.position, target);

        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, target);

        // Actualizar la escala en Z y la posición del objeto instanciado
        if (instantiatedObject != null)
        {
            Vector3 newScale = instantiatedObject.transform.localScale;
            newScale.z = currentDistance / 2f; // Escala en Z es la mitad de la distancia
            instantiatedObject.transform.localScale = newScale;

            instantiatedObject.transform.position = transform.position + direction * (currentDistance / 2f);
            instantiatedObject.transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    private void ExtendLineRendererAndObject(Vector3 target)
    {
        // Calcular la distancia que debe recorrer en este frame
        float step = extensionSpeed * Time.deltaTime;

        // Aumentar la distancia actual de extensión hasta el target
        currentDistance = Mathf.MoveTowards(currentDistance, Vector3.Distance(transform.position, target), step);

        // Actualizar el Line Renderer gradualmente
        Vector3 direction = (target - transform.position).normalized;
        Vector3 endPoint = transform.position + direction * currentDistance;
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, endPoint);

        // Actualizar la escala en Z del objeto instanciado
        if (instantiatedObject != null)
        {
            Vector3 newScale = instantiatedObject.transform.localScale;
            newScale.z = currentDistance / 2f; // Escala en Z es la mitad de la distancia
            instantiatedObject.transform.localScale = newScale;

            // Ajustar la posición del objeto
            instantiatedObject.transform.position = transform.position + direction * (currentDistance / 2f);

            // Alinear el objeto con la dirección del Line Renderer
            instantiatedObject.transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    private void UpdateScaleAndPosition()
    {
        if (instantiatedObject == null) return;

        // Incrementar la escala en X e Y basándose en la velocidad y la constante
        float scaleStep = extensionSpeed * scaleSpeedConstant * Time.deltaTime;

        currentScale.x = Mathf.MoveTowards(currentScale.x, maxScaleXY, scaleStep);
        currentScale.y = Mathf.MoveTowards(currentScale.y, maxScaleXY, scaleStep);

        // Aplicar la nueva escala al objeto
        Vector3 newScale = instantiatedObject.transform.localScale;
        newScale.x = currentScale.x;
        newScale.y = currentScale.y;
        instantiatedObject.transform.localScale = newScale;

        // Ajustar la posición del objeto solo en el eje Y
        Vector3 currentPosition = instantiatedObject.transform.position;
        currentPosition.y = transform.position.y - (currentScale.y);
        instantiatedObject.transform.position = currentPosition;

        // Debug de escala y posición
        Debug.Log($"Escala actual del objeto: X = {currentScale.x:F2}, Y = {currentScale.y:F2}, Z = {newScale.z:F2}");
        Debug.Log($"Posición ajustada en Y: {currentPosition.y:F2}");
    }
}
