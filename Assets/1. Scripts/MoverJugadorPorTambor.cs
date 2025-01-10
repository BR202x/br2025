using UnityEngine;

public class MoverJugadorPorTambor : MonoBehaviour
{
    public bool mostrarDebug;
    [Space]
    private Rigidbody rigidJugador;

    public DetectarRotacionTambor detectarRotacionTambor;

    [Header("Fuerza Tambor")]
    public float fuerzaTambor = 5f; // Empuje que causa el tambor al jugador
    public ForceMode modoFuerza = ForceMode.Acceleration; // NOTA: Probando cuál fuerza se siente mejor
 
    

    private void Start()
    {
        rigidJugador = GetComponent<Rigidbody>();
        rigidJugador.useGravity = true; // Dependerá de tu diseño
    }

    private void FixedUpdate()
    {
        if (detectarRotacionTambor != null)
        {
            Vector3 fuerza = Vector3.zero;

            if (detectarRotacionTambor.girandoSentidoHorario)
            {
                fuerza = Vector3.right * fuerzaTambor;
                DLog("Aplicando fuerza hacia la derecha (X+)");
            }
            else if (detectarRotacionTambor.girandoSentidoContrario)
            {
                fuerza = Vector3.left * fuerzaTambor;
                DLog("Aplicando fuerza hacia la izquierda (X-)");
            }

            if (fuerza != Vector3.zero)
            {
                rigidJugador.AddForce(fuerza, modoFuerza);
            }
        }
    }

    private void DLog(string texto)
    {
        if (mostrarDebug)
        Debug.Log($"[MoverJugadorPorTambor]: {texto}");
    }
}
