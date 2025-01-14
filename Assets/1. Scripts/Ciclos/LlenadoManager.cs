using UnityEngine;

public class LlenadoManager : MonoBehaviour
{
#region Variables

    public bool llenandoTambor = false;
    [Header("Posiciones de Llenado de Agua")]
    public Transform posicionInicial;
    public Transform posicionFinal;
    [Header("Objeto Agua")]
    public GameObject nivelAgua;
    [Header("Velocidad de Llenado")]
    public float velocidad = 5f; // Velocidad de movimiento    

#endregion

    void Update()
    {
        // Mover el objeto según la dirección
        if (llenandoTambor)
        {         
            //Debug.Log("Llenando");
            // Mover hacia posicionFinal
            nivelAgua.transform.position = Vector3.MoveTowards(
                nivelAgua.transform.position,
                posicionFinal.position,
                velocidad * Time.deltaTime
            );
        }
        else
        {            
            // Debug.Log("No llenando");
            // Mover hacia posicionInicial
            nivelAgua.transform.position = Vector3.MoveTowards(
                nivelAgua.transform.position,
                posicionInicial.position,
                velocidad * Time.deltaTime
            );
        }
    }
}
