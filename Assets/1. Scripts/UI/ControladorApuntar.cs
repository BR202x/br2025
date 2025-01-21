using System.Collections;
using UnityEngine;
using TMPro;

public class ControladorApuntar : MonoBehaviour
{
    [Header("Depuraci�n")]
    public bool mostrarLog;

    #region Variables

    [Header("Configuraci�n del Raycast")]
    public float distanciaMaxima = 50f;
    public LayerMask capaEnemigos;
    public bool mostrarRaycast = true;

    [Header("Referencias de UI")]
    public GameObject panelInfoEnemigo;
    public Transform panelVidaEnemigo;
    public GameObject prefabVida;
    public TextMeshProUGUI textoNombreEnemigo;

    [Header("Configuraci�n del Temporizador")]
    public float tiempoParaOcultar = 5f;

    [Header("C�mara")]
    public GameObject camaraApuntar;
    public Camera camaraMain;
    public GameObject escudoRebote;

    private Coroutine ocultarInfoCoroutine;

    #endregion

    public void Start()
    {
        if (!camaraApuntar.activeSelf)
        {
            escudoRebote.SetActive(false);
        }
    }


    private void Update()
    {
        if (camaraApuntar.gameObject.activeSelf)
        {
            escudoRebote.SetActive(true);

            // Calcular el punto desplazado en la pantalla
            Vector3 puntoPantalla = new Vector3(Screen.width / 2, Screen.height / 2 - 7f, 0f);

            // Generar el raycast desde la c�mara al punto desplazado
            Ray ray = camaraMain.ScreenPointToRay(puntoPantalla);
            RaycastHit hit;

            if (mostrarRaycast)
            {
                Debug.DrawRay(ray.origin, ray.direction * distanciaMaxima, Color.red);
            }

            if (Physics.Raycast(ray, out hit, distanciaMaxima, capaEnemigos))
            {
                ControladorVidaEnemigo enemigo = hit.collider.GetComponent<ControladorVidaEnemigo>();
                if (enemigo != null)
                {
                    enemigo.ActivarPuntero();
                }

                 /*
                ControladorVidaEnemigo enemigo = hit.collider.GetComponent<ControladorVidaEnemigo>();
                if (enemigo != null)
                {
                    ActualizarUIEnemigo(enemigo);
                }
                */
            }
        }
        else
        {
            escudoRebote.SetActive(false);

            if (ocultarInfoCoroutine == null)
            {
                ocultarInfoCoroutine = StartCoroutine(TemporizadorOcultarUI());
            }
        }
    }

    private void ActualizarUIEnemigo(ControladorVidaEnemigo enemigo)
    {
        if (ocultarInfoCoroutine != null)
        {
            StopCoroutine(ocultarInfoCoroutine);
            ocultarInfoCoroutine = null;
        }

        textoNombreEnemigo.text = enemigo.nombreEnemigo;

        foreach (Transform child in panelVidaEnemigo)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < enemigo.vidaActual; i++)
        {
            Instantiate(prefabVida, panelVidaEnemigo);
        }

        panelInfoEnemigo.SetActive(true);
    }

    private IEnumerator TemporizadorOcultarUI()
    {
        yield return new WaitForSeconds(tiempoParaOcultar);
        OcultarUIEnemigo();
        ocultarInfoCoroutine = null;
    }

    private void OcultarUIEnemigo()
    {
        panelInfoEnemigo.SetActive(false);
    }
}
