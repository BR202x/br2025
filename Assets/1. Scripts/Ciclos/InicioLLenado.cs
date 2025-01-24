using UnityEngine;

public class InicioLLenado : MonoBehaviour
{
#region Variables

    public bool mostrarDebug;
    [Header("Checks Llenado")]
        [Tooltip("Indicador de cuando el chorro toca la superficie")]
    public bool estaLlenando;     // Indica si el flujo está tocando el suelo
        [Tooltip("Indicador activar el Llenado del Agua")]
    public bool empezarLlenado;  // Estado que se activa al iniciar el llenado
    [Header("Referencias")]
    public LlenadoManager llenadoManager;

#endregion
    void Start()
    {
        llenadoManager = GameObject.Find("LlenadoDeAguaManager").GetComponent<LlenadoManager>();
        empezarLlenado = false; // Inicializamos como falso
    }

    void Update()
    {   
        InstanciaNewChorro chorro  = FindFirstObjectByType<InstanciaNewChorro>();

        if (chorro != null)
        {

            estaLlenando = chorro.estaTocandoLlenar;

        }
        else
        {
            estaLlenando = false;
        }

        // Cambiar el estado de `moviendoHaciaFinal` según `estaLlenando`
        if (estaLlenando)
        {
            empezarLlenado = true;
            llenadoManager.llenandoTambor = true;
        }
        else
        {
            empezarLlenado = false;
            llenadoManager.llenandoTambor = false;
        }

        if (mostrarDebug)
        {
            MostrarDebug($"estaLlenando: {estaLlenando}, moviendoHaciaFinal: {llenadoManager.llenandoTambor}");
        }
    }

    private void MostrarDebug(string mensaje)
    {
        if (mostrarDebug)
        {
            Debug.Log($"[InicioLLenado]: {mensaje}");
        }
    }
}
