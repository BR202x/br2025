using UnityEngine;

public class SonidoDañoBoss : MonoBehaviour
{
    public HealthController vidaActual;
    private float ultimaVida;
    public bool estaVivo = true;
    public int reproduccion = 0;
    public GestionEscenarios cambioEscena;

    void Start()
    {
        estaVivo = true;

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
            reproduccion++;

            if (reproduccion > 1)
            {
                AudioImp.Instance.Reproducir("enemyBossHit");
            }
            ultimaVida = vidaActual.vidaActual;
        }

        if (vidaActual.vidaActual == 0 && estaVivo)
        {            
            AudioImp.Instance.Reproducir("BossDie");
            cambioEscena.CambiarEscenaDelay();
            estaVivo = false;
        }
    }
}
