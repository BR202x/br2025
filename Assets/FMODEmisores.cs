using System.Collections.Generic;
using UnityEngine;

public class FMODEmisores : MonoBehaviour
{
    [System.Serializable]
    public class Emitter
    {
        public string nombre;
        public GameObject gameObject;
    }

    [SerializeField] private List<Emitter> emisores = new List<Emitter>();
    private Dictionary<string, GameObject> emisorDictionary = new Dictionary<string, GameObject>();

    public static FMODEmisores instance { get; private set; }

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Más de un FMODEmisores activo en la escena.");
            return;
        }

        instance = this;

        // Rellenar el diccionario con los emisores iniciales
        foreach (var emisor in emisores)
        {
            if (!emisorDictionary.ContainsKey(emisor.nombre))
            {
                emisorDictionary.Add(emisor.nombre, emisor.gameObject);
            }
            else
            {
                Debug.LogWarning($"El emisor con el nombre '{emisor.nombre}' ya está registrado.");
            }
        }
    }

    public void AddEmisor(string nombre, GameObject gameObject)
    {
        if (emisorDictionary.ContainsKey(nombre))
        {
            Debug.LogWarning($"El emisor con el nombre '{nombre}' ya existe. No se añadirá.");
            return;
        }

        emisorDictionary.Add(nombre, gameObject);
        emisores.Add(new Emitter { nombre = nombre, gameObject = gameObject });
    }

    public void RemoveEmisor(string nombre)
    {
        if (!emisorDictionary.ContainsKey(nombre))
        {
            Debug.LogWarning($"El emisor con el nombre '{nombre}' no existe.");
            return;
        }

        GameObject objToRemove = emisorDictionary[nombre];
        emisores.RemoveAll(e => e.nombre == nombre);
        emisorDictionary.Remove(nombre);
    }

    public GameObject GetEmisor(string nombre)
    {
        if (emisorDictionary.TryGetValue(nombre, out var gameObject))
        {
            return gameObject;
        }

        Debug.LogWarning($"El emisor con el nombre '{nombre}' no se encontró.");
        return null;
    }
}
