using UnityEngine;

public class InstanciaNewChorro : MonoBehaviour
{
    #region Variables

    [Header("depuracion")]
    public bool mostrarLog = false;

    private LineRenderer lineRenderer;
    private GameObject instantiatedObject;
    private GameObject reboteInstancia;
    public GameObject prefabRebote;
    public Transform target;
    public bool estaTocandoLlenar;

    [Header("Objeto Shader")]
    public GameObject objectPrefab;
    public KeyCode toggleKey = KeyCode.Space;

    [Header("Capas de Colision")]
    public LayerMask collisionLayers;

    [Header("Velocidad de disparo")]
    public float extensionSpeed = 5f;
    public float maxDistance = 50f;
    public float scaleSpeedConstant = 0.5f;

    [Header("Velocidad Cerrado")]
    public float reductionSpeedConstant = 1f;
    public float rScale = 1f;

    private GameObject chorroReboteInst;
    private float currentDistance = 0f;
    private Vector3 currentScale;
    private bool isOpen = false;
    private Vector3 collisionPoint;

    #endregion

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        target = GameObject.Find("Valvula").GetComponent<SeguirTarget>().target;

        if (lineRenderer == null || objectPrefab == null)
        {
            if (mostrarLog) { Debug.LogError("Faltan referencias en el script."); }
            return;
        }

        if (target == null)
        {
            if (mostrarLog) { Debug.LogError("El Target no esta asignado. Por favor, asigna un objeto desde el inspector."); }
            return;
        }

        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, transform.position);

        isOpen = true;
        CreateInstance();
    }

    private void Update()
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
            if (mostrarLog) { Debug.LogWarning("El Target no esta asignado. El Raycast no se puede calcular."); }
            return;
        }

        if (isOpen)
        {
            HandleOpenState();
        }
        else
        {
            HandleCloseState();
        }
    }

    private void HandleOpenState()
    {
        float currentExtensionSpeed = extensionSpeed;

        if (instantiatedObject == null)
        {
            CreateInstance();
        }

        Vector3 direction = (target.position - transform.position).normalized;
        RaycastHit hit;
        bool isObstructed = Physics.Raycast(transform.position, direction, out hit, maxDistance, collisionLayers);

        if (isObstructed)
        {
            float distanciaDelChorro = Vector3.Distance(lineRenderer.GetPosition(1), hit.point);

            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Escudo"))
            {
                currentExtensionSpeed *= 100f;

                if (distanciaDelChorro < 0.1f)
                {
                    if (mostrarLog) { Debug.Log("Prueba Tocar Escudo"); }
                }

                float step = currentExtensionSpeed * Time.deltaTime;
                currentDistance = Mathf.MoveTowards(currentDistance, Vector3.Distance(transform.position, hit.point), step);
                Vector3 currentPoint = transform.position + (hit.point - transform.position).normalized * currentDistance;

                lineRenderer.SetPosition(1, currentPoint);

                if (reboteInstancia == null)
                {
                    InstanciarEnColision(hit);
                }
            }
            else if (hit.collider.gameObject.layer == LayerMask.NameToLayer("SuperficieTambor") ||
                     hit.collider.gameObject.layer == LayerMask.NameToLayer("Flotante"))
            {
                currentExtensionSpeed = extensionSpeed;

                if (distanciaDelChorro <= 0.1f)
                {
                    if (mostrarLog) { Debug.Log("Prueba Llenando"); }
                    estaTocandoLlenar = true;
                }
                else
                {
                    estaTocandoLlenar = false;
                }

                float step = currentExtensionSpeed * Time.deltaTime;
                currentDistance = Mathf.MoveTowards(currentDistance, Vector3.Distance(transform.position, hit.point), step);
                Vector3 currentPoint = transform.position + (hit.point - transform.position).normalized * currentDistance;
                lineRenderer.SetPosition(1, currentPoint);
            }
            else
            {
                currentExtensionSpeed = extensionSpeed;

                float step = currentExtensionSpeed * Time.deltaTime;
                currentDistance = Mathf.MoveTowards(currentDistance, Vector3.Distance(transform.position, hit.point), step);
                Vector3 currentPoint = transform.position + (hit.point - transform.position).normalized * currentDistance;

                lineRenderer.SetPosition(1, currentPoint);
                DestruirRebote();
            }
        }
        else
        {
            currentExtensionSpeed = extensionSpeed;

            float step = currentExtensionSpeed * Time.deltaTime;
            currentDistance = Mathf.MoveTowards(currentDistance, maxDistance, step);
            Vector3 currentPoint = transform.position + direction * currentDistance;

            lineRenderer.SetPosition(1, currentPoint);
            DestruirRebote();
        }

        UpdateObjectTransform(direction, currentDistance, true);
    }

    private void InstanciarEnColision(RaycastHit hit)
    {
        if (prefabRebote != null && reboteInstancia == null)
        {
            if (hit.collider != null)
            {
                reboteInstancia = Instantiate(prefabRebote, hit.transform.position, Quaternion.identity);
                reboteInstancia.transform.SetParent(hit.collider.transform);
                reboteInstancia.transform.localPosition = Vector3.zero;
            }
        }
        else
        {
            if (mostrarLog) { Debug.LogWarning("No se asigno un prefab para la instancia en colision."); }
        }
    }

    private void DestruirRebote()
    {
        if (reboteInstancia != null)
        {
            Destroy(reboteInstancia);
            Destroy(chorroReboteInst);
            reboteInstancia = null;

            if (mostrarLog) { Debug.Log("Instancia de rebote destruida."); }
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

    public void CerrarChorro()
    {
        isOpen = false;
    }
}
