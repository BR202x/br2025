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

    [Header("Configuración de Lavado")]
    public float velocidadNormal = 10f;
    [Space]
    public float factorVelocidadReducida = 0.5f;
    public float duracionRotacionNormal = 2f;
    public float duracionRotacionReducida = 3f;

    [Header("Configuración de Enjuague")]
    public float velocidadEnjuague = 5f;
    [Space]
    public float duracionGiro = 2f;
    public float pausaEntreGiros = 0.5f;

    private Coroutine cicloActivo; // Referencia a la corrutina activa

    public bool estaRotando = false;
    public bool reproducirAudio1 = false;
    public bool reproducirAudio2 = false;
    public DeteccionAguaMotor deteccionAgua;

    #endregion

    private void Start()
    {
        if (mostrarLog) { Debug.Log("[RotacionSuperficieController]: Componente del objeto " + gameObject.name); }

        deteccionAgua = GameObject.Find("OV_DeteccionAguaSonido").GetComponent<DeteccionAguaMotor>();
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

        if (estaRotando && !deteccionAgua.sobreAgua)
        {
            ReproducirSonidoGiro();
        }
        else if (estaRotando && deteccionAgua.sobreAgua)
        { 
            ReproducirSonidoGiroEnAgua();
        }
    }

    #region Ciclos de Lavadora

    public void IniciarSinRotacion()
    {
        DetenerCiclo();
        if (mostrarLog) { Debug.Log("[RotacionSuperficieController]: Deteniendo rotación."); }
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
            // Pausar si el juego está pausado
            while (ControladorScripts.instance.isPaused)
            {
                yield return null;
            }

            // Primer ciclo de rotación (normal)
            estaRotando = false;
            float tiempo = 0f;

            while (tiempo < duracionRotacionNormal)
            {
                while (ControladorScripts.instance.isPaused)
                {
                    yield return null; // Espera si el juego está pausado
                }

                estaRotando = true;
                superficieTambor.transform.Rotate(0, velocidadNormal * Time.deltaTime, 0);
                tiempo += Time.deltaTime;
                yield return null;
            }

            // Pausa entre ciclos
            estaRotando = false;
            reproducirAudio1 = false;
            reproducirAudio2 = false;
            while (ControladorScripts.instance.isPaused)
            {
                yield return null;
            }
            yield return null;

            // Segundo ciclo de rotación (reducida)
            tiempo = 0f;

            while (tiempo < duracionRotacionReducida)
            {
                while (ControladorScripts.instance.isPaused)
                {
                    yield return null; // Espera si el juego está pausado
                }

                estaRotando = true;
                superficieTambor.transform.Rotate(0, -velocidadNormal * factorVelocidadReducida * Time.deltaTime, 0);
                tiempo += Time.deltaTime;
                yield return null;
            }

            // Pausa entre ciclos
            estaRotando = false;
            reproducirAudio1 = false;
            reproducirAudio2 = false;
            while (ControladorScripts.instance.isPaused)
            {
                yield return null;
            }
            yield return null;
        }
    }



    private IEnumerator CicloEnjuague()
    {
        while (true)
        {
            // Verifica el estado de pausa antes de iniciar cada acción
            while (ControladorScripts.instance.isPaused)
            {
                yield return null; // Esperar mientras el juego esté pausado
            }

            float tiempo = 0f;

            // Primer giro (positivo)
            while (tiempo < duracionGiro)
            {
                // Verifica el estado de pausa en cada frame
                if (ControladorScripts.instance.isPaused)
                {
                    yield return null;
                    continue;
                }

                estaRotando = true;
                superficieTambor.transform.Rotate(0, velocidadEnjuague * Time.deltaTime, 0);
                tiempo += Time.deltaTime;
                yield return null;
            }

            // Pausa entre giros
            estaRotando = false;
            reproducirAudio1 = false;
            reproducirAudio2 = false;
            while (ControladorScripts.instance.isPaused)
            {
                yield return null; // Esperar mientras el juego esté pausado
            }
            yield return new WaitForSeconds(pausaEntreGiros);

            // Segundo giro (negativo)
            tiempo = 0f;
            while (tiempo < duracionGiro)
            {
                if (ControladorScripts.instance.isPaused)
                {
                    yield return null;
                    continue;
                }

                estaRotando = true;
                superficieTambor.transform.Rotate(0, -velocidadEnjuague * Time.deltaTime, 0);
                tiempo += Time.deltaTime;
                yield return null;
            }

            // Pausa entre giros
            estaRotando = false;
            reproducirAudio1 = false;
            reproducirAudio2 = false;
            while (ControladorScripts.instance.isPaused)
            {
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

    public void ReproducirSonidoGiro()
    {
        if (!reproducirAudio1)
        {
            AudioImp.Instance.Reproducir("Motor");
            Debug.Log("Sonido1");
            reproducirAudio1 = true;
        }
    }
    public void ReproducirSonidoGiroEnAgua()
    {
        if (!reproducirAudio2)
        {
            AudioImp.Instance.Reproducir("Motor");
            AudioImp.Instance.Reproducir("MotorAgua");
            Debug.Log("Sonido2");
            reproducirAudio2 = true;
        }
    }
}






