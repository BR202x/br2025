using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// EDITADO: 29/01/2025 - 22:20
public class ChorroTargetController : MonoBehaviour
{
    [Header("depuracion")]
    [Tooltip("Activa o desactiva los mensajes de depuracion en este script")]
    public bool mostrarLog;
    [Tooltip("Variable de prueba para activar estados manualmente")]
    public bool test = true;

    #region Variables

    [Header("Configuracion de Estados")]
    [Tooltip("Estado actual del chorro")]
    public Estado estadoSeleccionado;
    [Tooltip("Controla si el chorro está activo o no")]
    public bool abrirChorro = false;
    [Tooltip("Referencia al controlador de rotación de superficies")]
    public RotacionSuperficieController superficieController;

    [Header("Estado0: Sin Estados")]
    [Tooltip("Velocidad de retorno del chorro cuando no tiene estado activo")]
    public float velocidadRetorno;
    [Tooltip("Referencia al controlador de golpes de la válvula")]
    public ControladorGolpeValvula golpesValvula;

    [Header("Comportamiento por Defecto")]
    [Tooltip("Lista de objetivos a los que se moverá el chorro")]
    public List<Transform> objetivosChorro = new List<Transform>();
    [Tooltip("Referencia al objeto que sigue el chorro")]
    public Transform targetChorro;
    [Tooltip("Velocidad de movimiento del chorro hacia los objetivos")]
    public float moveSpeed = 2f;

    [Header("Estado 1: Movimiento Aleatorio y Disparo")]
    [Tooltip("Tiempo entre disparos en el estado 1")]
    public float tiempoEntreDisparosEstado1 = 4f;
    [Tooltip("Tiempo de disparo continuo en el estado 1")]
    public float tiempoDisparoQuietoEstado1 = 3f;
    [Tooltip("Referencia a la instancia del chorro")]
    public InstanciaNewChorro chorro;
    [Tooltip("Indica si el estado 1 está activo")]
    public bool estado1 = false;

    private float originalFollowDuration = 10f;
    private float originalIdleAfterFollowDuration = 6f;
    private float originalTiempoEntreDisparos = 6f;
    private float originalTiempoDisparoQuieto = 10f;

    [Header("Estado 2: Seguir al Jugador y Disparo")]
    [Tooltip("Escala de los tiempos para el estado 2")]
    [Range(0f, 3f)]
    public float escalaDeTiempos = 1f;
    [Tooltip("Referencia al jugador")]
    public Transform player;
    [Tooltip("Velocidad de seguimiento del jugador")]
    public float followPlayerSpeed = 5.5f;
    [Tooltip("Duración del seguimiento en el estado 2")]
    public float followDuration;
    [Tooltip("Tiempo de espera después de seguir al jugador")]
    public float idleAfterFollowDuration;
    [Tooltip("Tiempo entre disparos en el estado 2")]
    public float tiempoEntreDisparos;
    [Tooltip("Tiempo de disparo continuo en el estado 2")]
    public float tiempoDisparoQuieto;

    private SeguirTarget seguirTarget;

    private Coroutine moverCorrutina;
    private Coroutine disparoCorrutina;
    private Coroutine seguirPlayerCorrutina;

    [Header("Estado 3: Aturdimiento")]
    [Tooltip("Referencia a la válvula que se aturde")]
    public Transform valvulaPrefab;
    [Tooltip("Duración total del aturdimiento")]
    public float duracionAturdimiento = 5f;
    [Tooltip("Tiempo acumulado en el estado de aturdimiento")]
    public float tiempoAturdimiento = 0;
    [Tooltip("Velocidad de movimiento durante el aturdimiento")]
    public float velocidadAturdimiento = 2f;
    [Tooltip("Indica si el estado 3 está activo")]
    public bool estaEstado3;
    private Coroutine aturdimientoCorrutina;
    [Tooltip("Lista de objetos afectados por el aturdimiento")]
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
        if (mostrarLog) { Debug.Log($"[ChorroTargetController]: Componente del objeto {gameObject.name}"); }

        ActualizarValoresEscalados();

        golpesValvula = GameObject.Find("Valvula").GetComponent<ControladorGolpeValvula>();
        seguirTarget = GameObject.Find("Valvula").GetComponent<SeguirTarget>();
        superficieController = GameObject.Find("RotacionManager").GetComponent<RotacionSuperficieController>();

        if (objetivosChorro.Count == 0 && mostrarLog)
        {
            Debug.LogWarning("[ChorroTargetController]: La lista objetivosChorro está vacía. Agrega objetos para mover el target.");
        }

        CambiarEstado(estadoSeleccionado);
    }

    private void Update()
    {
        if (mostrarLog) { Debug.Log("[ChorroTargetController]: Actualizando estado."); }

        if (Input.GetKeyDown(KeyCode.U)) { EmpezarEstado1(); }
        if (Input.GetKeyDown(KeyCode.I)) { EmpezarEstado2(); }
        if (Input.GetKeyDown(KeyCode.O)) { EmpezarEstado3(); }

        chorro = FindFirstObjectByType<InstanciaNewChorro>();

        if (chorro != null)
        {
            float chorroInicial = chorro.rScale;
            chorro.rScale = estado1 ? 0.5f : chorroInicial;
        }

        if (estaEstado3)
        {
            tiempoAturdimiento += Time.deltaTime;
            if (tiempoAturdimiento >= duracionAturdimiento)
            {
                estaEstado3 = false;
                if (mostrarLog) { Debug.Log("[ChorroTargetController]: Se cumplió el tiempo de aturdimiento."); }

                if (aturdimientoCorrutina != null)
                {
                    StopCoroutine(aturdimientoCorrutina);
                    aturdimientoCorrutina = null;
                }

                CambiarEstado(Estado.SinEstado);
                tiempoAturdimiento = 0;
            }
        }

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

        estadoSeleccionado = nuevoEstado;

        if (mostrarLog) { Debug.Log($"[ChorroTargetController]: Cambio de estado a {nuevoEstado}"); }

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
                if (mostrarLog) { Debug.LogWarning("[ChorroTargetController]: Estado desconocido."); }
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
        if (mostrarLog) { Debug.Log("[ChorroTargetController]: Estado 0 iniciado: Sin Corutinas"); }
    }

    public void StartEstado1()
    {
        estado1 = true;
        superficieController.IniciarCicloEnjuague();
        StopAllCoroutinesForState();
        moverCorrutina = StartCoroutine(MoverAleatoriamente());
        disparoCorrutina = StartCoroutine(Disparo1());

        if (mostrarLog) { Debug.Log("[ChorroTargetController]: Estado 1 iniciado: Movimiento aleatorio y disparo."); }
    }

    public void StartEstado2()
    {
        estado1 = false;
        superficieController.IniciarCicloEnjuague();
        StopAllCoroutinesForState();
        seguirPlayerCorrutina = StartCoroutine(SeguirPlayer());
        disparoCorrutina = StartCoroutine(Disparo2());

        if (mostrarLog) { Debug.Log("[ChorroTargetController]: Estado 2 iniciado: Seguir al jugador y disparo."); }
    }

    public void StartEstado3()
    {
        estado1 = false;
        estaEstado3 = true;
        tiempoAturdimiento = 0f;

        Renderer valvulaRenderer = valvulaPrefab.GetComponentInChildren<Renderer>();
        if (valvulaRenderer != null)
        {
            originalColors.Clear();

            foreach (Material material in valvulaRenderer.materials)
            {
                if (material.HasProperty("_BaseColor"))
                {
                    originalColors[material] = material.GetColor("_BaseColor");
                }
                else if (material.HasProperty("_Color"))
                {
                    originalColors[material] = material.GetColor("_Color");
                }
            }

            colorChangeCoroutine = StartCoroutine(CambiarColorIntermitente(valvulaRenderer.materials));
        }

        aturdimientoCorrutina = StartCoroutine(RotacionesValvula());

        if (mostrarLog) { Debug.Log("[ChorroTargetController]: Estado 3 iniciado: Aturdimiento."); }
    }

    // Aqui vamos

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

        if (mostrarLog) { Debug.Log("[ChorroTargetController]: Todas las corrutinas asociadas al estado actual han sido detenidas."); }
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
            if (mostrarLog) { Debug.Log("[ChorroTargetController]: Estado 1 - Disparando."); }

            yield return new WaitForSeconds(tiempoDisparoQuietoEstado1);

            abrirChorro = false;
            seguirTarget.DestruirChorro();
            if (mostrarLog) { Debug.Log("[ChorroTargetController]: Estado 1 - Deteniendo disparo."); }
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

            if (mostrarLog) { Debug.Log("[ChorroTargetController]: Estado 2 - Quieto después de seguir al jugador."); }
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
            if (mostrarLog) { Debug.Log("[ChorroTargetController]: Estado 2 - Disparando."); }

            yield return new WaitForSeconds(tiempoDisparoQuieto);

            abrirChorro = false;
            seguirTarget.DestruirChorro();
            if (mostrarLog) { Debug.Log("[ChorroTargetController]: Estado 2 - Deteniendo disparo."); }
        }
    }

    private IEnumerator RotacionesValvula()
    {
        while (true)
        {
            Transform currentTarget = GetRandomTargetStun();

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
                Debug.Log($"[ChorroTargetController]: Movimiento completado hacia {currentTarget.name}.");
            }

            yield return new WaitForSeconds(0.00002f); // Pausa entre frames. "Fluidez"
        }
    }


    // Aqui vamos
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
        if (mostrarLog) { Debug.Log("[ChorroTargetController]: Iniciando Estado 0."); }
        CambiarEstado(Estado.SinEstado);
    }

    public void EmpezarEstado1()
    {
        if (mostrarLog) { Debug.Log("[ChorroTargetController]: Iniciando Estado 1."); }
        CambiarEstado(Estado.Estado1);
    }

    public void EmpezarEstado2()
    {
        if (mostrarLog) { Debug.Log("[ChorroTargetController]: Iniciando Estado 2."); }
        CambiarEstado(Estado.Estado2);
    }

    public void EmpezarEstado3() // Aturdimiento
    {
        if (mostrarLog) { Debug.Log("[ChorroTargetController]: Iniciando Estado 3 - Aturdimiento."); }
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
