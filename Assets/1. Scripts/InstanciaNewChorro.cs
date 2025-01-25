using UnityEngine;

public class InstanciaNewChorro : MonoBehaviour
{
    private LineRenderer lineRenderer;     // Referencia al Line Renderer
    private GameObject instantiatedObject; // Referencia al objeto instanciado (prefab)
    public GameObject reboteInstancia;    // Referencia a la instancia creada al colisionar con el escudo
    public Transform target;              // Referencia al Target (asignado desde el Inspector)
    public bool estaTocandoLlenar;

    [Header("Objeto Shader")]
    public GameObject objectPrefab;       // Prefab del objeto a instanciar    
    public GameObject prefabRebote;       // Prefab que se instanciar� al colisionar con el escudo
    public KeyCode toggleKey = KeyCode.Space; // Tecla para alternar abrir/cerrar el sistema
    [Header("Capas de Colision")]
    public LayerMask collisionLayers;     // Capas contra las que se detendr� el Raycast
    public LayerMask escudoLayer;         // Capa espec�fica del escudo
    [Header("Velocidad de disparo")]
    public float extensionSpeed = 5f;     // Velocidad de extensi�n en Z
    public float maxDistance = 50f;       // Distancia m�xima del Raycast
    public float scaleSpeedConstant = 0.5f; // Velocidad de escalado al abrir (X y Z)
    [Header("Velocidad Cerrado")]
    public float reductionSpeedConstant = 1f; // Velocidad de reducci�n al cerrar (X y Z)
    public float rScale = 1f;             // Escala m�xima en X y Z

    public GameObject chorroReboteInst;

    private float currentDistance = 0f;    // Distancia actual de extensi�n
    private Vector3 currentScale;          // Escala actual del objeto instanciado
    private bool isOpen = false;           // Estado del sistema (abierto o cerrado)
    private Vector3 collisionPoint;        // Guarda el punto de colisi�n con el Escudo

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
            Debug.LogError("El Target no est� asignado. Por favor, asigna un objeto desde el inspector.");
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
        chorroReboteInst = GameObject.Find("ChorroNew(Clone)");

        if (Input.GetKeyDown(toggleKey))
        {
            isOpen = false;
        }

        lineRenderer.SetPosition(0, transform.position);

        if (target == null)
        {
            Debug.LogWarning("El Target no est� asignado. El Raycast no se puede calcular.");
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
                    InstanciarEnColision(hit);
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

            // Si no hay colisi�n, destruir la instancia de rebote
            DestruirRebote();
        }

        UpdateObjectTransform(direction, currentDistance, true);
    }

    private void InstanciarEnColision(RaycastHit hit)
    {
        if (prefabRebote != null && reboteInstancia == null)
        {
            // Crear la instancia en la posici�n del punto de colisi�n
            reboteInstancia = Instantiate(prefabRebote, hit.point, Quaternion.identity);

            // Configurar como hijo del objeto colisionado
            if (hit.collider != null)
            {
                reboteInstancia.transform.SetParent(hit.collider.transform);
                reboteInstancia.transform.localPosition = hit.collider.transform.InverseTransformPoint(hit.point);
            }
        }
        else
        {
            Debug.LogWarning("No se asign� un prefab para la instancia en colisi�n.");
        }
    }

    private void DestruirRebote()
    {
        if (reboteInstancia != null)
        {
            Destroy(reboteInstancia);
            Destroy(chorroReboteInst);
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

    private void CreateInstance() // El objeto con Shader
    {
        instantiatedObject = Instantiate(objectPrefab, transform.position, Quaternion.identity);
        currentScale = Vector3.zero;
        instantiatedObject.transform.localScale = currentScale;
    }

    public void CerrarChorro()
    {
        isOpen = false;
    }
}
