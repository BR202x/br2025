using System.Collections;
using UnityEngine;
using TMPro;

public class ControladorApuntar : MonoBehaviour
{
    [Header("Depuración")]
    public bool mostrarLog;

    #region Variables

    [Header("Configuración del Raycast")]
    public float distanciaMaxima = 50f;
    public LayerMask capaEnemigos;
    public bool mostrarRaycast = true;

    [Header("Referencias de UI")]
    public GameObject panelInfoEnemigo;
    public Transform panelVidaEnemigo;
    public GameObject prefabVida;
    public TextMeshProUGUI textoNombreEnemigo;

    [Header("Configuración del Temporizador")]
    public float tiempoParaOcultar = 5f;

    [Header("Cámara")]
    public GameObject camaraApuntar;
    public Camera camaraMain;

    private Coroutine ocultarInfoCoroutine;

    #endregion

    private void Update()
    {
        if (camaraApuntar.gameObject.activeSelf)
        {
            Ray ray = camaraMain.ScreenPointToRay(Input.mousePosition);
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
                    ActualizarUIEnemigo(enemigo);
                }
            }
        }
        else
        {
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
