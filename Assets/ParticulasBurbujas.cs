using UnityEngine;

public class ParticulasBurbujas : MonoBehaviour
{
    public GameObject particulaBurbujas;
    public float tiempoActivacion = 1.0f; // Tiempo para encender
    public float tiempoDesactivacion = 2.0f; // Tiempo para apagar
    public bool tocandoCollision = false; // Hacerlo público para verlo en el Inspector
    private float contadorBurbujasOn = 0f;
    private float contadorBurbujasOff = 0f;

    public Rigidbody rbJugador;
    public float fuerzaEmpuje = 5.0f;

    void Update()
    {
        if (tocandoCollision)
        {
            contadorBurbujasOn += Time.deltaTime;
            

            if (contadorBurbujasOn >= tiempoActivacion && !particulaBurbujas.activeSelf)
            {
                particulaBurbujas.SetActive(true);            
            }
        }
        else if (particulaBurbujas.activeSelf)
        {
            contadorBurbujasOff += Time.deltaTime;            

            if (contadorBurbujasOff >= tiempoDesactivacion)
            {
                particulaBurbujas.SetActive(false);
                
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            rbJugador = collision.gameObject.GetComponent<Rigidbody>();


            tocandoCollision = true;
            contadorBurbujasOn = 0f;
            contadorBurbujasOff = 0f;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            rbJugador = collision.gameObject.GetComponent<Rigidbody>();

            if (particulaBurbujas.activeSelf && rbJugador != null)
            {
                rbJugador.AddForce(Vector3.up * fuerzaEmpuje, ForceMode.Acceleration);
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            tocandoCollision = false;
            contadorBurbujasOff = 0f;
        }
    }
}
