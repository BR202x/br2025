using UnityEngine;

public class DistanciaPlayerObjetos : MonoBehaviour
{
    public float distancia;
    public GameObject jugador;
    public GameObject objetoReproduccion;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        distancia = Vector3.Distance(jugador.transform.position, objetoReproduccion.transform.position);
    }
}
