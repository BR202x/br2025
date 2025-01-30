using DG.Tweening.Plugins.Options;
using UnityEngine;
public class TestAudio : MonoBehaviour
{
    public string nombreReproduccion;

    private void Start()
    {
        ReproducirEvento(nombreReproduccion);
    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.E))
        {
            AudioImp.Instance.Reproducir("PlayerSword");
                       
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            AudioImp.Instance.Reproducir("Test2");
        }        
        if (Input.GetKeyDown(KeyCode.T))
        {
            
        }
        if (Input.GetKeyUp(KeyCode.T))
        {

        }

        if (Input.GetKeyDown(KeyCode.Y))
        {
            
        }
        if (Input.GetKeyUp(KeyCode.Y))
        {
            
        }
    }

    public void ReproducirEvento(string nombreReproduccion)
    {
        AudioImp.Instance.Reproducir(nombreReproduccion);
    }
}
