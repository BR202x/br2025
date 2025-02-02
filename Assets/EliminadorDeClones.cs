using UnityEngine;
using System.Collections.Generic;

public class EliminadorDeClones : MonoBehaviour
{
    private List<GameObject> clonesRegistrados = new List<GameObject>();
    private Dictionary<GameObject, float> tiempoDeCreacion = new Dictionary<GameObject, float>();

    private void Update()
    {
        // SUPER MACHETEADA - Mirar documentacion
        GameObject[] allObjects = Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None);

        foreach (GameObject obj in allObjects)
        {            
            if (obj.name.Contains("Burbuja(Clone)") && !tiempoDeCreacion.ContainsKey(obj))
            {
                tiempoDeCreacion[obj] = Time.time;
                clonesRegistrados.Add(obj);

                AudioImp.Instance.Reproducir("BossBubbles");
            }
        }
                
        if (clonesRegistrados.Count > 0)
        {
            GameObject primerClone = clonesRegistrados[0];
            float tiempoAparecido = tiempoDeCreacion[primerClone];

            if (Time.time >= tiempoAparecido + 5f)
            {
                clonesRegistrados.RemoveAt(0);
                tiempoDeCreacion.Remove(primerClone);
                Destroy(primerClone);
                Debug.Log("Se eliminó el clon: " + primerClone.name);
            }
        }
    }
}
