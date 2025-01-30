using UnityEngine;

// EDITADO: 29/01/2025
public class VelocidadCaminarAgua : MonoBehaviour
{
    [Header("depuracion")]
    [Tooltip("Activa o desactiva los mensajes de depuracion en este script")]
    public bool mostrarLog = false;

    #region Variables

    [Header("Referencias")]
    [Tooltip("Referencia al script PlayerMovement")]
    public PlayerMovement playerMovement;
    [Tooltip("Script que detecta si los pies estan en el agua")]
    public DeteccionAguaPies deteccionAguaPies;

    [Header("Configuracion de Velocidad")]
    [Tooltip("Velocidad al caminar en el agua")]
    public float velocidadAgua = 3f;
    private bool enAgua = false;
    private float velocidadNormal;

    #endregion

    private void Start()
    {
        if (mostrarLog) { Debug.Log($"[VelocidadCaminarAgua]: Componente del objeto {gameObject.name}"); }

        playerMovement = GameObject.Find("Player").GetComponent<PlayerMovement>();
        velocidadNormal = playerMovement.moveNormalSpeed;

        if (mostrarLog) { Debug.Log("[VelocidadCaminarAgua]: Velocidad normal guardada: " + velocidadNormal); }
    }

    private void Update()
    {
        if (deteccionAguaPies.playerPiesAgua)
        {
            if (!enAgua)
            {
                enAgua = true;
                velocidadNormal = playerMovement.moveNormalSpeed;

                if (mostrarLog) { Debug.Log("[VelocidadCaminarAgua]: Jugador entro al agua. Velocidad reducida a " + velocidadAgua); }
            }

            playerMovement.moveSpeed = velocidadAgua;
        }
        else
        {
            if (enAgua)
            {
                enAgua = false;
                playerMovement.moveSpeed = velocidadNormal;

                if (mostrarLog) { Debug.Log("[VelocidadCaminarAgua]: Jugador salio del agua. Velocidad restaurada a " + velocidadNormal); }
            }
        }
    }
}
