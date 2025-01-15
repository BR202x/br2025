using UnityEngine;

public class ControladorScripts : MonoBehaviour
{
    public static ControladorScripts instance;

    [Header("Scripts Referenciados")]
    public PlayerMovement playerMovement;
    public CameraController cameraController;

    public Shield shield;
    private Vector3 storedVelocity;

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
            MostrarLog($"Shield {(estado ? "activado" : "desactivado")}");
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

        MostrarLog($"Movimiento del jugador {(estado ? "activado" : "desactivado")}");
    }

    private void MostrarLog(string mensaje)
    {
        Debug.Log($"[ControladorScripts]: {mensaje}");
    }

    public void Prueba()
    {
        MostrarLog("El método 'Prueba' fue llamado desde el singleton ControladorScripts.");
    }
}
