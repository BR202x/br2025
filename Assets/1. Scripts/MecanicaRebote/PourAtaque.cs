using System.Collections;
using UnityEngine;

public class PourAtaque : MonoBehaviour
{
    [Header("depuracion")]
    public bool mostrarLog;

    #region Variables

    [Space]
    public Transform origen = null;
    public GameObject valvulaPadre = null;
    public GameObject streamPrefab = null;

    [Header("Configuracion Salida Agua")]
    public Vector3 direccionLinea = Vector3.down;
    public float velocidadAgua = 5f;

    [Header("Configuracion de Ancho del LineRenderer")]
    public float anchoInicial = 0.1f;
    public float anchoFinal = 0.05f;

    private bool isPouring = false;
    public bool pourCheck = false;
    private ChorroAtaque currentStream = null;

    #endregion

    private void Update()
    {        
        if (Input.GetKeyDown(KeyCode.L))
        {
            if (mostrarLog) { Debug.Log("Presionando X en: " + this.name); }
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
               
        if (currentStream != null)
        {
            currentStream.direccionLinea = direccionLinea;
        }
    }

    private void StartPour()
    {        
        currentStream = CreateStream();
        currentStream.Begin();
    }

    private void EndPour()
    {        
        if (currentStream != null)
        {
            currentStream.End();
            currentStream = null;
        }
    }

    private ChorroAtaque CreateStream()
    {        
        string originName = origen.name;
        string streamName = "Stream";
        int number = ExtractNumberFromName(originName);

            if (number != -1)
            {
                streamName += number;
            }
                
        GameObject streamObject = Instantiate(streamPrefab, origen.position, Quaternion.identity, valvulaPadre.transform);
        streamObject.name = streamName;
                
        ChorroAtaque streamComponent = streamObject.GetComponent<ChorroAtaque>();
        streamComponent.direccionLinea = direccionLinea;
        streamComponent.velocidadAnimacion = velocidadAgua;
        
        LineRenderer lineRenderer = streamObject.GetComponent<LineRenderer>();

            if (lineRenderer != null)
            {
                lineRenderer.startWidth = anchoInicial;
                lineRenderer.endWidth = anchoFinal;
            }

        if (mostrarLog) { Debug.Log($"Creado Stream con nombre: {streamName}, direccion: {direccionLinea}, ancho inicial: {anchoInicial}, ancho final: {anchoFinal}"); }
        return streamComponent;
    }

    private int ExtractNumberFromName(string name) // Para el nombre de la instancia, no creo que sea necesario pero quiero diferenciarlas.
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
}
