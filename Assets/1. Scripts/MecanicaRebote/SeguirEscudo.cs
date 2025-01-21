using UnityEngine;

public class SeguirEscudo : MonoBehaviour
{
    [Header("Configuración")]
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
                rotacionActual.y = objetivoRaton.rotation.eulerAngles.y;
                transform.rotation = Quaternion.Euler(rotacionActual);
            }
        }        
    }
}
