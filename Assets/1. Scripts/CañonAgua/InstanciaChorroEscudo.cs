using Unity.VisualScripting;
using UnityEngine;

// EDITADO: 29/01/2025 - 23:10
public class InstanciaChorroEscudo : MonoBehaviour
{
    [Header("depuracion")]
    [Tooltip("Activa o desactiva los mensajes de depuracion en este script")]
    public bool mostrarLog = false;

    #region Variables

    private LineRenderer lineRenderer; 
    private float currentDistance = 0f; 
    private bool isHitting = false;

    [Header("Configuracion del Chorro")]
    [Tooltip("Velocidad de extension del chorro")]
    public float extensionSpeed = 5f;
    [Tooltip("Distancia maxima del Raycast")]
    public float maxDistance = 50f;
    [Tooltip("Capas contra las que el Raycast detectara colisiones")]
    public LayerMask collisionLayers;

    [Header("Objeto Shader")]
    [Tooltip("Prefab del objeto que cambia la escala")]
    public GameObject objectPrefab;
    private GameObject instantiatedObject; 
    private Vector3 currentScale; 

    [Header("Configuracion de Escala")]
    [Tooltip("Escala fija en X")]
    public float fixedScaleX = 1f;
    [Tooltip("Escala fija en Z")]
    public float fixedScaleZ = 1f;

    [Header("Camara")]
    [Tooltip("Referencia a la camara principal (puede configurarse desde el Inspector)")]
    public Camera mainCamera;

    #endregion

    private void Start()
    {
        if (mostrarLog) { Debug.Log($"[InstanciaChorroEscudo]: Componente del objeto {gameObject.name}"); }

        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            Debug.LogError("[InstanciaChorroEscudo]: Faltan referencias al LineRenderer en el objeto.");
            return;
        }

        if (mainCamera == null)
        {
            mainCamera = Camera.main;
            if (mainCamera == null)
            {
                Debug.LogError("[InstanciaChorroEscudo]: No se encontro ninguna camara principal en la escena.");
                return;
            }
        }

        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, transform.position);

        CreateInstance();
    }

    private void Update()
    {
        ExtenderChorro();
    }

    private void ExtenderChorro()
    {
        Vector3 rayOrigin = mainCamera.transform.position;
        Vector3 rayDirection = mainCamera.transform.forward;

        RaycastHit hit;
        bool isObstructed = Physics.Raycast(rayOrigin, rayDirection, out hit, maxDistance, collisionLayers);

        Vector3 endPoint;
        float step = extensionSpeed * Time.deltaTime;

        if (isObstructed)
        {
            // Extender progresivamente hacia el punto de colisi�n
            currentDistance = Mathf.MoveTowards(currentDistance, Vector3.Distance(transform.position, hit.point), step);
            endPoint = transform.position + rayDirection * currentDistance;
            int hitLayer = hit.collider.gameObject.layer;

            if (currentDistance >= Vector3.Distance(transform.position, hit.point))
            {
                endPoint = hit.point;
                isHitting = true;

                if (mostrarLog) Debug.Log($"Chorro alcanz� el punto de colisi�n en: {endPoint}, Objeto: {hit.collider.name}");
            }
            else if (hitLayer == LayerMask.NameToLayer("Enemigo"))
            {
                Debug.Log("REBOTE ESCUDO CONTRA ENEMIGO");
            }
            else
            {
                isHitting = false;
            }
        }
        else
        {
            // Si no hay colisi�n, extender hasta la distancia m�xima
            currentDistance = Mathf.MoveTowards(currentDistance, maxDistance, step);
            endPoint = transform.position + rayDirection * currentDistance;
            isHitting = false;

            if (mostrarLog) Debug.Log("No hay colisi�n, extendiendo hacia el m�ximo.");
        }

        // Actualizar posiciones del LineRenderer
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, endPoint);

        // Asegurar que el objeto asociado tambi�n siga el movimiento
        UpdateObjectTransform(endPoint);
    }


    private void UpdateObjectTransform(Vector3 endPoint)
    {
        if (instantiatedObject == null) return;

        instantiatedObject.transform.position = transform.position;
        Vector3 directionToEndPoint = (endPoint - transform.position).normalized;
        if (directionToEndPoint != Vector3.zero)
        {
            instantiatedObject.transform.rotation = Quaternion.LookRotation(directionToEndPoint) * Quaternion.Euler(90f, 0f, 0f);
        }

        currentScale.y = currentDistance / 2f;
        currentScale.x = fixedScaleX;
        currentScale.z = fixedScaleZ;
        instantiatedObject.transform.localScale = currentScale;
    }

    private void CreateInstance()
    {
        if (objectPrefab == null)
        {
            Debug.LogError("[InstanciaChorroEscudo]: No se asigno un prefab para el objeto que cambia de escala.");
            return;
        }

        instantiatedObject = Instantiate(objectPrefab, transform.position, Quaternion.identity);
        currentScale = Vector3.zero;
        instantiatedObject.transform.localScale = currentScale;
    }

    public bool IsHitting()
    {
        return isHitting;
    }

    private void OnDisable()
    {
        if (instantiatedObject != null)
        {
            Destroy(instantiatedObject.gameObject);
        }
    }

    public void DestruirRebote()
    {
        if (instantiatedObject != null)
        {
            Destroy(instantiatedObject.gameObject);
        }
    }

    private void OnEnable()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
            if (mainCamera == null)
            {
                Debug.LogError("[InstanciaChorroEscudo]: No se encontro ninguna camara principal en la escena.");
            }
        }
    }
}
