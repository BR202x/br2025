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
    public bool iniciar = false;

    #region Variables

    #region Estado Inicial
    [Header("Configuracion de Estados")]
    [Tooltip("Estado actual del chorro")]
    public Estado estadoSeleccionado;
    [Tooltip("Controla si el chorro est� activo o no")]
    public bool abrirChorro = false;
    [Tooltip("Referencia al controlador de rotaci�n de superficies")]
    public RotacionSuperficieController superficieController;
    [Header("Estado0: Sin Estados")]
    [Tooltip("Velocidad de retorno del chorro cuando no tiene estado activo")]
    public float velocidadRetorno;
    [Tooltip("Referencia al controlador de golpes de la v�lvula")]
    public ControladorGolpeValvula golpesValvula;
    [Header("Comportamiento por Defecto")]
    [Tooltip("Lista de objetivos a los que se mover� el chorro")]
    public List<Transform> objetivosChorro = new List<Transform>();
    [Tooltip("Referencia al objeto que sigue el chorro")]
    public Transform targetChorro;
    [Tooltip("Velocidad de movimiento del chorro hacia los objetivos")]
    public float moveSpeed = 2f;
    #endregion

    #region VariablesEstado1
    [Header("Estado 1: Movimiento Aleatorio y Disparo")]
    [Tooltip("Tiempo entre disparos en el estado 1")]
    public float tiempoEntreDisparosEstado1 = 4f;
    [Tooltip("Tiempo de disparo continuo en el estado 1")]
    public float tiempoDisparoQuietoEstado1 = 3f;
    [Tooltip("Referencia a la instancia del chorro")]
    public InstanciaNewChorro chorro;
    [Tooltip("Indica si el estado 1 est� activo")]
    public bool estado1 = false;
    private float originalFollowDuration = 10f;
    private float originalIdleAfterFollowDuration = 6f;
    private float originalTiempoEntreDisparos = 6f;
    private float originalTiempoDisparoQuieto = 10f;
    #endregion

    #region VariablesEstado2
    [Header("Estado 2: Seguir al Jugador y Disparo")]
    [Tooltip("Escala de los tiempos para el estado 2")]
    [Range(0f, 3f)]
    public float escalaDeTiempos = 1f;
    [Tooltip("Referencia al jugador")]
    public Transform player;
    [Tooltip("Velocidad de seguimiento del jugador")]
    public float followPlayerSpeed = 5.5f;
    [Tooltip("Duraci�n del seguimiento en el estado 2")]
    public float followDuration;
    [Tooltip("Tiempo de espera despu�s de seguir al jugador")]
    public float idleAfterFollowDuration;
    [Tooltip("Tiempo entre disparos en el estado 2")]
    public float tiempoEntreDisparos;
    [Tooltip("Tiempo de disparo continuo en el estado 2")]
    public float tiempoDisparoQuieto;
    #endregion

    #region VariablesEstado3 
    [Header("Estado 3: Aturdimiento")]
    
    public Transform valvulaPrefab;    
    public float duracionAturdimiento = 5f;    
    public float tiempoAturdimiento = 0;    
    public float velocidadAturdimiento = 2f;
    public bool estaAturdido = false;
    public bool estaEstado3;
    
    public List<Transform> objetosAturdimiento = new List<Transform>();
    #endregion

    #region VariablesEstado4

    [Header("Estado 4: Seguir al Jugador y Disparo")]
    [Tooltip("Escala de los tiempos para el estado 4")]
    [Range(0f, 3f)]
    public float escalaDeTiempos4 = 1f;
    [Tooltip("Velocidad de seguimiento del jugador en el estado 4")]
    public float followPlayerSpeed4 = 6f;
    [Tooltip("Duraci�n del seguimiento en el estado 4")]
    public float followDuration4 = 3f;
    [Tooltip("Tiempo de espera despu�s de seguir al jugador en el estado 4")]
    public float idleAfterFollowDuration4 = 1f;
    [Tooltip("Tiempo entre disparos en el estado 4")]
    public float tiempoEntreDisparos4 = 0.5f;
    [Tooltip("Tiempo de disparo continuo en el estado 4")]
    public float tiempoDisparoQuieto4 = 1f;

    #endregion

    private Dictionary<Material, Color> originalColors = new Dictionary<Material, Color>();
        private Coroutine colorChangeCoroutine;
        private Coroutine aturdimientoCorrutina;
        private Coroutine moverCorrutina;
        private Coroutine disparoCorrutina;
        private Coroutine seguirPlayerCorrutina;
        private Color originalBaseColor;
        private Estado estadoActual = Estado.SinEstado;
        private SeguirTarget seguirTarget;
    private Coroutine colorChangeCoroutine4;
    private Coroutine seguirPlayerCorrutina4;
    private Coroutine disparoCorrutina4;

    public enum Estado
    {
        SinEstado,
        Estado1,
        Estado2,
        Estado3,
        Estado4,
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
            Debug.LogWarning("[ChorroTargetController]: La lista objetivosChorro est� vac�a. Agrega objetos para mover el target.");
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
                if (mostrarLog) { Debug.Log("[ChorroTargetController]: Se cumpli� el tiempo de aturdimiento."); }

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
            case Estado.Estado4:
                StartEstado4();
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

        if (!test && iniciar) { StartCoroutine(CambiarEstados()); }
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

    public void StartEstado4()
    {
        estado1 = false;
        superficieController.velocidadEnjuague = 20;
        superficieController.duracionGiro = 10;
        superficieController.pausaEntreGiros = 1;
        superficieController.IniciarCicloEnjuague();
        StopAllCoroutinesForState();

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

            colorChangeCoroutine4 = StartCoroutine(CambiarColor4(valvulaRenderer.materials));
        }
        
        seguirPlayerCorrutina4 = StartCoroutine(SeguirPlayer4());
        disparoCorrutina4 = StartCoroutine(Disparo4());

        if (mostrarLog) { Debug.Log("[ChorroTargetController]: Estado 4 iniciado: Seguir al jugador y disparo."); }
    }

    public void StartEstado3() // Aturdimiento
    {
        estado1 = false;
        estaEstado3 = true;
        estaAturdido = true;
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
            yield return ControladorScripts.instance.WaitForUnpause(); // Espera si el juego est� pausado

            if (objetivosChorro.Count > 0)
            {
                Transform currentTarget = GetRandomTarget();

                while (Vector3.Distance(targetChorro.position, currentTarget.position) > 0.1f)
                {
                    yield return ControladorScripts.instance.WaitForUnpause(); // Detiene el movimiento si el juego est� pausado

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
            yield return ControladorScripts.instance.WaitForUnpause(); // Espera si el juego est� pausado
            yield return new WaitForSeconds(tiempoEntreDisparosEstado1);

            yield return ControladorScripts.instance.WaitForUnpause();
            abrirChorro = true;
            seguirTarget.CrearChorro();
            estaAturdido = false;
            if (mostrarLog) { Debug.Log("[ChorroTargetController]: Estado 1 - Disparando."); }

            yield return ControladorScripts.instance.WaitForUnpause();
            yield return new WaitForSeconds(tiempoDisparoQuietoEstado1);

            yield return ControladorScripts.instance.WaitForUnpause();
            abrirChorro = false;
            seguirTarget.DestruirChorro();
            if (mostrarLog) { Debug.Log("[ChorroTargetController]: Estado 1 - Deteniendo disparo."); }
        }
    }
    private IEnumerator SeguirPlayer()
    {
        while (true)
        {
            yield return ControladorScripts.instance.WaitForUnpause(); // Espera si el juego est� pausado

            float timer = 0f;
            while (timer < followDuration)
            {
                yield return ControladorScripts.instance.WaitForUnpause(); // Detiene el seguimiento si el juego est� pausado

                targetChorro.position = Vector3.Lerp(
                    targetChorro.position,
                    new Vector3(player.position.x + 0.5f, player.position.y, player.position.z),
                    followPlayerSpeed * Time.deltaTime
                );

                timer += Time.deltaTime;
                yield return null;
            }

            if (mostrarLog) { Debug.Log("[ChorroTargetController]: Estado 2 - Quieto despu�s de seguir al jugador."); }

            yield return ControladorScripts.instance.WaitForUnpause();
            yield return new WaitForSeconds(idleAfterFollowDuration);
        }
    }

    private IEnumerator Disparo2()
    {
        while (true)
        {
            yield return ControladorScripts.instance.WaitForUnpause(); // Espera si el juego est� pausado
            yield return new WaitForSeconds(tiempoEntreDisparos);

            yield return ControladorScripts.instance.WaitForUnpause();
            abrirChorro = true;
            seguirTarget.CrearChorro();
            estaAturdido = false;
            if (mostrarLog) { Debug.Log("[ChorroTargetController]: Estado 2 - Disparando."); }

            yield return ControladorScripts.instance.WaitForUnpause();
            yield return new WaitForSeconds(tiempoDisparoQuieto);

            yield return ControladorScripts.instance.WaitForUnpause();
            abrirChorro = false;
            seguirTarget.DestruirChorro();
            if (mostrarLog) { Debug.Log("[ChorroTargetController]: Estado 2 - Deteniendo disparo."); }
        }
    }

    private IEnumerator CambiarColor4(Material[] materials)
    {
        while (estaEstado3) // Puedes cambiar `estaEstado3` por la condici�n adecuada para tu estado
        {
            yield return ControladorScripts.instance.WaitForUnpause(); // Espera si el juego est� pausado

            // Cambiar el color de los materiales a rojo
            foreach (Material material in materials)
            {
                if (originalColors.ContainsKey(material))
                {
                    if (material.HasProperty("_BaseColor"))
                    {
                        material.SetColor("_BaseColor", Color.red);
                    }
                    else if (material.HasProperty("_Color"))
                    {
                        material.SetColor("_Color", Color.red);
                    }
                }
            }

            yield return ControladorScripts.instance.WaitForUnpause();
            yield return new WaitForSeconds(0.002f); // Pausa entre actualizaciones
        }
    }

    private IEnumerator SeguirPlayer4()
    {
        while (true)
        {
            yield return ControladorScripts.instance.WaitForUnpause(); // Espera si el juego est� pausado

            float timer = 0f;
            while (timer < followDuration4)
            {
                yield return ControladorScripts.instance.WaitForUnpause(); // Detiene el seguimiento si el juego est� pausado

                targetChorro.position = Vector3.Lerp(
                    targetChorro.position,
                    new Vector3(player.position.x + 0.5f, player.position.y, player.position.z),
                    followPlayerSpeed4 * Time.deltaTime
                );

                timer += Time.deltaTime;
                yield return null;
            }

            if (mostrarLog) { Debug.Log("[ChorroTargetController]: Estado 4 - Quieto despu�s de seguir al jugador."); }

            yield return ControladorScripts.instance.WaitForUnpause();
            yield return new WaitForSeconds(idleAfterFollowDuration4);
        }
    }

    private IEnumerator Disparo4()
    {
        while (true)
        {
            yield return ControladorScripts.instance.WaitForUnpause(); // Espera si el juego est� pausado
            yield return new WaitForSeconds(tiempoEntreDisparos4);

            yield return ControladorScripts.instance.WaitForUnpause();
            abrirChorro = true;
            seguirTarget.CrearChorro();
            estaAturdido = false;
            if (mostrarLog) { Debug.Log("[ChorroTargetController]: Estado 4 - Disparando."); }

            yield return ControladorScripts.instance.WaitForUnpause();
            yield return new WaitForSeconds(tiempoDisparoQuieto4);

            yield return ControladorScripts.instance.WaitForUnpause();
            abrirChorro = false;
            seguirTarget.DestruirChorro();
            if (mostrarLog) { Debug.Log("[ChorroTargetController]: Estado 4 - Deteniendo disparo."); }
        }
    }


    private IEnumerator RotacionesValvula()
    {
        while (true)
        {
            yield return ControladorScripts.instance.WaitForUnpause(); // Espera si el juego est� pausado

            Transform currentTarget = GetRandomTargetStun();
            Vector3 posicionDestino = currentTarget.position;

            while (Vector3.Distance(targetChorro.position, posicionDestino) > 0.1f)
            {
                yield return ControladorScripts.instance.WaitForUnpause(); // Detiene el movimiento si el juego est� pausado

                targetChorro.position = Vector3.MoveTowards(
                    targetChorro.position,
                    posicionDestino,
                    velocidadAturdimiento * Time.deltaTime
                );

                yield return null;
            }

            if (mostrarLog) Debug.Log($"[ChorroTargetController]: Movimiento completado hacia {currentTarget.name}.");

            yield return ControladorScripts.instance.WaitForUnpause();
            yield return new WaitForSeconds(0.00002f); // Pausa entre frames para "fluidez"
        }
    }
    private IEnumerator CambiarColorIntermitente(Material[] materials)
    {
        float elapsedTime = 0f;
        bool toggleColor = false;

        while (estaEstado3)
        {
            yield return ControladorScripts.instance.WaitForUnpause(); // Espera si el juego est� pausado

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

            yield return ControladorScripts.instance.WaitForUnpause();
            yield return new WaitForSeconds(0.002f); // Pausa entre frames. "Fluidez"
        }

        // Restaurar colores originales al salir del estado
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
        if (!estaAturdido)
        {
            if (mostrarLog) { Debug.Log("[ChorroTargetController]: Iniciando Estado 3 - Aturdimiento."); }
            CambiarEstado(Estado.Estado3);
        }
    }

    public void EmpezarEstado4()
    {
        if (mostrarLog) { Debug.Log("[ChorroTargetController]: Iniciando Estado 4."); }
        CambiarEstado(Estado.Estado4);
    }

    private IEnumerator CambiarEstados()
    {
        if (test)
        {
            yield break;
        }

        yield return ControladorScripts.instance.WaitForUnpause(); // Espera si el juego est� pausado
        yield return new WaitForSeconds(2f);

        while (Vector3.Distance(targetChorro.position, Vector3.zero) > 0.01f)
        {
            yield return ControladorScripts.instance.WaitForUnpause(); // Detiene el movimiento si el juego est� pausado

            targetChorro.position = Vector3.Lerp(
                targetChorro.position,
                Vector3.zero,
                velocidadRetorno * Time.deltaTime
            );

            yield return null; // Espera al siguiente frame
        }

        yield return ControladorScripts.instance.WaitForUnpause();
        yield return new WaitForSeconds(0.2f);

        if (!golpesValvula.fase2 && !test)
        {
            EmpezarEstado1();
            yield return null;
        }

        yield return ControladorScripts.instance.WaitForUnpause();
        yield return new WaitForSeconds(0.2f);

        if (golpesValvula.fase2 && !test)
        {
            EmpezarEstado2();
            yield return null;
        }

        yield return ControladorScripts.instance.WaitForUnpause();
        yield return new WaitForSeconds(0.2f);

        if (golpesValvula.fase3 && !test)
        {
            EmpezarEstado4();
            yield return null;
        }

        
    }

}
