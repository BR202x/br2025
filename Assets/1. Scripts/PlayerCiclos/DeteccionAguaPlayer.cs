using UnityEngine;

public class DeteccionAguaPlayer : MonoBehaviour
{
    #region Variables

    [Header("depuracion")]
    public bool mostrarDebug;

    [Header("Valores de Deteccion")]
    [Tooltip("Radio de la esfera, volumen para detectar el agua")]
    public float radioEsfera = 0.5f;

    [Tooltip("Capa del objeto NivelAgua, para detectar cuando el jugador este sumergido")]
    public LayerMask capaAgua;
        
    public bool playerCabezaAgua;

    #endregion

    private void Update()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, radioEsfera, capaAgua);

        if (hits.Length > 0 )
        {
            if (!playerCabezaAgua)
            {
                playerCabezaAgua = true;
                if (mostrarDebug) { Debug.Log("[CabezaAgua]: Player esta tocando agua con cabeza."); }
            }
        }
        else
        {
            if (playerCabezaAgua)
            {
                playerCabezaAgua = false;
                if (mostrarDebug) { Debug.Log("[CabezaAgua]: Player ya no esta tocando agua con cabeza."); }
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = playerCabezaAgua ? Color.green : Color.cyan;
        Gizmos.DrawWireSphere(transform.position, radioEsfera);
    }
}
