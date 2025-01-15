using UnityEngine;

public class ControladorVidaEnemigo : MonoBehaviour
{
    [Header("Depuraci�n")]
    public bool mostrarDebug = true;

    [Header("Variables Vida")]
    public int vida = 2;
    public int vidaActual;

    [Header("Informaci�n del Enemigo")]
    public string nombreEnemigo;

    void Start()
    {
        vidaActual = vida;
    }

    public void RecibirDa�o()
    {
        if (vidaActual > 0)
        {
            vidaActual--;

            if (mostrarDebug)
            {
                Debug.Log($"[ControladorVidaEnemigo] {nombreEnemigo} recibi� da�o. Vida actual: {vidaActual}/{vida}");
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
