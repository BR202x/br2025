using UnityEngine;

public class ControladorAtaque : MonoBehaviour
{
    [Header("Depuración")]
    public bool mostrarLog;

    void Update()
    {
        Shield shieldObject = FindFirstObjectByType<Shield>();
        if (shieldObject != null)
        {
            if (!shieldObject.gameObject.TryGetComponent<ColisionEscudo>(out _))
            {
                shieldObject.gameObject.AddComponent<ColisionEscudo>();
                if (mostrarLog) { Debug.Log("Se ha agregado el script ShieldCollisionLogger al prefab Shield."); }
            }
        }
        else
        {
            if (mostrarLog) { Debug.LogWarning("No se encontró un objeto llamado Shield en la escena."); }
        }
    }
}
