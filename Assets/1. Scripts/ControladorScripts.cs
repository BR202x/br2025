using UnityEngine;

public class ControladorScripts : MonoBehaviour
{
#region Variables

    [Header("Depuracion")]
    public bool mostrarDebug = true;

    public static ControladorScripts instance;
    [Header("Scripts Referenciados")]
    public PlayerMovement playerMovement;
    public CameraController cameraController;
    public Shield shield;
    private Vector3 storedVelocity;

#endregion

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

    public void MovimientoJugador(bool estado)
    {
        Shield shield = FindFirstObjectByType<Shield>();

        if (shield != null)
        {
            shield.enabled = estado;
            if (mostrarDebug) Debug.Log($"[ControladorScripts] Shield {(estado ? "activado" : "desactivado")}");
        }

        if (!estado)
        {
            storedVelocity = playerMovement.rb.linearVelocity;
            playerMovement.rb.linearVelocity = Vector3.zero;
        }
        else
        {
            playerMovement.rb.linearVelocity = storedVelocity;
        }

        playerMovement.enabled = estado;
        playerMovement.GetComponentInChildren<Animator>().enabled = estado;

        cameraController.enabled = estado;

        if (mostrarDebug) Debug.Log($"[ControladorScripts] Movimiento del jugador {(estado ? "activado" : "desactivado")}");
    }

    public void Prueba()
    {
        if (mostrarDebug) Debug.Log("[ControladorScripts] El método 'Prueba' fue llamado desde el singleton ControladorScripts.");
    }
}
