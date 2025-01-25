using System.Collections;
using UnityEngine;

public class InicioLLenado : MonoBehaviour
{
    #region Variables

    public bool mostrarDebug;
    [Header("Checks Llenado")]
    [Tooltip("Indicador de cuando el chorro toca la superficie")]
    public bool estaLlenando; // Indica si el flujo está tocando el suelo
    [Tooltip("Indicador activar el Llenado del Agua")]
    public bool empezarLlenado; // Estado que se activa al iniciar el llenado
    [Header("Referencias")]
    public LlenadoManager llenadoManager;

    // Tiempo de retraso para empezar el llenado
    [Tooltip("Tiempo de retraso para empezar el llenado en segundos")]
    public float tiempoRetraso = 2f;

    private Coroutine llenadoCoroutine; // Referencia a la corutina de llenado

    #endregion

    void Start()
    {
        llenadoManager = GameObject.Find("LlenadoDeAguaManager").GetComponent<LlenadoManager>();
        empezarLlenado = false; // Inicializamos como falso
    }

    void Update()
    {
        InstanciaNewChorro chorro = FindFirstObjectByType<InstanciaNewChorro>();

        if (chorro != null)
        {
            estaLlenando = chorro.estaTocandoLlenar;
        }
        else
        {
            estaLlenando = false;
        }

        // Iniciar o detener el llenado según el estado de `estaLlenando`
        if (estaLlenando)
        {
            if (llenadoCoroutine == null) // Evitar múltiples corutinas
            {
                llenadoCoroutine = StartCoroutine(ActivarLlenadoConRetraso());
            }
        }
        else
        {
            if (llenadoCoroutine != null) // Detener la corutina si `estaLlenando` es falso
            {
                StopCoroutine(llenadoCoroutine);
                llenadoCoroutine = null;
            }

            // Reiniciar los estados
            empezarLlenado = false;
            llenadoManager.llenandoTambor = false;
        }

        if (mostrarDebug)
        {
            MostrarDebug($"estaLlenando: {estaLlenando}, moviendoHaciaFinal: {llenadoManager.llenandoTambor}");
        }
    }

    private IEnumerator ActivarLlenadoConRetraso()
    {
        // Esperar el tiempo configurado
        yield return new WaitForSeconds(tiempoRetraso);

        // Activar los estados después del retraso
        empezarLlenado = true;
        llenadoManager.llenandoTambor = true;

        if (mostrarDebug)
        {
            MostrarDebug($"Llenado activado después de {tiempoRetraso} segundos.");
        }

        llenadoCoroutine = null; // Resetear la referencia de la corutina
    }

    private void MostrarDebug(string mensaje)
    {
        if (mostrarDebug)
        {
            Debug.Log($"[InicioLLenado]: {mensaje}");
        }
    }
}
