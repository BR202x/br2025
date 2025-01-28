using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChorroTargetController : MonoBehaviour
{
    #region Variables

    [Header("Depuración")]
    public bool mostrarLog = false;
    public bool test = true;

    [Header("Configuración de Estados")]
    public Estado estadoSeleccionado; // Dropdown en el Inspector para seleccionar el estado
    public bool abrirChorro = false;
    public RotacionSuperficieController superficieController;

    [Header("Estado0: Sin Estados")]
    public float velocidadRetorno;
    public ControladorGolpeValvula golpesValvula;

    [Header("Comportamiento por Defecto")]
    public List<Transform> objetivosChorro = new List<Transform>();
    public Transform targetChorro;
    public float moveSpeed = 2f;

    [Header("Estado 1: Movimiento Aleatorio y Disparo")]
    public float tiempoEntreDisparosEstado1 = 4f;
    public float tiempoDisparoQuietoEstado1 = 3f;
    public InstanciaNewChorro chorro;
    public bool estado1 = false;
    
    private float originalFollowDuration = 10f;
    private float originalIdleAfterFollowDuration = 6f;
    private float originalTiempoEntreDisparos = 6f;
    private float originalTiempoDisparoQuieto = 10f;

    [Header("Estado 2: Seguir al Jugador y Disparo")]
    [Range(0f, 3f)]
    public float escalaDeTiempos = 1f;
    public Transform player;
    public float followPlayerSpeed = 5.5f;

    public float followDuration;
    public float idleAfterFollowDuration;
    public float tiempoEntreDisparos;
    public float tiempoDisparoQuieto;

    private SeguirTarget seguirTarget;

    private Coroutine moverCorrutina;
    private Coroutine disparoCorrutina;
    private Coroutine seguirPlayerCorrutina;

    [Header("Estado 3: Aturdimiento")]
    public Transform valvulaPrefab;
    public float duracionAturdimiento = 5f;
    public float tiempoAturdimiento = 0;
    public float velocidadAturdimiento = 2f;
    public bool estaEstado3;
    private Coroutine aturdimientoCorrutina; 
    public List<Transform> objetosAturdimiento = new List<Transform>();

    private Color originalBaseColor;
    private Dictionary<Material, Color> originalColors = new Dictionary<Material, Color>();
    private Coroutine colorChangeCoroutine;

    private Estado estadoActual = Estado.SinEstado;

    public enum Estado
    {
        SinEstado,
        Estado1,
        Estado2,
        Estado3,
    }

    #endregion

    private void Start()
    {
        ActualizarValoresEscalados();

        golpesValvula = GameObject.Find("Valvula").GetComponent<ControladorGolpeValvula>();
        seguirTarget = GameObject.Find("Valvula").GetComponent<SeguirTarget>();
        superficieController = GameObject.Find("RotacionManager").GetComponent<RotacionSuperficieController>();

        if (objetivosChorro.Count == 0 && mostrarLog)
        {
            Debug.LogWarning("La lista objetivosChorro está vacía. Agrega objetos para mover el target.");
        }

        CambiarEstado(estadoSeleccionado);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            EmpezarEstado1();
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            EmpezarEstado2();
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            EmpezarEstado3();
        }

        // Buscar la instancia del objeto chorro
        chorro = FindFirstObjectByType<InstanciaNewChorro>();

        // Si el chorro existe, manejar su lógica
        if (chorro != null)
        {
            float chorroInicial = chorro.rScale;
            // Si estado1 está activo, asignar un nuevo valor a rScale
            if (estado1)
            {
                chorro.rScale = 0.5f;
            }
            else
            {
                // Dejar el valor original o realizar otra acción
                chorro.rScale = chorroInicial; // Esto mantiene el valor actual
            }
        }

        if (estaEstado3)
        {
            tiempoAturdimiento += Time.deltaTime;

            // Verificar si se supera el tiempo total de aturdimiento
            if (tiempoAturdimiento >= duracionAturdimiento)
            {
                estaEstado3 = false;
                Debug.Log("SE CUMPLIÓ el tiempo de aturdimiento.");

                // Detener la corrutina de aturdimiento
                if (aturdimientoCorrutina != null)
                {
                    StopCoroutine(aturdimientoCorrutina);
                    aturdimientoCorrutina = null; // Limpiar referencia
                }

                
                CambiarEstado(Estado.SinEstado);         
                tiempoAturdimiento = 0;
                
            }
        }

        // Mantener la lógica de actualización de otros estados
        ActualizarValoresEscalados();

        if (estadoSeleccionado != estadoActual)
        {
            CambiarEstado(estadoSeleccionado);
        }
    }

    private void ActualizarValoresEscalados()
    {
        followDuration = originalFollowDuration * escalaDeTiempos;
        idleAfterFollowDuration = originalIdleAfterFollowDuration * escalaDeTiempos;
        tiempoEntreDisparos = originalTiempoEntreDisparos * escalaDeTiempos;
        tiempoDisparoQuieto = originalTiempoDisparoQuieto * escalaDeTiempos;
    }

    public void CambiarEstado(Estado nuevoEstado)
    {
        StopAllCoroutinesForState();
        estadoActual = nuevoEstado;

        // Actualizar el Dropdown en el Inspector
        estadoSeleccionado = nuevoEstado;

        switch (estadoActual)
        {
            case Estado.SinEstado:
                StartSinEstado();
                break;
            case Estado.Estado1:
                StartEstado1();
                break;
            case Estado.Estado2:
                StartEstado2();
                break;
            case Estado.Estado3:
                StartEstado3();
                break;
            default:
                if (mostrarLog) { Debug.LogWarning("Estado desconocido: " + estadoActual); }
                break;
        }
    }

    public void StartSinEstado()
    {
        superficieController.IniciarSinRotacion();
        seguirTarget.DestruirChorro();
        StopAllCoroutinesForState();

        estado1 = false;

        if (!test) { StartCoroutine(CambiarEstados()); }
        if (mostrarLog) { Debug.Log("Estado 0 iniciado: Sin Corutinas"); }
    }

    public void StartEstado1()
    {
        estado1 = true;
        superficieController.IniciarCicloEnjuague();
        StopAllCoroutinesForState();
        moverCorrutina = StartCoroutine(MoverAleatoriamente());
        disparoCorrutina = StartCoroutine(Disparo1());
        if (mostrarLog) { Debug.Log("Estado 1 iniciado: Movimiento aleatorio y disparo."); }
    }

    public void StartEstado2()
    {        
        estado1 = false;
        superficieController.IniciarCicloLavado();
        StopAllCoroutinesForState();
        seguirPlayerCorrutina = StartCoroutine(SeguirPlayer());
        disparoCorrutina = StartCoroutine(Disparo2());
        if (mostrarLog) { Debug.Log("Estado 2 iniciado: Seguir al jugador y disparo."); }
    }

    public void StartEstado3()
    {        
        estado1 = false;
        estaEstado3 = true; // Activar la lógica del estado 3
        tiempoAturdimiento = 0f; // Reiniciar el tiempo transcurrido

        Renderer valvulaRenderer = valvulaPrefab.GetComponentInChildren<Renderer>();
        if (valvulaRenderer != null)
        {
            // Limpiar la lista de colores originales
            originalColors.Clear();

            // Iterar por todos los materiales del objeto
            foreach (Material material in valvulaRenderer.materials)
            {
                // Guardar el color original si el material tiene propiedades relevantes
                if (material.HasProperty("_BaseColor"))
                {
                    originalColors[material] = material.GetColor("_BaseColor");
                }
                else if (material.HasProperty("_Color"))
                {
                    originalColors[material] = material.GetColor("_Color");
                }
            }

            // Iniciar cambio de color
            colorChangeCoroutine = StartCoroutine(CambiarColorIntermitente(valvulaRenderer.materials));
        }

        // Iniciar la corrutina de aturdimiento
        aturdimientoCorrutina = StartCoroutine(RotacionesValvula());

        if (mostrarLog)
        {
            Debug.Log("Estado 3 iniciado: Aturdimiento.");
        }
    }
    private void StopAllCoroutinesForState()
    {

        Renderer valvulaRenderer = valvulaPrefab.GetComponentInChildren<Renderer>();

        foreach (Material material in valvulaRenderer.materials)
        {
            if (originalColors.ContainsKey(material))
            {
                if (material.HasProperty("_BaseColor"))
                {
                    material.SetColor("_BaseColor", originalColors[material]);
                }
                else if (material.HasProperty("_Color"))
                {
                    material.SetColor("_Color", originalColors[material]);
                }
            }
        }

        // Detener todas las corrutinas
        if (colorChangeCoroutine != null) StopCoroutine(colorChangeCoroutine);
        if (moverCorrutina != null) StopCoroutine(moverCorrutina);
        if (disparoCorrutina != null) StopCoroutine(disparoCorrutina);
        if (seguirPlayerCorrutina != null) StopCoroutine(seguirPlayerCorrutina);
        if (aturdimientoCorrutina != null) StopCoroutine(aturdimientoCorrutina);

        colorChangeCoroutine = null;
        moverCorrutina = null;
        disparoCorrutina = null;
        seguirPlayerCorrutina = null;
        aturdimientoCorrutina = null;

        if (mostrarLog) Debug.Log("Todas las corrutinas asociadas al estado actual han sido detenidas.");
    }

    private IEnumerator MoverAleatoriamente()
    {
        targetChorro.position = Vector3.MoveTowards(targetChorro.position, Vector3.zero, 50 * Time.deltaTime);

        while (true)
        {
            if (objetivosChorro.Count > 0)
            {
                Transform currentTarget = GetRandomTarget();
                while (Vector3.Distance(targetChorro.position, currentTarget.position) > 0.1f)
                {
                    targetChorro.position = Vector3.MoveTowards(
                        targetChorro.position,
                        currentTarget.position,
                        moveSpeed * Time.deltaTime
                    );
                    yield return null;
                }
            }
            yield return null;
        }
    }

    private IEnumerator Disparo1()
    {
        while (true)
        {
            yield return new WaitForSeconds(tiempoEntreDisparosEstado1);

            abrirChorro = true;
            seguirTarget.CrearChorro();
            if (mostrarLog) { Debug.Log("Estado 1: Disparando..."); }

            yield return new WaitForSeconds(tiempoDisparoQuietoEstado1);

            abrirChorro = false;
            seguirTarget.DestruirChorro();
            if (mostrarLog) { Debug.Log("Estado 1: Deteniendo disparo..."); }
        }
    }

    private IEnumerator SeguirPlayer()
    {
        while (true)
        {
            float timer = 0f;
            while (timer < followDuration)
            {
                targetChorro.position = Vector3.Lerp(
                    targetChorro.position,
                    new Vector3(player.position.x + 0.5f, player.position.y, player.position.z),
                    followPlayerSpeed * Time.deltaTime
                );

                timer += Time.deltaTime;
                yield return null;
            }

            if (mostrarLog) { Debug.Log("Estado 2: Quieto después de seguir al jugador."); }
            yield return new WaitForSeconds(idleAfterFollowDuration);
        }
    }

    private IEnumerator Disparo2()
    {
        while (true)
        {
            yield return new WaitForSeconds(tiempoEntreDisparos);

            abrirChorro = true;
            seguirTarget.CrearChorro();
            if (mostrarLog) { Debug.Log("Estado 2: Disparando..."); }

            yield return new WaitForSeconds(tiempoDisparoQuieto);

            abrirChorro = false;
            seguirTarget.DestruirChorro();
            if (mostrarLog) { Debug.Log("Estado 2: Deteniendo disparo..."); }
        }
    }

    private IEnumerator RotacionesValvula()
    {
        while (true)
        {
            Transform currentTarget = GetRandomTargetStun();

            /* Comprobacion - 
            if (currentTarget == null)
            {
                Debug.LogWarning("No se encontró un objetivo válido en objetivosChorro.");
                yield break; 
            }
            */

            Vector3 posicionDestino = currentTarget.position;

            while (Vector3.Distance(targetChorro.position, posicionDestino) > 0.1f)
            {
                targetChorro.position = Vector3.MoveTowards(
                    targetChorro.position,
                    posicionDestino,
                    velocidadAturdimiento * Time.deltaTime
                );

                yield return null;
            }

            if (mostrarLog)
            {
                Debug.Log($"Movimiento completado hacia {currentTarget.name}.");
            }

            yield return new WaitForSeconds(0.00002f); // Pausa entre frames. "Fluidez"
        }
    }

    private IEnumerator CambiarColorIntermitente(Material[] materials)
    {
        float elapsedTime = 0f;
        bool toggleColor = false;

        while (estaEstado3)
        {
            elapsedTime += Time.deltaTime;

            // Alternar colores en todos los materiales
            foreach (Material material in materials)
            {
                if (originalColors.ContainsKey(material))
                {
                    Color targetColor = toggleColor ? Color.red : originalColors[material];

                    if (material.HasProperty("_BaseColor"))
                    {
                        material.SetColor("_BaseColor", targetColor);
                    }
                    else if (material.HasProperty("_Color"))
                    {
                        material.SetColor("_Color", targetColor);
                    }
                }
            }

            toggleColor = !toggleColor;
            yield return new WaitForSeconds(0.002f); // Pausa entre frames. "Fluidez"
        }

        // Restaurar colores originales
        foreach (Material material in materials)
        {
            if (originalColors.ContainsKey(material))
            {
                if (material.HasProperty("_BaseColor"))
                {
                    material.SetColor("_BaseColor", originalColors[material]);
                }
                else if (material.HasProperty("_Color"))
                {
                    material.SetColor("_Color", originalColors[material]);
                }
            }
        }
    }


    private Transform GetRandomTarget()
    {
        if (objetivosChorro.Count == 0) return null;
        return objetivosChorro[Random.Range(0, objetivosChorro.Count)];
    }

    private Transform GetRandomTargetStun()
    {
        if (objetosAturdimiento.Count == 0) return null;
        return objetosAturdimiento[Random.Range(0, objetosAturdimiento.Count)];
    }

    public void EmpezarEstado0()
    {
        CambiarEstado(Estado.SinEstado);
    }

    public void EmpezarEstado1()
    {
        CambiarEstado(Estado.Estado1);
    }

    public void EmpezarEstado2()
    {
        CambiarEstado(Estado.Estado2);
    }

    public void EmpezarEstado3() // Aturdimiento
    {
        Debug.Log("ESTADO3");
        CambiarEstado(Estado.Estado3);
    }

    private IEnumerator CambiarEstados()
    {        
        if (test)
        {
            yield break;
        }

        yield return new WaitForSeconds(2f);

        while (Vector3.Distance(targetChorro.position, Vector3.zero) > 0.01f)
        {
            targetChorro.position = Vector3.Lerp(targetChorro.position, Vector3.zero, velocidadRetorno * Time.deltaTime);
            yield return null; // Espera al siguiente frame
        }

        yield return new WaitForSeconds(0.2f);

        if (!golpesValvula.fase2 && !test)
        {
            EmpezarEstado1();
            yield return null;
        }

        yield return new WaitForSeconds(0.2f);

        if (golpesValvula.fase2 && !test)
        {
            EmpezarEstado2();
            yield return null;
        }
    }


}
