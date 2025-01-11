using System.Collections;
using UnityEngine;

public class PourDetector : MonoBehaviour
{
    public bool mostrarDebug;
    [Space]
    public Transform origin = null;
    public GameObject streamPrefab = null;

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
    }

    private void StartPour()
    {
        MostrarDebug("Start");
        currentStream = CreateStream();
        currentStream.Begin();
    }

    private void EndPour()
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
        // Determina el número basado en el nombre del origin
        string originName = origin.name;
        string streamName = "Stream"; // Nombre por defecto
        int number = ExtractNumberFromName(originName);

        if (number != -1)
        {
            streamName += number; // Si el nombre contiene un número, lo agrega
        }

        // Instancia el Stream y le asigna un nombre único
        GameObject streamObject = Instantiate(streamPrefab, origin.position, Quaternion.identity, transform);
        streamObject.name = streamName;

        MostrarDebug($"Creado Stream con nombre: {streamName}");
        return streamObject.GetComponent<Stream>();
    }

    // Método para extraer números del nombre
    private int ExtractNumberFromName(string name)
    {
        // Filtra los caracteres del nombre que son números
        string numberString = "";
        foreach (char c in name)
        {
            if (char.IsDigit(c))
            {
                numberString += c;
            }
        }

        // Si encuentra un número, lo convierte en entero; de lo contrario, devuelve -1
        return int.TryParse(numberString, out int result) ? result : -1;
    }

    // Método para mostrar mensajes de depuración si está habilitado
    private void MostrarDebug(string mensaje)
    {
        if (mostrarDebug)
        {
            Debug.Log($"[PourDetector]: {mensaje}");
        }
    }
}
