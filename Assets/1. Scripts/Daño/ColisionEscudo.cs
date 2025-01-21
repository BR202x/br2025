using UnityEngine;

public class ColisionEscudo : MonoBehaviour
{
    [Header("Depuraci�n")]
    public bool mostrarLog = false;
    [Space]
    public bool da�o = false;
    public ControladorUIEnemigo controladorUI;
    public ActivacionPanelTargets panelVidaEnemigos;

    private void OnEnable()
    {
        panelVidaEnemigos = GameObject.Find("Panel_VidaEnemigos").GetComponent<ActivacionPanelTargets>();
    }

    private void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (mostrarLog) { Debug.Log($"El Shield entr� en contacto con: {other.gameObject.name}"); }

        if (other.CompareTag("Enemigo") && !da�o)
        {
            panelVidaEnemigos.activarCanvasTarget();
            ControladorUIEnemigo controladorUI = panelVidaEnemigos.GetComponentInChildren<ControladorUIEnemigo>();

            ControladorVidaEnemigo enemigo = other.GetComponent<ControladorVidaEnemigo>();

            if (enemigo != null)
            {
                enemigo.RecibirDa�o();
                controladorUI.ActualizarUI(enemigo);
                da�o = true;               

            }
        }

        if (other.CompareTag("Valvula"))
        {
            PourDetector accionValvula = other.GetComponent<PourDetector>();

            if (accionValvula != null)
            {
                accionValvula.EndPour();
            }
        }
    }
}
