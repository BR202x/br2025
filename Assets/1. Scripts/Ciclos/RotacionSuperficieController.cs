using UnityEngine;
using System.Collections;

// EDITADO: 29/01/2025
public class RotacionSuperficieController : MonoBehaviour
{
    [Header("depuracion")]
    public bool mostrarLog = false;

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

    private void Start()
    {
        if (mostrarLog) { Debug.Log("[RotacionSuperficieController]: Componente del objeto " + gameObject.name); }
    }

    private void Update()
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

    public void IniciarSinRotacion()
    {
        DetenerCiclo();
        if (mostrarLog) { Debug.Log("[RotacionSuperficieController]: Deteniendo rotaci�n."); }
    }

    public void IniciarCicloLavado()
    {
        DetenerCiclo();
        cicloActivo = StartCoroutine(CicloLavado());
        if (mostrarLog) { Debug.Log("[RotacionSuperficieController]: Iniciando ciclo de lavado."); }
    }

    public void IniciarCicloEnjuague()
    {
        DetenerCiclo();
        cicloActivo = StartCoroutine(CicloEnjuague());
        if (mostrarLog) { Debug.Log("[RotacionSuperficieController]: Iniciando ciclo de enjuague."); }
    }

    public void IniciarCicloCentrifugado()
    {
        DetenerCiclo();
        cicloActivo = StartCoroutine(CicloCentrifugado());
        if (mostrarLog) { Debug.Log("[RotacionSuperficieController]: Iniciando ciclo de centrifugado."); }
    }

    public void DetenerCiclo()
    {
        if (cicloActivo != null)
        {
            StopCoroutine(cicloActivo);
            cicloActivo = null;
            if (mostrarLog) { Debug.Log("[RotacionSuperficieController]: Ciclo detenido."); }
        }
    }

    private IEnumerator CicloLavado()
    {
        while (true)
        {
            float tiempo = 0f;
            while (tiempo < duracionRotacionNormal)
            {
                superficieTambor.transform.Rotate(0, velocidadNormal * Time.deltaTime, 0);
                tiempo += Time.deltaTime;
                yield return null;
            }

            tiempo = 0f;
            while (tiempo < duracionRotacionReducida)
            {
                superficieTambor.transform.Rotate(0, -velocidadNormal * factorVelocidadReducida * Time.deltaTime, 0);
                tiempo += Time.deltaTime;
                yield return null;
            }
        }
    }

    private IEnumerator CicloEnjuague()
    {
        while (true)
        {
            float tiempo = 0f;
            while (tiempo < duracionGiro)
            {
                superficieTambor.transform.Rotate(0, velocidadEnjuague * Time.deltaTime, 0);
                tiempo += Time.deltaTime;
                yield return null;
            }

            yield return new WaitForSeconds(pausaEntreGiros);

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
            superficieTambor.transform.Rotate(0, velocidadTest * 2f * Time.deltaTime, 0);
            yield return null;
        }
    }

    #endregion
}
