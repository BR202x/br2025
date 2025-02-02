using UnityEngine;

public class SonidoDañoBoss : MonoBehaviour
{
    public HealthController vidaActual;
    private float ultimaVida;

    void Start()
    {
        if (vidaActual != null)
        {           
            ultimaVida = vidaActual.vidaActual;
        }
        else
        {
            Debug.LogError("Referencia a HealthController no asignada.");
        }
    }

    void Update()
    {
        if (vidaActual == null) return;
                
        if (!Mathf.Approximately(ultimaVida, vidaActual.vidaActual))
        {
            AudioImp.Instance.Reproducir("enemyBossHit");
            ultimaVida = vidaActual.vidaActual;
        }
    }
}
