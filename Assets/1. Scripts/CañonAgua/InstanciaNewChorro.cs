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

    [Header("Colision")]
    public bool tocandoJugador;
    public bool tocandoEscudo;
    public bool tocandoTambor;

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

    [Header("Empuje Jugador")]
    public float fuerzaEmpujeJugador;
    public bool hitJugador = false;

    private GameObject chorroReboteInst;
    private float currentDistance = 0f;
    private Vector3 currentScale;
    private bool isOpen = false;
    private Vector3 collisionPoint;
    private bool sonidoReproducido = false;

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

        if (hitJugador && !sonidoReproducido)
        {
            SonidoHitJugador();
        }

    }

    private void HandleOpenState()
    {
        float currentExtensionSpeed = extensionSpeed;
        hitJugador = false;

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

            int hitLayer = hit.collider.gameObject.layer;
            string hitName = hit.collider.gameObject.name;

            if (distanciaDelChorro < 0.1f)
            {
                // Manejo de cada caso
                if ((hitLayer == LayerMask.NameToLayer("SuperficieTambor") ||
                    hitLayer == LayerMask.NameToLayer("Flotante") ||
                    hitLayer == LayerMask.NameToLayer("Agua")) && (distanciaDelChorro < 0.1f && distanciaDelChorro >= 0))
                {
                    HandleFlotanteSuperficie();
                    sonidoReproducido = false;
                }
                else if (hitLayer == LayerMask.NameToLayer("Escudo"))
                {
                    HandleEscudo(currentExtensionSpeed, hit);

                }
                else if (hitName == "Player")
                {
                    if (!hitJugador)
                    {
                        hitJugador = true;
                    }

                    HandlePlayer(hit.collider.gameObject, direction);
                    estaTocandoLlenar = false;
                }
            }
            

            if (distanciaDelChorro < 0.1f && hitLayer != LayerMask.NameToLayer("Escudo"))
            {
                DestruirRebote();
            }

            MoverChorroHacia(currentExtensionSpeed, hit.point);
        }
        else
        {
            HandleNoColision(direction, currentExtensionSpeed);
        }

        UpdateObjectTransform(direction, currentDistance, true);
    }

    private void HandleFlotanteSuperficie()
    {        
        estaTocandoLlenar = true;
        if (mostrarLog) { Debug.Log("[InstanciaChorro]: Llenando Tambor - Tocando Superficie/Flotante/Agua");}
    }

    private void HandleEscudo(float currentExtensionSpeed, RaycastHit hit)
    {
        estaTocandoLlenar = false;
        currentExtensionSpeed *= 100f;

        if (mostrarLog) { Debug.Log("[InstanciaChorro]: Tocando Escudo"); }

        if (reboteInstancia == null)
        {
            InstanciarEnColision(hit);
        }
    }

    private void HandlePlayer(GameObject player, Vector3 direction)
    {
        estaTocandoLlenar = false;
        if (mostrarLog) { Debug.Log("[InstanciaChorro]: Tocando Jugador"); }
                
        MoverJugador(player, direction);
    }

    private void HandleNoColision(Vector3 direction, float currentExtensionSpeed)
    {
        estaTocandoLlenar = false;
        DestruirRebote();
        MoverChorroHacia(currentExtensionSpeed, transform.position + direction * maxDistance);               
    }



    private void MoverJugador(GameObject player, Vector3 direction)
    {        
        Rigidbody playerRb = player.GetComponent<Rigidbody>();

        if (playerRb == null)
        {
            if (mostrarLog) { Debug.LogWarning("El jugador no tiene un Rigidbody."); }
            return;
        }

        // Ignorar el componente Y de la dirección y normalizar
        direction.y = 0f;
        direction = direction.normalized;

        // Aplicar fuerza al jugador en la dirección del chorro        
        playerRb.AddForce(direction * fuerzaEmpujeJugador, ForceMode.Impulse);

        if (mostrarLog)
        {
            Debug.Log($"Jugador empujado en dirección: {direction}, con fuerza: {fuerzaEmpujeJugador}");
        }
    }


    private void MoverChorroHacia(float currentExtensionSpeed, Vector3 targetPoint)
    {
        float step = currentExtensionSpeed * Time.deltaTime;
        currentDistance = Mathf.MoveTowards(currentDistance, Vector3.Distance(transform.position, targetPoint), step);
        Vector3 currentPoint = transform.position + (targetPoint - transform.position).normalized * currentDistance;

        lineRenderer.SetPosition(1, currentPoint);
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

            AudioImp.Instance.Reproducir("ChorroStop");
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

    public void SonidoHitJugador()
    {
        if (!sonidoReproducido)
        {
            Debug.Log("SONIDO DANO CHORRO");
            AudioImp.Instance.Reproducir("PlayerHitChorro");
            AudioImp.Instance.Reproducir("PlayerHurt");
            sonidoReproducido = true;
        }
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
