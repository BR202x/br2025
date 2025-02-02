using UnityEngine;

public class DeteccionAguaMotor : MonoBehaviour
{
    [Header("Check para escenas DEBUG")]
    public bool isEnabled = false;
    [Header("Objetos Detectar Nivel Agua - FMOD")]
    public Transform objetoAgua;
    public Transform umbralCambioSonido;
    public bool sobreAgua;

    void Update()
    {
        if (isEnabled)
        {
            sobreAgua = false;

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
}

