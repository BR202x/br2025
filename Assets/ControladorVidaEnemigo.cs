using UnityEngine;

public class ControladorVidaEnemigo : MonoBehaviour
{
    [Header("Depuración")]
    public bool mostrarDebug = true;

    [Header("Variables Vida")]
    public int vida = 2;
    public int vidaActual;

    [Header("Información del Enemigo")]
    public string nombreEnemigo;

    void Start()
    {
        vidaActual = vida;
    }

    public void RecibirDaño()
    {
        if (vidaActual > 0)
        {
            vidaActual--;

            if (mostrarDebug)
            {
                Debug.Log($"[ControladorVidaEnemigo] {nombreEnemigo} recibió daño. Vida actual: {vidaActual}/{vida}");
            }

            if (vidaActual <= 0)
            {
                if (mostrarDebug)
                {
                    Debug.Log($"[ControladorVidaEnemigo] {nombreEnemigo} ha sido derrotado.");
                }
                gameObject.SetActive(false); // Desactivar el objeto enemigo
            }
        }
        else
        {
            if (mostrarDebug)
            {
                Debug.Log($"[ControladorVidaEnemigo] {nombreEnemigo} ya no tiene vidas.");
            }
        }
    }
}
