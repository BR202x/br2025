using UnityEngine;

public class ColisionEscudo : MonoBehaviour
{
    #region Variables

    [Header("depuracion")]
    public bool mostrarLog = false;

    [Header("Configuracion")]
    private bool dano = false;
    private ControladorUIEnemigo controladorUI;
    private ActivacionPanelTargets panelVidaEnemigos;

    #endregion

    private void OnEnable()
    {
        panelVidaEnemigos = GameObject.Find("Panel_VidaEnemigos").GetComponent<ActivacionPanelTargets>();
    }

    private void Start()
    {
        // Inicializacion si es necesaria en el futuro
    }

    private void OnTriggerEnter(Collider other)
    {
        if (mostrarLog) { Debug.Log($"El Shield entro en contacto con: {other.gameObject.name}"); }

        if (other.CompareTag("Enemigo") && !dano)
        {
            panelVidaEnemigos.activarCanvasTarget();
            ControladorUIEnemigo controladorUI = panelVidaEnemigos.GetComponentInChildren<ControladorUIEnemigo>();

            ControladorVidaEnemigo enemigo = other.GetComponent<ControladorVidaEnemigo>();

            if (enemigo != null)
            {
                enemigo.RecibirDano();
                controladorUI.ActualizarUI(enemigo);
                dano = true;
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
