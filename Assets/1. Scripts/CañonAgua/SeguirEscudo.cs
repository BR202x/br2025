using UnityEngine;

// EDITADO: 29/01/2025 - 23:15
public class SeguirEscudo : MonoBehaviour
{
    [Header("Configuracion")]
    [Tooltip("Transform del escudo a seguir")]
    public Transform objetivoEscudo;
    [Tooltip("Transform del raton que determina la rotacion en Y")]
    public Transform objetivoRaton;

    private void Update()
    {
        if (objetivoEscudo != null)
        {
            transform.position = objetivoEscudo.position;

            if (objetivoRaton != null)
            {
                Vector3 rotacionActual = transform.rotation.eulerAngles;

                rotacionActual.y = objetivoRaton.rotation.eulerAngles.y;

                transform.rotation = Quaternion.Euler(rotacionActual);
            }
        }
    }
}
