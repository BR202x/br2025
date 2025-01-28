using UnityEngine;

public class VelocidadCaminarAgua : MonoBehaviour
{
    public PlayerMovement playerMovement; // Referencia al script PlayerMovement
    public DeteccionAguaPies deteccionAguaPies; // Script que detecta si los pies están en el agua
    public float velocidadAgua = 3f; // Velocidad al caminar en el agua
    private bool enAgua = false; // Bandera que indica si está en el agua
    private float velocidadNormal; // Velocidad original del PlayerMovement

    void Start()
    {
        // Encontrar la referencia al PlayerMovement y guardar su velocidad normal
        playerMovement = GameObject.Find("Player").GetComponent<PlayerMovement>();
        velocidadNormal = playerMovement.moveNormalSpeed; // Guardamos la velocidad normal predeterminada
    }

    void Update()
    {
        // Comprobar si el jugador está en el agua
        if (deteccionAguaPies.playerPiesAgua)
        {
            // Si está en el agua, sobrescribir siempre la velocidad
            if (!enAgua) // Solo entra aquí cuando cambia de estado (entra al agua)
            {
                enAgua = true;
                velocidadNormal = playerMovement.moveNormalSpeed; // Guardamos la velocidad actual al entrar al agua
            }

            playerMovement.moveSpeed = velocidadAgua; // Sobrescribimos la velocidad siempre que esté en el agua
        }
        else
        {
            // Si está fuera del agua
            if (enAgua) // Solo entra aquí cuando cambia de estado (sale del agua)
            {
                enAgua = false;
                playerMovement.moveSpeed = velocidadNormal; // Restauramos la velocidad al salir del agua
            }
        }
    }
}
