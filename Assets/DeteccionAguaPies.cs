using UnityEngine;

public class DeteccionAguaPies : MonoBehaviour
{
    #region Variables

    [Header("depuracion")]
    public bool mostrarDebug;

    [Header("Valores de Deteccion")]
    [Tooltip("Radio de la esfera, volumen para detectar el agua")]
    public float radioEsfera = 0.5f;

    [Tooltip("Capa del objeto NivelAgua, para detectar cuando el jugador este sumergido")]
    public LayerMask capaAgua;

    public bool playerPiesAgua;

    #endregion

    private void Update()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, radioEsfera, capaAgua);

        if (hits.Length > 0)
        {
            if (!playerPiesAgua)
            {
                playerPiesAgua = true;
                if (mostrarDebug) { Debug.Log("[PiesAgua]: Player esta tocando agua con pies."); }
            }
        }
        else
        {
            if (playerPiesAgua)
            {
                playerPiesAgua = false;
                if (mostrarDebug) { Debug.Log("[PiesAgua]: Player ya no esta tocando agua con pies."); }
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = playerPiesAgua ? Color.green : Color.cyan;
        Gizmos.DrawWireSphere(transform.position, radioEsfera);
    }
}
