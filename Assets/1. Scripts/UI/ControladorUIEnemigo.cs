using UnityEngine;
using TMPro;

public class ControladorUIEnemigo : MonoBehaviour
{
    [Header("Depuración")]
    public bool mostrarLog;

    [Header("Referencias de UI")]
    public TextMeshProUGUI textoNombreEnemigo;
    public Transform panelVidaEnemigo;
    public GameObject prefabVida;

    public void ActualizarUI(ControladorVidaEnemigo enemigo)
    {
        if (enemigo == null)
        {
            if (mostrarLog) { Debug.LogWarning("[ControladorUIEnemigo] El enemigo es null, no se puede actualizar la UI."); }
            return;
        }

        textoNombreEnemigo.text = enemigo.nombreEnemigo;

        foreach (Transform child in panelVidaEnemigo)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < enemigo.vidaActual; i++)
        {
            Instantiate(prefabVida, panelVidaEnemigo);
        }

        if (mostrarLog) { Debug.Log($"[ControladorUIEnemigo] UI actualizada para {enemigo.nombreEnemigo} con {enemigo.vidaActual} vidas."); }
    }
}
