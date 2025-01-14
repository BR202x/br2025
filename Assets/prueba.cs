using UnityEngine;

public class prueba : MonoBehaviour
{

    public float velocidadInicial = 10f;
    public float angulo = 45f;
    public float gravedad = 2f;

    private float tiempo = 0f;

    private void Update()
    {
        tiempo += Time.deltaTime; // Aumentamos el tiempo en cada frame

        // Calculamos la posición en el eje X
        float x = velocidadInicial * Mathf.Cos(Mathf.Deg2Rad * angulo) * tiempo;

        // Calculamos la posición en el eje Y
        float y = velocidadInicial * Mathf.Sin(Mathf.Deg2Rad * angulo) * tiempo - (0.5f * gravedad * Mathf.Pow(tiempo, 2));

        // Actualizamos la posición del proyectil
        transform.position = new Vector3(x, y, 0);
    }
}
