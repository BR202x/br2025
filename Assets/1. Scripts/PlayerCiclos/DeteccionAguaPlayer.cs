using UnityEngine;

public class DeteccionAguaPlayer : MonoBehaviour
{
#region Variables

    public bool mostrarDebug; // Controla si se mostrarán los mensajes de depuración en la consola
    [Space]
    public bool playerTocandoAgua;    
    [Header("Valores de Deteccion")]
        [Tooltip("Radio de la Esfera, volumen para detectar el Agua")]
    public float radioEsfera = 0.5f;
        [Tooltip("Capa del objeto NivelAgua, para detectar cuando el Jugador este sumergido")]
    public LayerMask capaAgua;

#endregion

    void Update()
    {        
        Collider[] hits = Physics.OverlapSphere(transform.position, radioEsfera, capaAgua); // 2do intento de Raycast - Esfera
                
        if (hits.Length > 0)
        {
            if (!playerTocandoAgua)
            {
                playerTocandoAgua = true;
                DLog("Player está tocando agua.");
            }
        }
        else
        {
            if (playerTocandoAgua)
            {
                playerTocandoAgua = false;
                DLog("Player ya no está tocando agua.");
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = playerTocandoAgua ? Color.green : Color.cyan;
        Gizmos.DrawWireSphere(transform.position, radioEsfera);
    }

    private void DLog(string mensaje)
    {
        if (mostrarDebug)
        {
            Debug.Log($"[DeteccionAguaPlayer]: {mensaje}");
        }
    }
}
