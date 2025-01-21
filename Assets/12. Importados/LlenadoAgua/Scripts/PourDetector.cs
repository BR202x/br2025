using System.Collections;
using UnityEngine;

public class PourDetector : MonoBehaviour
{
    public bool mostrarDebug;
    [Space]
    public Transform origen = null;
    public GameObject valvulaPadre = null;
    public GameObject streamPrefab = null;

    [Header("Configuración Salida Agua")]
    public Vector3 direccionLinea = Vector3.down; // Dirección de la línea que se aplicará al Stream
    public float velocidadAgua;

    [Header("Configuración de Ancho del LineRenderer")]
    public float anchoInicial = 0.1f; // Ancho inicial del LineRenderer
    public float anchoFinal = 0.05f;  // Ancho final del LineRenderer

    private bool isPouring = false;
    public bool pourCheck = false;
    private Stream currentStream = null;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            MostrarDebug("Presionando X en: " + this.name);
            pourCheck = !pourCheck;
        }

        if (isPouring != pourCheck)
        {
            isPouring = pourCheck;

            if (isPouring)
            {
                StartPour();
            }
            else
            {
                EndPour();
            }
        }

        // Buscar el objeto hijo con el componente Stream
        Stream streamComponent = valvulaPadre.GetComponentInChildren<Stream>();

        if (streamComponent != null)
        {
            MostrarDebug($"Se encontró un Stream en {streamComponent.gameObject.name}");
            // Puedes modificar propiedades del Stream si es necesario
            streamComponent.direccionLinea = direccionLinea;
        }
        else
        {
            MostrarDebug("No se encontró ningún Stream en los hijos de valvulaPadre");
        }
    }

    public void StartPour()
    {
        MostrarDebug("Start");
        currentStream = CreateStream();
        currentStream.Begin();        

    }

    public void EndPour()
    {
        MostrarDebug("End");
        if (currentStream != null)
        {
            currentStream.End();
            currentStream = null;
        }
    }

    private Stream CreateStream()
    {
        // Determina el número basado en el nombre del origen
        string originName = origen.name;
        string streamName = "Stream";
        int number = ExtractNumberFromName(originName);

        if (number != -1)
        {
            streamName += number;
        }

        // Instancia el Stream y le asigna un nombre único
        GameObject streamObject = Instantiate(streamPrefab, origen.position, Quaternion.identity, valvulaPadre.transform);
        streamObject.name = streamName;

        // Configura el Stream instanciado
        Stream streamComponent = streamObject.GetComponent<Stream>();
        streamComponent.direccionLinea = direccionLinea;
        streamComponent.velocidadAnimacion = velocidadAgua;

        // Configurar el ancho del LineRenderer
        LineRenderer lineRenderer = streamObject.GetComponent<LineRenderer>();
        if (lineRenderer != null)
        {
                
            lineRenderer.startWidth = anchoInicial;
            lineRenderer.endWidth = anchoFinal;
        }

        MostrarDebug($"Creado Stream con nombre: {streamName}, dirección: {direccionLinea}, ancho inicial: {anchoInicial}, ancho final: {anchoFinal}");
        return streamComponent;
    }

    private int ExtractNumberFromName(string name)
    {
        string numberString = "";
        foreach (char c in name)
        {
            if (char.IsDigit(c))
            {
                numberString += c;
            }
        }

        return int.TryParse(numberString, out int result) ? result : -1;
    }

    private void MostrarDebug(string mensaje)
    {
        if (mostrarDebug)
        {
            Debug.Log($"[PourDetector]: {mensaje}");
        }
    }
}
