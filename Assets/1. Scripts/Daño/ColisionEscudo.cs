using UnityEngine;

public class ColisionEscudo : MonoBehaviour
{
    [Header("Depuración")]
    public bool mostrarLog = true;
    [Space]
    public bool daño = false;
    public ControladorUIEnemigo controladorUI;
    public ActivacionPanelTargets panelVidaEnemigos;

    private void OnEnable()
    {
        panelVidaEnemigos = GameObject.Find("Panel_VidaEnemigos").GetComponent<ActivacionPanelTargets>();
    }

    private void Start()
    {
        // panelVidaEnemigos = FindFirstObjectByType<ActivacionPanelTargets>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (mostrarLog) { Debug.Log($"El Shield entró en contacto con: {other.gameObject.name}"); }

        if (other.CompareTag("Enemigo") && !daño)
        {
            panelVidaEnemigos.activarCanvasTarget();
            ControladorUIEnemigo controladorUI = panelVidaEnemigos.GetComponentInChildren<ControladorUIEnemigo>();

            ControladorVidaEnemigo enemigo = other.GetComponent<ControladorVidaEnemigo>();

            if (enemigo != null)
            {
                enemigo.RecibirDaño();
                controladorUI.ActualizarUI(enemigo);
                daño = true;               

            }
        }
    }
}
