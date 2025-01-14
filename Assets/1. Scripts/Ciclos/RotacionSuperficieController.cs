using UnityEngine;

public class RotacionSuperficieController : MonoBehaviour
{ 
#region Variables

    [Header("Configuración de Rotación")]
        [Tooltip("Valor de Velocidad de rotacion de la superficie")]
    public float velocidadRotacion = 10f; // Velocidad de rotación configurable desde el inspector
        [Tooltip("Objeto Superficie del Tambor")]
    public GameObject superficieTambor;

#endregion

    void Update()
    {
        if (Input.GetKey(KeyCode.RightArrow))
        {
            superficieTambor.transform.Rotate(0, velocidadRotacion * Time.deltaTime, 0);
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            superficieTambor.transform.Rotate(0, -velocidadRotacion * Time.deltaTime, 0);
        }
    }
}
