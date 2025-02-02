using UnityEngine;

public class MenuInicial : MonoBehaviour
{
#region Variables

    [Header("Depuraci�n")]
    public bool mostrarDebug;

    [Header("Objetos Menu Principal")]
    public GameObject menuInicial;
    public GameObject menuOpciones;
    public GameObject menuCreditos;    

#endregion

    void Start()
    {
        MostrarLog("Menu inicial cargado.");
    }

    void Update()
    {

    }

    public void Jugar()
    {
        MostrarLog("Entrando al juego...");     
    }


    public void AbrirMenuInicial()
    {
        menuInicial.SetActive(true);
        menuOpciones.SetActive(false);
        menuCreditos.SetActive(false);
        MostrarLog("Se abri� el men� inicial.");
    }

    public void AbrirMenuOpciones()
    {
        menuInicial.SetActive(false);
        menuOpciones.SetActive(true);
        menuCreditos.SetActive(false);
        MostrarLog("Se abri� el men� de opciones.");
    }

    public void AbrirMenuCreditos()
    {
        menuInicial.SetActive(false);
        menuOpciones.SetActive(false);
        menuCreditos.SetActive(true);
        MostrarLog("Se abri� el men� de cr�ditos.");
    }

    public void Salir()
    {
        MostrarLog("Saliendo del juego...");
        Application.Quit();
    }

    private void MostrarLog(string mensaje)
    {
        if (mostrarDebug)
        {
            Debug.Log($"[MenuInicial]: {mensaje}");
        }
    }
}
