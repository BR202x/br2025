using UnityEngine;
using TMPro;

public class ControladorUIEnemigo : MonoBehaviour
{
    [Header("Referencias de UI")]
    public TextMeshProUGUI textoNombreEnemigo;
    public Transform panelVidaEnemigo;
    public GameObject prefabVida;

    public void ActualizarUI(ControladorVidaEnemigo enemigo)
    {
        if (enemigo == null)
        {
            Debug.LogWarning("[ControladorUIEnemigo] El enemigo es null, no se puede actualizar la UI.");
            return;
        }

        // Actualizar el nombre del enemigo
        textoNombreEnemigo.text = enemigo.nombreEnemigo;

        // Actualizar las vidas visibles en la UI
        foreach (Transform child in panelVidaEnemigo)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < enemigo.vidaActual; i++)
        {
            Instantiate(prefabVida, panelVidaEnemigo);
        }

        Debug.Log($"[ControladorUIEnemigo] UI actualizada para {enemigo.nombreEnemigo} con {enemigo.vidaActual} vidas.");
    }
}
