using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class ControladorDaño : MonoBehaviour
{
    [Header("Variables Vida")]
    [Tooltip("Determina si el daño se aplica de menor a mayor en la lista de vidas.")]
    public bool actuarDeMenorAMayor = true;
    public int vida = 4;
    public int vidaActual;
    public List<GameObject> listaVidas = new List<GameObject>();

    [Header("Variables Oxígeno")]
    public float oxigeno = 1;
    public bool estaEnAgua = false;
    public Slider sliderOxigeno;
    public float tiempoOxigeno = 5f;

    [Header("Objetos")]
    public GameObject panelVida;
    public GameObject prefabVida;

    private bool dañoAplicado = false;

    void Start()
    {
        vidaActual = vida;
        GenerarVidas();
        sliderOxigeno.maxValue = 1;
        sliderOxigeno.value = oxigeno;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            MostrarLog("Presionando M: Recibir Daño");
            RecibirDaño();
        }

        if (Input.GetKeyDown(KeyCode.N))
        {
            MostrarLog("Presionando N: Recuperar Vida");
            RecuperarVida();
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            MostrarLog("Presionando O: Cambiando estado de estaEnAgua");
            estaEnAgua = !estaEnAgua;
        }

        if (estaEnAgua)
        {
            ReducirOxigeno();
        }
        else
        {
            RecuperarOxigeno();
        }
    }

    void GenerarVidas()
    {
        foreach (Transform child in panelVida.transform)
        {
            Destroy(child.gameObject);
        }

        listaVidas.Clear();

        for (int i = 0; i < vida; i++)
        {
            GameObject nuevaVida = Instantiate(prefabVida, panelVida.transform);
            nuevaVida.name = "Vida_" + (i + 1);
            listaVidas.Add(nuevaVida);
        }
    }

    public void RecibirDaño()
    {
        if (vidaActual > 0)
        {
            int index = actuarDeMenorAMayor ? vidaActual - 1 : listaVidas.Count - vidaActual;
            GameObject vidaPrefab = listaVidas[index];
            CambioSprite cambioSprite = vidaPrefab.GetComponent<CambioSprite>();
            if (cambioSprite != null)
            {
                cambioSprite.lleno = false;
                MostrarLog($"Vida {vidaPrefab.name} ahora está vacía.");
            }
            vidaActual--;
        }
    }

    public void RecuperarVida()
    {
        if (vidaActual < vida)
        {
            int index = actuarDeMenorAMayor ? vidaActual : listaVidas.Count - vidaActual - 1;
            GameObject vidaPrefab = listaVidas[index];
            CambioSprite cambioSprite = vidaPrefab.GetComponent<CambioSprite>();
            if (cambioSprite != null)
            {
                cambioSprite.lleno = true;
                MostrarLog($"Vida {vidaPrefab.name} ahora está llena.");
            }
            vidaActual++;
        }
        else
        {
            MostrarLog("La vida ya está al máximo.");
        }
    }

    private void ReducirOxigeno()
    {
        oxigeno -= Time.deltaTime / tiempoOxigeno;
        oxigeno = Mathf.Clamp(oxigeno, 0, 1);
        sliderOxigeno.value = oxigeno;

        if (oxigeno <= 0 && !dañoAplicado)
        {
            RecibirDaño();
            dañoAplicado = true;
        }
    }

    private void RecuperarOxigeno()
    {
        oxigeno += Time.deltaTime / (tiempoOxigeno / 2);
        oxigeno = Mathf.Clamp(oxigeno, 0, 1);
        sliderOxigeno.value = oxigeno;

        if (oxigeno > 0)
        {
            dañoAplicado = false;
        }
    }

    private void MostrarLog(string mensaje)
    {
        Debug.Log(mensaje);
    }
}
