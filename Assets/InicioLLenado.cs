using UnityEngine;

public class InicioLLenado : MonoBehaviour
{
    public bool mostrarDebug;
    [Header("Checks Llenado")]
    public bool estaLlenando;     // Indica si el flujo está tocando el suelo
    public bool empezarLlenado;  // Estado que se activa al iniciar el llenado una vez

    private bool estadoAnterior; // Check de alternación entre estados para evitar múltiples updates    

    void Start()
    {
        estadoAnterior = false;   // Inicializamos el estado anterior como falso
        empezarLlenado = false;   // Inicializamos como falso
    }

    void Update()
    {
        // Intenta encontrar el objeto por su nombre
        GameObject streamObject = GameObject.Find("Stream1");
        if (streamObject != null) // Verifica si el objeto fue encontrado
        {
            Stream streamComponent = streamObject.GetComponent<Stream>();
            if (streamComponent != null) // Verifica si tiene el componente Stream
            {
                estaLlenando = streamComponent.estaTocando;
            }
        }
        else
        {
            // Si no encuentra el objeto, asegura que estaLlenando sea false
            estaLlenando = false;
        }

        // Cambia el estado solo si es necesario
        if (estaLlenando && !estadoAnterior)
        {
            empezarLlenado = true;
            MostrarDebug("Empezó Llenado");
        }
        else if (!estaLlenando && estadoAnterior)
        {
            empezarLlenado = false;
            MostrarDebug("Detuvo Llenado");
        }

        estadoAnterior = estaLlenando;


    }

    private void MostrarDebug(string mensaje)
    {
        if (mostrarDebug)
        {
            Debug.Log($"[InicioLLenado]: {mensaje}");
        }
    }
}
