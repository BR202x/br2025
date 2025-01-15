using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class ControladorDano : MonoBehaviour
{
    [Header("Configuración de Depuración")]
    public bool mostrarDebug = true;

    [Header("Variables Vida")]
    [Tooltip("Esta tiene nombre, se que se despertara la duda 'Ni si piidi hicir qi si ridisqi in il itri sintidi?' xD")]
    public bool actuarDeMenorAMayor = true;
    public int vida = 4;
    public int vidaActual;
    public List<GameObject> listaVidas = new List<GameObject>();

    [Header("Variables Oxígeno")]
    public float oxigeno = 1;
    public bool estaEnAgua = false;
    public Slider sliderOxigeno;
    public float tiempoOxigeno = 5f;

    [Header("Daño por oxígeno bajo")]
    public float tiempoParaReducirVida = 3f;
    private float contadorTiempo = 0f;

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
            if (mostrarDebug) Debug.Log("[ControladorDaño] Input M: Recibir Daño.");
            RecibirDaño();
        }

        if (Input.GetKeyDown(KeyCode.N))
        {
            if (mostrarDebug) Debug.Log("[ControladorDaño] Input N: Recuperar Vida.");
            RecuperarVida();
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            if (mostrarDebug) Debug.Log("[ControladorDaño] Input O: Cambiando estado de agua.");
            estaEnAgua = !estaEnAgua;
        }

        if (estaEnAgua)
        {
            ReducirOxigeno();
        }
        else if (oxigeno < 1)
        {
            RecuperarOxigeno();
        }

        if (oxigeno <= 0)
        {
            ContarParaReducirVida();
        }
        else
        {
            contadorTiempo = tiempoParaReducirVida;
        }
    }

    void GenerarVidas()
    {
        if (mostrarDebug) Debug.Log("[ControladorDaño] Generando vidas.");
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
        if (mostrarDebug) Debug.Log("[ControladorDaño] Aplicando daño.");
        if (vidaActual > 0)
        {
            int index = actuarDeMenorAMayor ? vidaActual - 1 : listaVidas.Count - vidaActual;
            GameObject vidaPrefab = listaVidas[index];
            CambioSprite cambioSprite = vidaPrefab.GetComponent<CambioSprite>();
            if (cambioSprite != null)
            {
                cambioSprite.lleno = false;
                if (mostrarDebug) Debug.Log($"[ControladorDaño] Vida {vidaPrefab.name} vacía.");
            }
            vidaActual--;
        }
    }

    public void RecuperarVida()
    {
        if (mostrarDebug) Debug.Log("[ControladorDaño] Recuperando vida.");
        if (vidaActual < vida)
        {
            int index = actuarDeMenorAMayor ? vidaActual : listaVidas.Count - vidaActual - 1;
            GameObject vidaPrefab = listaVidas[index];
            CambioSprite cambioSprite = vidaPrefab.GetComponent<CambioSprite>();
            if (cambioSprite != null)
            {
                cambioSprite.lleno = true;
                if (mostrarDebug) Debug.Log($"[ControladorDaño] Vida {vidaPrefab.name} llena.");
            }
            vidaActual++;
        }
        else
        {
            if (mostrarDebug) Debug.Log("[ControladorDaño] Vida al máximo.");
        }
    }

    private void ReducirOxigeno()
    {
        if (mostrarDebug) Debug.Log("[ControladorDaño] Reduciendo oxígeno.");
        oxigeno -= Time.deltaTime / tiempoOxigeno;
        oxigeno = Mathf.Clamp(oxigeno, 0, 1);
        sliderOxigeno.value = oxigeno;

        if (oxigeno <= 0 && !dañoAplicado)
        {
            dañoAplicado = true;
        }
    }

    private void RecuperarOxigeno()
    {
        if (mostrarDebug) Debug.Log("[ControladorDaño] Recuperando oxígeno.");
        oxigeno += Time.deltaTime / (tiempoOxigeno / 2);
        oxigeno = Mathf.Clamp(oxigeno, 0, 1);
        sliderOxigeno.value = oxigeno;

        if (oxigeno > 0)
        {
            dañoAplicado = false;
        }
    }

    private void ContarParaReducirVida()
    {
        if (mostrarDebug) Debug.Log("[ControladorDaño] Contando para daño.");
        contadorTiempo -= Time.deltaTime;

        if (contadorTiempo <= 0f)
        {
            RecibirDaño();
            contadorTiempo = tiempoParaReducirVida;
        }
    }
}
