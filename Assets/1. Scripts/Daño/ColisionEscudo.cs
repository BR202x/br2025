using UnityEngine;

public class ColisionEscudo : MonoBehaviour
{
    #region Variables

    [Header("depuracion")]
    public bool mostrarLog = true;

    [Header("Configuracion")]
    private bool dano = false;
    public ChorroTargetController chorroTargetController;
    private ControladorUIEnemigo controladorUI;
    private ActivacionPanelTargets panelVidaEnemigos;

    #endregion

    private void OnEnable()
    {
        panelVidaEnemigos = GameObject.Find("Panel_VidaEnemigos").GetComponent<ActivacionPanelTargets>();
        chorroTargetController = GameObject.Find("Chorro_Manager").GetComponent<ChorroTargetController>();
    }

    private void Start()
    {
          
    }

    private void OnTriggerEnter(Collider other)
    {
        if (mostrarLog) { Debug.Log($"[ColisionEscudo] Nombre: {other.gameObject.name} Tag: {other.gameObject.tag} Layer: {other.gameObject.layer}"); }

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
            Debug.Log("Valvula CAÑON");            
            chorroTargetController.EmpezarEstado3();
        }
    }
}
