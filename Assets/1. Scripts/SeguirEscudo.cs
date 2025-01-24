using UnityEngine;

public class SeguirEscudo : MonoBehaviour
{
    [Header("Configuraci�n")]
    public Transform objetivoEscudo;
    public Transform objetivoRaton;

    // Unicamente para ubicar el plano de rebote en la posicion del escudo.

    private void Update()
    {
        if (objetivoEscudo != null)
        {
            transform.position = objetivoEscudo.position;

            if (objetivoRaton != null)
            {
                Vector3 rotacionActual = transform.rotation.eulerAngles;

                // Asignar rotaci�n en Y desde el objetivo del rat�n
                rotacionActual.y = objetivoRaton.rotation.eulerAngles.y;

                transform.rotation = Quaternion.Euler(rotacionActual);
            }
        }
    }
}