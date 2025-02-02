using UnityEngine;
using UnityEngine.SceneManagement;

public class GestionEscenarios : MonoBehaviour
{
    public GameObject transicion;
    public ControladorGolpeValvula contadorValvula;
    public string escenaSiguiente;
    
    public bool esCanon = false;
    public bool esPato = false;
    void Start()
    {
        contadorValvula = GameObject.Find("Valvula").GetComponent<ControladorGolpeValvula>();
    }
        
    void Update()
    {
        if (contadorValvula.vidaActual >= contadorValvula.vidaTotal && esCanon)
        {            
            CambiarEscenaDelay(); // nombrar escena en el inspector AJJAJAJ
        }
    }
    public void CambiarEscenaDelay() // MACHETE 7 am
    {
        transicion.SetActive(true);
        ControladorScripts.instance.PausarJuego();        
        Invoke("CambiarEscena", 3);

    }
    public void CambiarEscena()
    {
        CambiarEscenaNext(escenaSiguiente);
    }

    public void CambiarEscenaNext(string nombreEscena)
    {
        Debug.Log("Cambiando Escena");        
        AudioImp.Instance.Reproducir("MusicaCanonStop");
        SceneManager.LoadScene(nombreEscena);
    }
}
