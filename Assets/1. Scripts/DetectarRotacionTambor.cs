using UnityEngine;

public class DetectarRotacionTambor : MonoBehaviour
{
    public bool mostrarDebug; // Controla si se mostrar�n los mensajes de depuraci�n en la consola

    [Header("Referencia Tambor")]
    public Transform tambor;

    [Header("Checks")] // Check para saber hacia d�nde aplicar la fuerza
    public bool girandoSentidoHorario;
    public bool girandoSentidoContrario;
    public bool tamborEstatico;    

    private float ultimaRotacionY;
    private const float umbralEstatico = 0.1f; // Umbral para considerar el tambor est�tico
    [Space]
    public float velocidadRotacionTambor; // Velocidad de rotaci�n del tambor en Z

    private void Start()
    {
        if (tambor != null)
        {
            ultimaRotacionY = tambor.eulerAngles.y;
        }
    }

    private void Update()
    {
        if (tambor != null)
        {
            #region C�lculo Velocidad de Rotaci�n del Tambor

            float rotacionActualY = tambor.eulerAngles.y;
            velocidadRotacionTambor = rotacionActualY - ultimaRotacionY;

            // Ajuste para evitar saltos entre 360 y 0
            if (velocidadRotacionTambor > 180f)
            {
                velocidadRotacionTambor -= 360f;
            }
            else if (velocidadRotacionTambor < -180f)
            {
                velocidadRotacionTambor += 360f;
            }

            ultimaRotacionY = rotacionActualY;

            #endregion

            #region Estados Movimiento del Tambor

            if (Mathf.Abs(velocidadRotacionTambor) < umbralEstatico) // Verificar si el tambor est� est�tico
            {
                girandoSentidoHorario = false;
                girandoSentidoContrario = false;
                tamborEstatico = true;

                DLog("El tambor est� est�tico.");
            }
            else if (velocidadRotacionTambor > 0) // Girando en sentido horario
            {
                girandoSentidoHorario = true;
                girandoSentidoContrario = false;
                tamborEstatico = false;

                DLog("El tambor est� girando en sentido horario.");
            }
            else if (velocidadRotacionTambor < 0) // Girando en sentido antihorario
            {
                girandoSentidoHorario = false;
                girandoSentidoContrario = true;
                tamborEstatico = false;

                DLog("El tambor est� girando en sentido antihorario.");
            }

            #endregion
        }
    }

    private void DLog(string texto)
    {
        if (mostrarDebug)
        {
            Debug.Log($"[DetectarRotacionTambor]: {texto}");
        }
    }
}
