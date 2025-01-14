using UnityEngine;

public class DetectarRotacionTambor : MonoBehaviour
{
#region Variables

    public bool mostrarDebug; // Controla si se mostrarán los mensajes de depuración en la consola
    [Header("Referencia Tambor")]
        [Tooltip("Transform del objeto Superficie Del Tambor para detectar su rotacion")]
    public Transform tambor;
    [Header("Checks")] // Check para saber hacia dónde aplicar la fuerza    
    public bool girandoSentidoHorario;
    public bool girandoSentidoContrario;
    public bool tamborEstatico;    
    private float ultimaRotacionY;
    [Header("Valores de Velocidad")]
        [Tooltip("Umbral para determinar cuando afectar al jugador por la rotacion de la superficie")]
    public float umbralMovJugador = 0.5f; // Umbral para considerar el tambor estático - mover al jugador fuerza tangencial    
        [Tooltip("Valor de Velocidad de rotacion de la superficie")]
    public float velocidadRotacionTambor; // Velocidad de rotación del tambor en Z

#endregion

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
            #region Cálculo Velocidad de Rotación del Tambor

            float rotacionActualY = tambor.eulerAngles.y;
            velocidadRotacionTambor = rotacionActualY - ultimaRotacionY;

            // Arreglo "Machetazo" para evitar saltos negativos hacer el calculo.
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

            if (Mathf.Abs(velocidadRotacionTambor) < umbralMovJugador)
            {
                girandoSentidoHorario = false;
                girandoSentidoContrario = false;
                tamborEstatico = true;

                DLog("El tambor está estático.");
            }
            else if (velocidadRotacionTambor > 0)
            {
                girandoSentidoHorario = true;
                girandoSentidoContrario = false;
                tamborEstatico = false;

                DLog("El tambor está girando en sentido horario.");
            }
            else if (velocidadRotacionTambor < 0)
            {
                girandoSentidoHorario = false;
                girandoSentidoContrario = true;
                tamborEstatico = false;

                DLog("El tambor está girando en sentido antihorario.");
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
