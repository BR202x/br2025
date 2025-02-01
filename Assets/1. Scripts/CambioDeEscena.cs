using UnityEngine;
using UnityEngine.SceneManagement;

public class CambioDeEscena : MonoBehaviour
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
        if (contadorValvula.vidaActual >= 9 && esCanon)
        {
            transicion.SetActive(true);
            ControladorScripts.instance.PausarJuego();
            Invoke("CambiarEscena", 3);
        }
    }

    public void CambiarEscena()
    {
        CambiarEscenaNext(escenaSiguiente);
    }

    public void CambiarEscenaNext(string nombreEscena)
    {
        Debug.Log("Cambiando Escena");
        SceneManager.LoadScene(nombreEscena);
    }
}
