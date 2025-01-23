using UnityEngine;

public class RaycastLineExtender : MonoBehaviour
{
    public LineRenderer lineRenderer;      // Referencia al Line Renderer
    public GameObject objectPrefab;       // Prefab del objeto a instanciar
    public float extensionSpeed = 5f;     // Velocidad de extensión en Z
    public float maxDistance = 50f;       // Distancia máxima del Raycast
    public float scaleSpeedConstant = 0.5f; // Constante para escalar X e Y
    public float maxScaleXY = 1f;         // Escala máxima en X e Y

    private GameObject instantiatedObject; // Referencia al objeto instanciado
    private Vector3 targetPoint;           // Punto objetivo del Line Renderer
    private float currentDistance = 0f;    // Distancia actual de extensión
    private Vector3 currentScale;          // Escala actual del objeto

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

        // Instanciar el objeto con su rotación original
        instantiatedObject = Instantiate(objectPrefab, transform.position, Quaternion.identity);

        // Inicializar la escala del objeto
        currentScale = new Vector3(0f, 0f, 0f);
        instantiatedObject.transform.localScale = currentScale;
    }

    void Update()
    {
        // Calcular el punto objetivo del Raycast
        targetPoint = transform.position + transform.up * maxDistance;

        // Gradualmente extender el Line Renderer y ajustar la escala del prefab
        ExtendLineRendererAndObject(targetPoint);
    }

    private void ExtendLineRendererAndObject(Vector3 target)
    {
        // Calcular la distancia que debe recorrer en este frame
        float step = extensionSpeed * Time.deltaTime;

        // Aumentar la distancia actual de extensión hasta el target
        currentDistance = Mathf.MoveTowards(currentDistance, Vector3.Distance(transform.position, target), step);

        // Actualizar el Line Renderer
        Vector3 direction = (target - transform.position).normalized;
        Vector3 endPoint = transform.position + direction * currentDistance;
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, endPoint);

        // Actualizar la escala del prefab
        UpdateScale();
    }

    private void UpdateScale()
    {
        if (instantiatedObject == null) return;

        // Calcular la nueva escala en Y proporcional a la distancia
        currentScale.y = Mathf.Lerp(0, maxScaleXY, currentDistance / maxDistance);

        // Aplicar la nueva escala al objeto
        instantiatedObject.transform.localScale = currentScale;

        // Debug para monitorear escalas
        Debug.Log($"Escala actual del objeto: X = {currentScale.x:F2}, Y = {currentScale.y:F2}, Z = {currentScale.z:F2}");
    }
}
