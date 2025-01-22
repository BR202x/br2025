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
    public GameObject canvasPuntero;

    #endregion

    void Start()
    {
        vidaActual = vida;
    }

    public void RecibirDano()
    {
        if (vidaActual > 0)
        {
            vidaActual--;

            if (mostrarLog) { Debug.Log($"[ControladorVidaEnemigo] {nombreEnemigo} recibi� dano. Vida actual: {vidaActual}/{vida}"); }

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

    public void ActivarPuntero()
    { 
        canvasPuntero.SetActive(true);
    }
}
