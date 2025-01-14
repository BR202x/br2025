using UnityEngine;

public class CambioPadreObjetos : MonoBehaviour
{
#region Variables

    [Header("Configuración del Raycast")]
        [Tooltip("Distancia máxima del raycast para detectar objetos")]
    public float distanciaRaycast = 2f;
        [Tooltip("Capa de la Superficie - Para ser Hijo al hacer Raycast")]
    public LayerMask layerObjetivo;

#endregion

    private void Update()
    {        
        Ray ray = new Ray(transform.position, Vector3.down);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, distanciaRaycast, layerObjetivo))
        {         
            transform.SetParent(hit.collider.transform);
        }
        else
        {     
            transform.SetParent(null);
        }

        // Debug para visualizar el raycast en la escena
        Debug.DrawRay(transform.position, Vector3.down * distanciaRaycast, Color.green);
    }

    private void OnDrawGizmos()
    {        
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * distanciaRaycast);
        Gizmos.DrawWireSphere(transform.position + Vector3.down * distanciaRaycast, 0.1f);
    }
}
