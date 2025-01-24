using UnityEngine;

public class InstanciaNewChorro : MonoBehaviour
{
    private LineRenderer lineRenderer;     // Referencia al Line Renderer
    private GameObject instantiatedObject; // Referencia al objeto instanciado (prefab)
    private GameObject reboteInstancia;    // Referencia a la instancia creada al colisionar con el escudo
    public Transform target;              // Referencia al Target (asignado desde el Inspector)
    public bool estaTocandoLlenar;

    [Header("Objeto Shader")]
    public GameObject objectPrefab;       // Prefab del objeto a instanciar    
    public GameObject prefabRebote;       // Prefab que se instanciará al colisionar con el escudo
    public KeyCode toggleKey = KeyCode.Space; // Tecla para alternar abrir/cerrar el sistema
    [Header("Capas de Colision")]
    public LayerMask collisionLayers;     // Capas contra las que se detendrá el Raycast
    public LayerMask escudoLayer;         // Capa específica del escudo
    [Header("Velocidad de disparo")]
    public float extensionSpeed = 5f;     // Velocidad de extensión en Z
    public float maxDistance = 50f;       // Distancia máxima del Raycast
    public float scaleSpeedConstant = 0.5f; // Velocidad de escalado al abrir (X y Z)
    [Header("Velocidad Cerrado")]
    public float reductionSpeedConstant = 1f; // Velocidad de reducción al cerrar (X y Z)
    public float rScale = 1f;             // Escala máxima en X y Z

    private float currentDistance = 0f;    // Distancia actual de extensión
    private Vector3 currentScale;          // Escala actual del objeto instanciado
    private bool isOpen = false;           // Estado del sistema (abierto o cerrado)
    private Vector3 collisionPoint;        // Guarda el punto de colisión con el Escudo

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        target = GameObject.Find("Valvula").GetComponent<SeguirTarget>().target;

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

        isOpen = true;

        // Crear la instancia inicial del prefab
        CreateInstance();
    }

    void Update()
    {
        target = GameObject.Find("Valvula").GetComponent<SeguirTarget>().target;

        if (Input.GetKeyDown(toggleKey))
        {
            isOpen = false;
        }

        lineRenderer.SetPosition(0, transform.position);

        if (target == null)
        {
            Debug.LogWarning("El Target no está asignado. El Raycast no se puede calcular.");
            return;
        }

        if (isOpen)
        {
            HandleOpenState(); // Abrir el sistema
        }
        else
        {
            HandleCloseState(); // Cerrar el sistema
        }
    }

    private void HandleOpenState()
    {
        if (instantiatedObject == null)
        {
            CreateInstance();
        }

        Vector3 direction = (target.position - transform.position).normalized;

        RaycastHit hit;
        bool isObstructed = Physics.Raycast(transform.position, direction, out hit, maxDistance, collisionLayers);

        if (isObstructed)
        {
            // Log del objeto impactado y su capa
            // Debug.Log($"Raycast impactó con el objeto: {hit.collider.gameObject.name}, en la capa: {LayerMask.LayerToName(hit.collider.gameObject.layer)}");

            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("SuperficieTambor"))
            {
                Debug.Log("TOCANDO SUPERFICIE");
                estaTocandoLlenar = true;
            }


            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Escudo"))
            {
                collisionPoint = hit.point;

                currentDistance = Vector3.Distance(transform.position, hit.point);
                lineRenderer.SetPosition(1, hit.point);

                // Crear una instancia si no existe ya una
                if (reboteInstancia == null)
                {
                    InstanciarEnColision(hit.point);
                }
            }
            else
            {
                currentDistance = Vector3.Distance(transform.position, hit.point);
                lineRenderer.SetPosition(1, hit.point);

                // Si no estamos colisionando con el escudo, destruir la instancia de rebote
                DestruirRebote();
            }
        }
        else
        {
            float step = extensionSpeed * Time.deltaTime;
            currentDistance = Mathf.MoveTowards(currentDistance, maxDistance, step);
            Vector3 currentPoint = transform.position + direction * currentDistance;

            lineRenderer.SetPosition(1, currentPoint);

            // Si no hay colisión, destruir la instancia de rebote
            DestruirRebote();
        }

        UpdateObjectTransform(direction, currentDistance, true);
    }


    private void InstanciarEnColision(Vector3 collisionPoint)
    {
        if (prefabRebote != null && reboteInstancia == null)
        {
            reboteInstancia = Instantiate(prefabRebote, collisionPoint, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("No se asignó un prefab para la instancia en colisión.");
        }
    }

    private void DestruirRebote()
    {
        if (reboteInstancia != null)
        {
            Destroy(reboteInstancia);
            reboteInstancia = null;
            Debug.Log("Instancia de rebote destruida.");
        }
    }

    private void HandleCloseState()
    {
        if (instantiatedObject == null) return;

        float step = extensionSpeed * Time.deltaTime;
        currentDistance = Mathf.MoveTowards(currentDistance, 0f, step);

        Vector3 currentPoint = transform.position;

        lineRenderer.SetPosition(1, transform.position + (currentPoint - transform.position).normalized * currentDistance);

        float scaleStep = reductionSpeedConstant * Time.deltaTime;
        currentScale.x = Mathf.MoveTowards(currentScale.x, 0f, scaleStep);
        currentScale.z = Mathf.MoveTowards(currentScale.z, 0f, scaleStep);

        instantiatedObject.transform.localScale = currentScale;

        if (currentScale.x <= 0f && currentScale.z <= 0f)
        {
            Destroy(instantiatedObject);

            instantiatedObject = null;

            lineRenderer.SetPosition(1, transform.position);
            currentDistance = 0f;

            Destroy(gameObject);
        }
    }

    private void UpdateObjectTransform(Vector3 direction, float distance, bool isExpanding)
    {
        if (instantiatedObject == null) return;

        instantiatedObject.transform.position = transform.position;
        instantiatedObject.transform.rotation = Quaternion.LookRotation(direction) * Quaternion.Euler(90f, 0f, 0f);

        currentScale.y = distance / 2f;

        float scaleStep = scaleSpeedConstant * Time.deltaTime;
        currentScale.x = Mathf.MoveTowards(currentScale.x, isExpanding ? rScale : 0f, scaleStep);
        currentScale.z = Mathf.MoveTowards(currentScale.z, isExpanding ? rScale : 0f, scaleStep);

        instantiatedObject.transform.localScale = currentScale;
    }

    private void CreateInstance()
    {
        instantiatedObject = Instantiate(objectPrefab, transform.position, Quaternion.identity);
        currentScale = Vector3.zero;
        instantiatedObject.transform.localScale = currentScale;
    }
}
