using UnityEngine;

public class ControladorVidaEnemigo : MonoBehaviour
{
    [Header("Depuración")]
    public bool mostrarLog = true;

    #region Variables

    [Header("Variables Vida")]
    public int vida = 2;
    public int vidaActual;

    [Header("Información del Enemigo")]
    public string nombreEnemigo;

    #endregion

    void Start()
    {
        vidaActual = vida;
    }

    public void RecibirDaño()
    {
        if (vidaActual > 0)
        {
            vidaActual--;

            if (mostrarLog) { Debug.Log($"[ControladorVidaEnemigo] {nombreEnemigo} recibió daño. Vida actual: {vidaActual}/{vida}"); }

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
