using UnityEngine;

// EDITADO: 29/01/2025
public class MostrarDebugController : MonoBehaviour
{
    #region Variables

    [Header("Referencias de scripts")]
    private AccionesMateriales aMateriales;
    private CaminarDeteccionMaterial cDeteccionMaterial;
    private ColisionEscudo cEscudo;
    private DeteccionAguaPies dAguaPies;
    private DeteccionAguaPlayer dAguaPlayer;
    private InstanciaNewChorro iNewChorro;
    private MoverJugadorPorTambor mJugadorTambor;
    private MoverObjetosTambor mObjetosTambor;
    private ChorroTargetController cTargetController;
    private ControladorGolpeValvula golpeValvula;
    private FlotacionObjetos flotObjetos;
    private InstanciaChorroEscudo iChorroEscudo;
    private SeguirTarget sTarget;
    private VelocidadCaminarAgua vCaminarAgua;

    [Header("Ver Mensajes de Depuracion - Lista Scripts")]
    [Space]
    [Header("Jugador")]
    [Tooltip("ColisionEscudo: Script agregado al Escudo para detectar colision")]
    public bool colisionEscudo = false;
    [Tooltip("DeteccionAguaPies: Detecta por SphereOverlap el objeto Agua en los Pies")]
    public bool deteccionAguaPies = false;
    [Tooltip("DeteccionAguaPlayer: Detecta por SphereOverlap el objeto Agua en la Cabeza")]
    public bool deteccionAguaPlayer = false;
    [Tooltip("MoverJugadorPorTambor: Aplica fuerza centrifuga al jugador por la rotacion")]
    public bool moverJugadorPorTambor = false;
    [Tooltip("VelocidadCaminarAgua: Cambia la velocidad de movimiento si esta en el agua")]
    public bool velocidadCaminarAgua = false;
    [Space]
    [Header("CañonAgua")]
    [Tooltip("ChorroTargetController: Encargado de la secuencia del chorro y rotacion del tambor")]
    public bool chorroTargetController = false;
    [Tooltip("InstanciaNewChorro: Crea el Chorro segun un prefab y un LineRenderer")]
    public bool instanciaNewChorro = false;
    [Tooltip("ControladorGolpeValvula: Registra las colisiones con el Canon de Agua")]
    public bool controlGolpeValvula = false;
    [Tooltip("InstanciaChorroEscudo: Instancia el rebote del chorro")]
    public bool instanciaChorroEscudo = false;
    [Tooltip("SeguirTarget: Instancia el linerenderer Prefab que crea el Chorro")]
    public bool seguirTarget = false;
    [Space]
    [Header("Sonido")]
    [Tooltip("AccionesMateriales: Asigna parametros segun materiales FMOD - Audio")]
    public bool accionesMateriales = false;
    [Tooltip("CaminarDeteccionMateriales: Detecta los materiales segun un Raycast")]
    public bool caminarDeteccionMaterial = false;
    [Space]
    [Header("Escenario")]
    [Tooltip("MoverObjetosPorTambor: Aplica fuerza centrifuga y centripeta a los objetos por la rotacion")]
    public bool moverObjetosTambor = false;
    [Tooltip("FlotacionObjetos: Encargado del comportamiento de los objetos en el agua")]
    public bool flotacionObjetos = false;

    #endregion

    private void Start()
    {           
        aMateriales = Object.FindFirstObjectByType<AccionesMateriales>();
        cDeteccionMaterial = Object.FindFirstObjectByType<CaminarDeteccionMaterial>();
        cEscudo = Object.FindFirstObjectByType<ColisionEscudo>();
        dAguaPies = Object.FindFirstObjectByType<DeteccionAguaPies>();
        dAguaPlayer = Object.FindFirstObjectByType<DeteccionAguaPlayer>();
        iNewChorro = Object.FindFirstObjectByType<InstanciaNewChorro>();
        mJugadorTambor = Object.FindFirstObjectByType<MoverJugadorPorTambor>();
        mObjetosTambor = Object.FindFirstObjectByType<MoverObjetosTambor>();
        cTargetController = Object.FindFirstObjectByType<ChorroTargetController>();
        golpeValvula = Object.FindFirstObjectByType<ControladorGolpeValvula>();
        flotObjetos = Object.FindFirstObjectByType<FlotacionObjetos>();
        iChorroEscudo = Object.FindFirstObjectByType<InstanciaChorroEscudo>();
        sTarget = Object.FindFirstObjectByType<SeguirTarget>();
        vCaminarAgua = Object.FindFirstObjectByType<VelocidadCaminarAgua>();
    }

    private void Update()
    {
        accionesMateriales = aMateriales ? aMateriales.mostrarDebug : false;
        caminarDeteccionMaterial = cDeteccionMaterial ? cDeteccionMaterial.mostrarDebug : false;
        colisionEscudo = cEscudo ? cEscudo.mostrarLog : false;
        deteccionAguaPies = dAguaPies ? dAguaPies.mostrarDebug : false;
        deteccionAguaPlayer = dAguaPlayer ? dAguaPlayer.mostrarDebug : false;
        instanciaNewChorro = iNewChorro ? iNewChorro.mostrarLog : false;
        moverJugadorPorTambor = mJugadorTambor ? mJugadorTambor.mostrarDebug : false;
        moverObjetosTambor = mObjetosTambor ? mObjetosTambor.mostrarDebug : false;
        chorroTargetController = cTargetController ? cTargetController.mostrarLog : false;
        controlGolpeValvula = golpeValvula ? golpeValvula.mostrarLog : false;
        flotacionObjetos = flotObjetos ? flotObjetos.mostrarDebug : false;
        instanciaChorroEscudo = iChorroEscudo ? iChorroEscudo.mostrarLog : false;
        seguirTarget = sTarget ? sTarget.mostrarLog : false;
        velocidadCaminarAgua = vCaminarAgua ? vCaminarAgua.mostrarLog : false;
    }
}
