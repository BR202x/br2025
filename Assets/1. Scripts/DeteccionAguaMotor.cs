using UnityEngine;

public class DeteccionAguaMotor : MonoBehaviour
{
    public Transform objetoAgua;
    public Transform umbralCambioSonido;
    public bool sobreAgua;
        
    void Update()
    {
        if (objetoAgua.position.y >= umbralCambioSonido.position.y)
        {            
            sobreAgua = true;
        }
        else
        {
            sobreAgua = false;
        }
    }
}
