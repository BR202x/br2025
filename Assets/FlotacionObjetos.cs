using UnityEditor.PackageManager.Requests;
using UnityEngine;

public class FlotacionObjetos : MonoBehaviour
{
    #region Variables

    public bool mostrarDebug;

    [Header("Configuración de Movimiento")]
    public Transform objetoAgua; // Objeto Agua cuya posición en Y seguirá el objeto
    public Transform posInicial; // Posición inicial a la que regresa el objeto
    public float velocidadFlotacion = 2f; // Velocidad del movimiento hacia la posición del agua
    public float velocidadRetorno = 1.5f; // Velocidad para regresar a la posición inicial
    public float velocidadPeso = 1.5f; // Velocidad para regresar a la posición inicial
    public Rigidbody rbObjeto;

    [Header("Configuración de Temporizador")]
    public float tiempoEsperaFlotacion = 2f; // Tiempo antes de iniciar el movimiento

    public bool dentroDelAgua;
    public bool moviendoHaciaArriba;
    public bool estaEnInicial = true;
    public bool estaEnFinal = false;
    public bool reiniciando = false;

    public bool jugadorParado = false;

    public float tiempoEnAgua;

    public float tiempoReinicio;
    public float tiempoEsperaReinicio = 2f;

    public float tiempoHundirse;
    public float tiempoEsperaHundirse = 3;



    public InicioLLenado inicioLlenado;
    public MoverJugadorPorTambor moverJugadorPorTambor;

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

        // Detectar si está dentro del agua y mover hacia el agua
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

            if (!moviendoHaciaArriba && (tiempoEnAgua >= tiempoEsperaFlotacion) && (posInicial.position.y <= objetoAgua.position.y) && estaEnInicial)
            {
                moviendoHaciaArriba = true;
                estaEnInicial = false;   

                DLog("Tiempo cumplido, iniciando movimiento para seguir al agua.");
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
        // Interpolación hacia la posición del objeto agua en Y
        Vector3 posicionActual = transform.position;
        Vector3 posicionObjetivo = new Vector3(posicionActual.x, objetoAgua.position.y, posicionActual.z);

        // Movimiento suave
        transform.position = Vector3.Lerp(posicionActual, posicionObjetivo, velocidadFlotacion * Time.deltaTime);

        // Verificar si está cerca de la posición del agua
        if (Mathf.Abs(transform.position.y - objetoAgua.position.y) < 0.15f)
        {            
            DLog("Objeto está alineado con la posición del agua.");            
            estaEnFinal  = true;
        }        
    }

    private void MoverHaciaPosInicial()
    {
        // Movimiento constante hacia la posición inicial en Y
        Vector3 posicionActual = transform.position;
        Vector3 posicionObjetivo = new Vector3(posicionActual.x, posInicial.position.y, posicionActual.z);

        // Movimiento lineal
        transform.position = Vector3.MoveTowards(posicionActual, posicionObjetivo, velocidadRetorno * Time.deltaTime);

        // Verificar si ya alcanzó la posición inicial
        if (Mathf.Abs(transform.position.y - posInicial.position.y) < 0.01f)
        {
            moviendoHaciaArriba = false; // Detener cualquier movimiento
            rbObjeto.isKinematic = false;
            estaEnInicial = true;
            dentroDelAgua = false;
            tiempoHundirse = 0;
            DLog("Objeto volvió a la posición inicial.");
        }
    }

    private void MoverHaciaPosInicialPeso()
    {
        // Movimiento constante hacia la posición inicial en Y
        Vector3 posicionActual = transform.position;
        Vector3 posicionObjetivo = new Vector3(posicionActual.x, posInicial.position.y, posicionActual.z);

        // Movimiento lineal
        transform.position = Vector3.MoveTowards(posicionActual, posicionObjetivo, velocidadPeso * Time.deltaTime);

        // Verificar si ya alcanzó la posición inicial
        if (Mathf.Abs(transform.position.y - posInicial.position.y) < 0.01f)
        {
            moviendoHaciaArriba = false; // Detener cualquier movimiento
            rbObjeto.isKinematic = false;
            estaEnInicial = true;
            dentroDelAgua = false;
            DLog("Objeto volvió a la posición inicial.");
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Agua") && !dentroDelAgua)
        {
            tiempoEnAgua = 0f; // Reiniciar el temporizador al entrar al agua
            DLog("Entrando en el agua, iniciando temporizador.");
            dentroDelAgua = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Agua"))
        {
            dentroDelAgua = true;
            DLog("Dentro del agua, verificando tiempo.");
        }
    }


    private void DLog(string texto)
    {
        if (mostrarDebug)
        {
            Debug.Log($"[FlotacionObjetos]: {texto}");
        }
    }
}
