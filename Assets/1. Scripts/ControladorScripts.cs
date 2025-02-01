using System.Collections;
using UnityEngine;

public class ControladorScripts : MonoBehaviour
{
    #region Variables

    [Header("Depuración")]
    public bool mostrarDebug = true;
    public static ControladorScripts instance;

    [Header("Scripts Referenciados")]
    public PlayerMovement playerMovement;
    public CameraController cameraController;
    public MoverJugadorPorTambor moverJugadorTambor;
    public Animator animJugador;
    public InicioLLenado inicioLlenado;
    public LlenadoManager llenadoManager;
    public ChorroTargetController chorroTargetController;
    public RotacionSuperficieController rotacionSuperficieController;
    public SeguirTarget seguirTarget;

    public Shield shield;
    private Vector3 storedVelocity;

    private bool isPaused = false; // Variable para saber si el juego está pausado

    #endregion

    private void OnEnable()
    {
        playerMovement = GameObject.Find("Player").GetComponent<PlayerMovement>();
        cameraController = GameObject.Find("Player").GetComponent<CameraController>();
        moverJugadorTambor = GameObject.Find("Player").GetComponent<MoverJugadorPorTambor>();
        animJugador = GameObject.Find("RATON  JACK").GetComponent<Animator>();
        inicioLlenado = GameObject.Find("LlenadoDeAguaManager").GetComponent<InicioLLenado>();
        llenadoManager = GameObject.Find("LlenadoDeAguaManager").GetComponent<LlenadoManager>();
        chorroTargetController = GameObject.Find("Chorro_Manager").GetComponent<ChorroTargetController>();
        rotacionSuperficieController = GameObject.Find("RotacionManager").GetComponent<RotacionSuperficieController>();
        seguirTarget = GameObject.Find("Valvula").GetComponent<SeguirTarget>();
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
        
    public void PausarJuego()
    {
        isPaused = true;
        MovimientoJugador(false);
        if (mostrarDebug) Debug.Log("[ControladorScripts] Juego Pausado.");
    }

    
    public void ReanudarJuego()
    {
        isPaused = false;
        MovimientoJugador(true); 
        if (mostrarDebug) Debug.Log("[ControladorScripts] Juego Reanudado.");
    }

    
    public IEnumerator WaitForUnpause()
    {
        while (isPaused)
        {
            yield return null;
        }
    }

    public void MovimientoJugador(bool estado)
    {
        Shield shield = FindFirstObjectByType<Shield>();
                
        if (shield != null)
        {
            shield.enabled = estado;
            if (mostrarDebug) Debug.Log($"[ControladorScripts] Shield {(estado ? "activado" : "desactivado")}");
        }
                
        if (playerMovement != null)
        {
            if (!estado)
            {        
                storedVelocity = playerMovement.rb.linearVelocity;
                playerMovement.rb.linearVelocity = Vector3.zero;
                playerMovement.rb.constraints = RigidbodyConstraints.FreezeAll;
            }
            else
            {                
                playerMovement.rb.constraints = RigidbodyConstraints.FreezeRotation;
                playerMovement.rb.linearVelocity = storedVelocity;
            }
        }
                
        if (playerMovement != null) playerMovement.enabled = estado;
        if (playerMovement != null) playerMovement.GetComponentInChildren<Animator>().enabled = estado;
        if (cameraController != null) cameraController.enabled = estado;
        if (moverJugadorTambor != null) moverJugadorTambor.enabled = estado;
        if (inicioLlenado != null) inicioLlenado.enabled = estado;
        if (llenadoManager != null) llenadoManager.enabled = estado;
        if (chorroTargetController != null) chorroTargetController.enabled = estado;
        if (seguirTarget != null) seguirTarget.enabled = estado;
        if (animJugador != null) animJugador.enabled = estado;

        if (mostrarDebug) Debug.Log($"[ControladorScripts] Movimiento del jugador {(estado ? "activado" : "desactivado")}");
    }

}
