using UnityEngine;

public class ControladorAtaque : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Puedes agregar l�gica de inicializaci�n si es necesario
    }

    void Update()
    {
        Shield shieldObject = FindFirstObjectByType<Shield>();
        if (shieldObject != null)
        {
            // Agregar un script adicional si no est� presente
            if (!shieldObject.gameObject.TryGetComponent<ColisionEscudo>(out _))
            {
                shieldObject.gameObject.AddComponent<ColisionEscudo>();
                Debug.Log("Se ha agregado el script ShieldCollisionLogger al prefab Shield.");
            }
        }
        else
        {
            Debug.LogWarning("No se encontr� un objeto llamado Shield en la escena.");
        }
    }
}
