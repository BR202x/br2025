using DG.Tweening;
using UnityEngine;

public class ControladorGolpeValvula : MonoBehaviour
{
    [Header("Depuracion")]
    public bool mostrarLog = false;

    #region Variables

    [Header("Variables Valvula")]
    public int vida = 0;
    public int vidaActual;
    public bool fase2 = false;
    public bool fase3 = false;

    [Header("Información Valvula")]
    public string valvulaNumero;
    public GameObject canvasPuntero;

    #endregion

    void Start()
    {
        vidaActual = vida;
    }

    public void RecibirDano()
    {
        if (vidaActual <= 4)
        {
            vidaActual++;            

            if (vidaActual == 2 && !fase2)
            {
                fase2 = true;
                if (mostrarLog) { Debug.Log($"[ControladorGolpeValvula]: cambio de fase {fase2}"); }
            }

            if (vidaActual == 4 && fase2 && !fase3)
            {
                fase3 = true;
                if (mostrarLog) { Debug.Log($"[ControladorGolpeValvula]: cambio de fase {fase3}"); }
            }
        }
    }

    public void ActivarPuntero()
    {
        canvasPuntero.SetActive(true);
    }
}
