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
    public int vidaParaFase2 = 3;
    public int vidaParaFase3 = 6;
    public int vidaTotal = 9;
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
        if (vidaActual <= vidaTotal)
        {
            vidaActual++;            

            if (vidaActual == vidaParaFase2 && !fase2)
            {
                fase2 = true;
                if (mostrarLog) { Debug.Log($"[ControladorGolpeValvula]: cambio de fase {fase2}"); }
            }

            if (vidaActual == vidaParaFase3 && fase2 && !fase3)
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
