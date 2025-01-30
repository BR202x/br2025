using UnityEngine;
public class SeguirJugadorAgua : MonoBehaviour
{
    public Transform jugador;
    public float offSetY;

    void Update()
    {
        transform.position = new Vector3(jugador.transform.position.x, jugador.transform.position.y + offSetY, jugador.transform.position.z);   
    }
}
