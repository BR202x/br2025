using UnityEngine;

public class MenuOpciones : MonoBehaviour
{
#region Variables

    [Header("Depuración")]
    public bool mostrarDebug;

    [Header("Paneles de Opciones")]
    public GameObject panelControles;
    public GameObject panelSonido;
    public GameObject panelGraficos;

#endregion

    void Start()
    {
        MostrarLog("Menú de opciones cargado.");
    }

    void Update()
    {

    }

    public void MostrarControles()
    {
        panelControles.SetActive(true);
        panelSonido.SetActive(false);
        panelGraficos.SetActive(false);
        MostrarLog("Se mostró el panel de controles.");
    }

    public void MostrarSonido()
    {
        panelControles.SetActive(false);
        panelSonido.SetActive(true);
        panelGraficos.SetActive(false);
        MostrarLog("Se mostró el panel de sonido.");
    }

    public void MostrarGraficos()
    {
        panelControles.SetActive(false);
        panelSonido.SetActive(false);
        panelGraficos.SetActive(true);
        MostrarLog("Se mostró el panel de gráficos.");
    }

    public void Volver()
    {
        panelControles.SetActive(true);
        panelSonido.SetActive(false);
        panelGraficos.SetActive(false);
    }

    private void MostrarLog(string mensaje)
    {
        if (mostrarDebug)
        {
            Debug.Log($"[MenuOpciones]: {mensaje}");
        }
    }
}
