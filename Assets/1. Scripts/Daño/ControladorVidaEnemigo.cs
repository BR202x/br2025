using UnityEngine;

public class ControladorVidaEnemigo : MonoBehaviour
{
    [Header("Depuraci�n")]
    public bool mostrarLog = true;

    #region Variables

    [Header("Variables Vida")]
    public int vida = 2;
    public int vidaActual;

    [Header("Informaci�n del Enemigo")]
    public string nombreEnemigo;

    #endregion

    void Start()
    {
        vidaActual = vida;
    }

    public void RecibirDa�o()
    {
        if (vidaActual > 0)
        {
            vidaActual--;

            if (mostrarLog) { Debug.Log($"[ControladorVidaEnemigo] {nombreEnemigo} recibi� da�o. Vida actual: {vidaActual}/{vida}"); }

            if (vidaActual <= 0)
            {
                if (mostrarLog) { Debug.Log($"[ControladorVidaEnemigo] {nombreEnemigo} ha sido derrotado."); }
                gameObject.SetActive(false);
            }
        }
        else
        {
            if (mostrarLog) { Debug.Log($"[ControladorVidaEnemigo] {nombreEnemigo} ya no tiene vidas."); }
        }
    }
}
