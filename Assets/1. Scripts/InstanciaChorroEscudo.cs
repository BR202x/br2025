using Unity.VisualScripting;
using UnityEngine;

public class InstanciaChorroEscudo : MonoBehaviour
{
    private LineRenderer lineRenderer; // Referencia al Line Renderer
    private float currentDistance = 0f; // Distancia actual de extensi�n del chorro
    private bool isHitting = false; // Indica si el LineRenderer est� colisionando con algo

    [Header("Configuraci�n del Chorro")]
    public float extensionSpeed = 5f; // Velocidad de extensi�n del chorro
    public float maxDistance = 50f; // Distancia m�xima del Raycast
    public LayerMask collisionLayers; // Capas contra las que el Raycast detectar� colisiones

    [Header("Objeto Shader")]
    public GameObject objectPrefab; // Prefab del objeto que cambia la escala
    private GameObject instantiatedObject; // Referencia al objeto instanciado
    private Vector3 currentScale; // Escala actual del objeto instanciado

    [Header("Configuraci�n de Escala")]
    public float fixedScaleX = 1f; // Escala fija en X
    public float fixedScaleZ = 1f; // Escala fija en Z

    [Header("C�mara")]
    public Camera mainCamera; // Referencia a la c�mara principal (puede configurarse desde el Inspector)

    void Start()
    {
        // Obtener el componente LineRenderer
        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            Debug.LogError("Faltan referencias al LineRenderer en el objeto.");
            return;
        }

        // Asignar la c�mara principal si no se configur� manualmente
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
            if (mainCamera == null)
            {
                Debug.LogError("No se encontr� ninguna c�mara principal en la escena.");
                return;
            }
        }

        // Inicializar el LineRenderer con dos puntos
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, transform.position); // Punto inicial
        lineRenderer.SetPosition(1, transform.position); // Punto final inicial

        // Crear la instancia inicial del prefab
        CreateInstance();
    }

    void Update()
    {
        // Extender el LineRenderer progresivamente
        ExtenderChorro();
    }

    private void ExtenderChorro()
    {
        // Obtener la direcci�n del Raycast desde la c�mara hacia un punto en el centro de la pantalla
        Vector3 rayOrigin = mainCamera.transform.position;
        Vector3 rayDirection = mainCamera.transform.forward;

        // Realizar un Raycast desde la c�mara
        RaycastHit hit;
        bool isObstructed = Physics.Raycast(rayOrigin, rayDirection, out hit, maxDistance, collisionLayers);

        Vector3 endPoint;

        if (isObstructed)
        {
            // Si hay colisi�n, actualizar el punto final del LineRenderer al punto de impacto
            currentDistance = Vector3.Distance(transform.position, hit.point);
            endPoint = hit.point;
            lineRenderer.SetPosition(1, endPoint);

            // Activar el bool cuando hay colisi�n
            isHitting = true;
        }
        else
        {
            // Si no hay colisi�n, extender progresivamente el LineRenderer hacia la direcci�n del Raycast
            float step = extensionSpeed * Time.deltaTime;
            currentDistance = Mathf.MoveTowards(currentDistance, maxDistance, step);
            endPoint = transform.position + rayDirection * currentDistance;

            // Actualizar el punto final del LineRenderer
            lineRenderer.SetPosition(1, endPoint);

            // Desactivar el bool si no est� colisionando
            isHitting = false;
        }

        // Actualizar el punto inicial del LineRenderer
        lineRenderer.SetPosition(0, transform.position);

        // Actualizar la escala y la rotaci�n del objeto asociado
        UpdateObjectTransform(endPoint);
    }

    private void UpdateObjectTransform(Vector3 endPoint)
    {
        if (instantiatedObject == null) return;

        // Ajustar la posici�n del objeto para que est� en la posici�n inicial del LineRenderer
        instantiatedObject.transform.position = transform.position;

        // Corregir la rotaci�n: Aseg�rate de que el objeto apunte hacia el extremo del LineRenderer
        Vector3 directionToEndPoint = (endPoint - transform.position).normalized;
        if (directionToEndPoint != Vector3.zero)
        {
            instantiatedObject.transform.rotation = Quaternion.LookRotation(directionToEndPoint) * Quaternion.Euler(90f, 0f, 0f);
        }

        // Escalar en Y proporcionalmente a la distancia actual del LineRenderer
        currentScale.y = currentDistance / 2f;

        // Escalas fijas en X y Z
        currentScale.x = fixedScaleX;
        currentScale.z = fixedScaleZ;

        // Aplicar la nueva escala al objeto instanciado
        instantiatedObject.transform.localScale = currentScale;
    }

    private void CreateInstance()
    {
        if (objectPrefab == null)
        {
            Debug.LogError("No se asign� un prefab para el objeto que cambia de escala.");
            return;
        }

        // Crear la instancia del objeto en la posici�n inicial del chorro
        instantiatedObject = Instantiate(objectPrefab, transform.position, Quaternion.identity);

        // Inicializar la escala en 0
        currentScale = Vector3.zero;
        instantiatedObject.transform.localScale = currentScale;
    }

    // M�todo p�blico para obtener el estado de colisi�n
    public bool IsHitting()
    {
        return isHitting;
    }

    private void OnDisable()
    {
        Destroy(instantiatedObject.gameObject);
    }

    void OnEnable()
    {
        // Asignar la c�mara principal si el objeto se activa y no tiene una c�mara asignada
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
            if (mainCamera == null)
            {
                Debug.LogError("No se encontr� ninguna c�mara principal en la escena.");
            }
        }
    }
}
