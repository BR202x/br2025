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
    private MoverJugadorPorTambor mJugadorTambor;
    private MoverObjetosTambor mObjetosTambor;
    private ChorroTargetController cTargetController;
    private ControladorGolpeValvula golpeValvula;
    private FlotacionObjetos flotObjetos;    
    private SeguirTarget sTarget;
    private VelocidadCaminarAgua vCaminarAgua;
    private AudioImp audioImp;

    private InstanciaNewChorro iNewChorro;
    private InstanciaChorroEscudo iChorroEscudo;

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
    [Tooltip("AudioImp: Implementacion de Audio Custom")]
    public bool audioImplementacion = false;
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
        mJugadorTambor = Object.FindFirstObjectByType<MoverJugadorPorTambor>();
        mObjetosTambor = Object.FindFirstObjectByType<MoverObjetosTambor>();
        cTargetController = Object.FindFirstObjectByType<ChorroTargetController>();
        golpeValvula = Object.FindFirstObjectByType<ControladorGolpeValvula>();
        flotObjetos = Object.FindFirstObjectByType<FlotacionObjetos>();        
        sTarget = Object.FindFirstObjectByType<SeguirTarget>();
        vCaminarAgua = Object.FindFirstObjectByType<VelocidadCaminarAgua>();
        audioImp = Object.FindFirstObjectByType <AudioImp>();
    }

    private void Update()
    {
        iNewChorro = Object.FindFirstObjectByType<InstanciaNewChorro>();
        iChorroEscudo = Object.FindFirstObjectByType<InstanciaChorroEscudo>();


        if (cEscudo) cEscudo.mostrarLog = colisionEscudo;
        if (dAguaPies) dAguaPies.mostrarDebug = deteccionAguaPies;
        if (dAguaPlayer) dAguaPlayer.mostrarDebug = deteccionAguaPlayer;
        if (mJugadorTambor) mJugadorTambor.mostrarDebug = moverJugadorPorTambor;
        if (vCaminarAgua) vCaminarAgua.mostrarLog = velocidadCaminarAgua;
        if (cTargetController) cTargetController.mostrarLog = chorroTargetController;
        if (iNewChorro) iNewChorro.mostrarLog = instanciaNewChorro;
        if (golpeValvula) golpeValvula.mostrarLog = controlGolpeValvula;
        if (iChorroEscudo) iChorroEscudo.mostrarLog = instanciaChorroEscudo;
        if (sTarget) sTarget.mostrarLog = seguirTarget;
        if (aMateriales) aMateriales.mostrarDebug = accionesMateriales;
        if (cDeteccionMaterial) cDeteccionMaterial.mostrarDebug = caminarDeteccionMaterial;
        if (mObjetosTambor) mObjetosTambor.mostrarDebug = moverObjetosTambor;
        if (flotObjetos) flotObjetos.mostrarDebug = flotacionObjetos;
        if (audioImp) audioImp.mostrarLog = audioImplementacion;
    }
}
