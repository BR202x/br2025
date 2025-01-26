using System.Collections;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

public class FlotacionObjetos : MonoBehaviour
{
    [Header("depuracion")]
    public bool mostrarDebug;

    #region Variables

    [Header("Configuración de Movimiento")]
    public Transform objetoAgua;
    public Transform posInicial;
    public float velocidadFlotacion = 2f;
    public float velocidadRetorno = 1.5f;
    public float velocidadPeso = 1.5f;

    [Header("Configuración de Temporizador")]
    public float tiempoEsperaFlotacion = 2f;
    public float tiempoEsperaReinicio = 2f;
    public float tiempoEsperaHundirse = 3f;

    private bool dentroDelAgua;
    private bool moviendoHaciaArriba;
    private bool estaEnInicial = true;
    public bool estaEnFinal = false;
    private bool reiniciando = false;
    public bool jugadorParado = false;

    [Header("Configuración de Hundimiento")]
    public float limiteHundirse = 5f; // Tiempo de cuenta regresiva antes de reiniciar tiempoHundirse
    public float contadorHundimiento = 0f; // Contador independiente para la cuenta regresiva
    public float fuerzaEmpuje = 1;
    public bool isOnTop = false;

    public float tiempoEnAgua;
    public float tiempoReinicio;
    public float tiempoHundirse;

    private InicioLLenado inicioLlenado;
    private MoverJugadorPorTambor moverJugadorPorTambor;
    private Rigidbody rbObjeto;

    #endregion

    private void Start()
    {
        rbObjeto = GetComponent<Rigidbody>();
        inicioLlenado = GameObject.Find("LlenadoDeAguaManager").GetComponent<InicioLLenado>();
        moverJugadorPorTambor = GameObject.Find("Player").GetComponent<MoverJugadorPorTambor>();
    }

    private void Update()
    {
        if (Mathf.Abs(transform.position.y - objetoAgua.position.y) > 0.1f)
        {
            estaEnFinal = false;
        }
        
        if (jugadorParado && estaEnFinal && !isOnTop) // Si el jugador está parado y aún no se ha ejecutado el efecto
        {
            StartCoroutine(SimularMovimientoHaciaAbajo()); // Ejecutar el movimiento hacia abajo
            isOnTop = true; // Marcar que el efecto ya se ejecutó
        }

        // Cuando el jugador deja de estar parado, reseteamos el flag
        if (!jugadorParado)
        {
            isOnTop = false; // Permitir que el efecto se vuelva a ejecutar en el futuro
        }

        if (estaEnFinal && contadorHundimiento > 0f && tiempoHundirse >= 3 && !jugadorParado)
        {
            contadorHundimiento -= Time.deltaTime;

            // Si el contador llega a 0, reiniciar tiempoHundirse
            if (contadorHundimiento <= 0f)
            {
                contadorHundimiento = 0f; // Asegurarse de que no sea negativo
                tiempoHundirse = 0f;                

                if (mostrarDebug) { Debug.Log("Contador completado, tiempoHundirse reiniciado."); }
            }
        }

        if (dentroDelAgua && inicioLlenado.estaLlenando && !jugadorParado)
        {
            if (moviendoHaciaArriba)
            {
                rbObjeto.isKinematic = true;
                MoverSeguirAgua();
            }

            if (tiempoEnAgua <= tiempoEsperaFlotacion && !reiniciando)
            {
                tiempoEnAgua += Time.deltaTime;
            }

            if (!moviendoHaciaArriba && tiempoEnAgua >= tiempoEsperaFlotacion && posInicial.position.y <= objetoAgua.position.y && estaEnInicial)
            {
                moviendoHaciaArriba = true;
                estaEnInicial = false;

                if (mostrarDebug) { Debug.Log("Tiempo cumplido, iniciando movimiento para seguir al agua."); }
            }
        }
        else if (!inicioLlenado.estaLlenando && dentroDelAgua && !estaEnInicial)
        {
            MoverHaciaPosInicial();
        }

        if (inicioLlenado.estaLlenando && dentroDelAgua && !estaEnInicial && jugadorParado && !estaEnFinal)
        {
            moviendoHaciaArriba = false;
            reiniciando = true;
            MoverHaciaPosInicialPeso();
        }

        if (inicioLlenado.estaLlenando && dentroDelAgua && !estaEnInicial && jugadorParado && estaEnFinal)
        {
            if (tiempoHundirse <= tiempoEsperaHundirse)
            {
                tiempoHundirse += Time.deltaTime;
            }

            if (tiempoHundirse >= tiempoEsperaHundirse)
            {
                moviendoHaciaArriba = false;
                reiniciando = true;
                MoverHaciaPosInicialPeso();
            }
        }

        if (inicioLlenado.estaLlenando && dentroDelAgua && !estaEnInicial && !jugadorParado && !moviendoHaciaArriba)
        {
            moviendoHaciaArriba = true;
        }

        if (!jugadorParado && inicioLlenado.estaLlenando && dentroDelAgua && !moviendoHaciaArriba && estaEnInicial)
        {
            if (tiempoReinicio <= tiempoEsperaReinicio)
            {
                tiempoReinicio += Time.deltaTime;

                if (tiempoReinicio >= tiempoEsperaReinicio)
                {
                    tiempoEnAgua = 0f;
                    tiempoReinicio = 0f;
                    tiempoHundirse = 0f;
                    estaEnInicial = true;
                    reiniciando = false;
                }
            }
        }
    }

    private void MoverSeguirAgua()
    {
        Vector3 posicionActual = transform.position;
        Vector3 posicionObjetivo = new Vector3(posicionActual.x, objetoAgua.position.y, posicionActual.z);

        transform.position = Vector3.Lerp(posicionActual, posicionObjetivo, velocidadFlotacion * Time.deltaTime);

        if (Mathf.Abs(transform.position.y - objetoAgua.position.y) < 0.15f)
        {
            if (mostrarDebug) { Debug.Log("Objeto está alineado con la posición del agua."); }
            estaEnFinal = true;

            // Iniciar el contador solo si está en 0
            if (contadorHundimiento == 0f)
            {
                contadorHundimiento = limiteHundirse;
            }
        }
    }

    private void MoverHaciaPosInicial()
    {
        Vector3 posicionActual = transform.position;
        Vector3 posicionObjetivo = new Vector3(posicionActual.x, posInicial.position.y, posicionActual.z);

        transform.position = Vector3.MoveTowards(posicionActual, posicionObjetivo, velocidadRetorno * Time.deltaTime);

        if (Mathf.Abs(transform.position.y - posInicial.position.y) < 0.01f)
        {
            moviendoHaciaArriba = false;
            rbObjeto.isKinematic = false;
            estaEnInicial = true;
            dentroDelAgua = false;
            tiempoHundirse = 0;

            if (mostrarDebug) { Debug.Log("Objeto volvió a la posición inicial."); }
        }
    }

    private void MoverHaciaPosInicialPeso()
    {
        Vector3 posicionActual = transform.position;
        Vector3 posicionObjetivo = new Vector3(posicionActual.x, posInicial.position.y, posicionActual.z);

        transform.position = Vector3.MoveTowards(posicionActual, posicionObjetivo, velocidadPeso * Time.deltaTime);

        if (Mathf.Abs(transform.position.y - posInicial.position.y) < 0.01f)
        {
            moviendoHaciaArriba = false;
            rbObjeto.isKinematic = false;
            estaEnInicial = true;
            dentroDelAgua = false;

            if (mostrarDebug) { Debug.Log("Objeto volvió a la posición inicial."); }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Agua") && !dentroDelAgua)
        {
            tiempoEnAgua = 0f;
            if (mostrarDebug) { Debug.Log("Entrando en el agua, iniciando temporizador."); }
            dentroDelAgua = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Agua"))
        {
            dentroDelAgua = true;
            if (mostrarDebug) { Debug.Log("Dentro del agua, verificando tiempo."); }
        }
    }

    private IEnumerator SimularMovimientoHaciaAbajo()
    {
        estaEnFinal = true;

        Vector3 posicionInicial = transform.position;     
        Vector3 posicionHaciaAbajo = new Vector3(posicionInicial.x, posicionInicial.y - 0.1f, posicionInicial.z);
                
        float tiempo = 0f;
        while (tiempo < 0.2f) // Baja en 0.2 segundos
        {
            tiempo += Time.deltaTime;
            transform.position = Vector3.Lerp(posicionInicial, posicionHaciaAbajo, tiempo / 0.2f);
            yield return null;
        }
                
        tiempo = 0f;
        while (tiempo < 0.2f) // Sube en 0.2 segundos
        {
            tiempo += Time.deltaTime;
            transform.position = Vector3.Lerp(posicionHaciaAbajo, posicionInicial, tiempo / 0.2f);
            yield return null;
        }

        estaEnFinal = true;
    }
}
