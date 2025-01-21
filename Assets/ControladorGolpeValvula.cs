using UnityEngine;

public class ControladorGolpeValvula : MonoBehaviour
{
    [Header("Depuracion")]
    public bool mostrarLog = true;

    #region Variables

    [Header("Variables Vida")]
    public int vida = 2;
    public int vidaActual;

    [Header("Información Valvula")]
    public string valvulaNumero;
    public GameObject canvasPuntero;

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

            if (mostrarLog) { Debug.Log($"[ControladorGolpeValvula] {valvulaNumero} recibió golpe, quedan: {vidaActual}/{vida}"); }

            if (vidaActual <= 0)
            {
                if (mostrarLog) { Debug.Log($"[ControladorGolpeValvula] {valvulaNumero} se cerro."); }

                //gameObject.SetActive(false);
            }
        }
        else
        {
            if (mostrarLog) { Debug.Log($"[ControladorGolpeValvula] {valvulaNumero} se cerro."); }
        }
    }

    public void ActivarPuntero()
    {
        canvasPuntero.SetActive(true);
    }
}
