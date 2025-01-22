using UnityEngine;
using System.Collections;

public class RotacionSuperficieController : MonoBehaviour
{
    #region Variables

    [Tooltip("Objeto Superficie del Tambor")]
    public GameObject superficieTambor;
    [Tooltip("Velocidad de testeo")]    
    public float velocidadTest = 10f;

    [Header("Configuraci�n de Lavado")]
    public float velocidadNormal = 10f;
    [Space]
    public float factorVelocidadReducida = 0.5f;    
    public float duracionRotacionNormal = 2f;    
    public float duracionRotacionReducida = 3f;

    [Header("Configuraci�n de Enjuague")]
    public float velocidadEnjuague = 5f;
    [Space]
    public float duracionGiro = 2f;
    public float pausaEntreGiros = 0.5f;


    private Coroutine cicloActivo; // Referencia a la corrutina activa

    #endregion

    void Update()
    {
        if (Input.GetKey(KeyCode.C))
        {
            IniciarCicloLavado();
        }

        if (Input.GetKey(KeyCode.V))
        {
            IniciarCicloEnjuague();
        }

        if (Input.GetKey(KeyCode.B))
        {
            IniciarCicloCentrifugado();
        }
        // Entrada manual para pruebas (opcional)
        if (Input.GetKey(KeyCode.RightArrow))
        {
            superficieTambor.transform.Rotate(0, velocidadTest * Time.deltaTime, 0);
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            superficieTambor.transform.Rotate(0, -velocidadTest * Time.deltaTime, 0);
        }
    }

    #region Ciclos de Lavadora

    public void IniciarCicloLavado()
    {
        DetenerCiclo(); // Asegura que no haya superposici�n
        cicloActivo = StartCoroutine(CicloLavado());
    }

    public void IniciarCicloEnjuague()
    {
        DetenerCiclo();
        cicloActivo = StartCoroutine(CicloEnjuague());
    }

    public void IniciarCicloCentrifugado()
    {
        DetenerCiclo();
        cicloActivo = StartCoroutine(CicloCentrifugado());
    }

    public void DetenerCiclo()
    {
        if (cicloActivo != null)
        {
            StopCoroutine(cicloActivo);
            cicloActivo = null;
        }
    }


    private IEnumerator CicloLavado()
    {
        while (true)
        {
            // Rotaci�n extensa en sentido horario
            float tiempo = 0f;
            while (tiempo < duracionRotacionNormal)
            {
                superficieTambor.transform.Rotate(0, velocidadNormal * Time.deltaTime, 0);
                tiempo += Time.deltaTime;
                yield return null;
            }

            // Pausa opcional (si la necesitas, descomenta la l�nea siguiente)
            // yield return new WaitForSeconds(1f);

            // Rotaci�n extensa en sentido antihorario con velocidad reducida
            tiempo = 0f;
            while (tiempo < duracionRotacionReducida)
            {
                superficieTambor.transform.Rotate(0, -velocidadNormal * factorVelocidadReducida * Time.deltaTime, 0);
                tiempo += Time.deltaTime;
                yield return null;
            }

            // Pausa opcional (si la necesitas, descomenta la l�nea siguiente)
            // yield return new WaitForSeconds(1f);
        }
    }


    private IEnumerator CicloEnjuague()
    {
        while (true)
        {
            // Rotaci�n en sentido horario
            float tiempo = 0f;
            while (tiempo < duracionGiro)
            {
                superficieTambor.transform.Rotate(0, velocidadEnjuague * Time.deltaTime, 0);
                tiempo += Time.deltaTime;
                yield return null;
            }

            yield return new WaitForSeconds(pausaEntreGiros);

            // Rotaci�n en sentido antihorario
            tiempo = 0f;
            while (tiempo < duracionGiro)
            {
                superficieTambor.transform.Rotate(0, -velocidadEnjuague * Time.deltaTime, 0);
                tiempo += Time.deltaTime;
                yield return null;
            }

            yield return new WaitForSeconds(pausaEntreGiros);
        }
    }


    private IEnumerator CicloCentrifugado()
    {
        while (true)
        {
            // Rotaci�n constante y r�pida en un solo sentido
            superficieTambor.transform.Rotate(0, velocidadTest * 2f * Time.deltaTime, 0);
            yield return null;
        }
    }

    #endregion
}
