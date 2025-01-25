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
    public LayerMask capaValvulas;
    public bool mostrarRaycast = true;

    [Header("Referencias de UI")]
    public GameObject panelInfoEnemigo;
    public Transform panelVidaEnemigo;
    public GameObject prefabVida;
    public TextMeshProUGUI textoNombreEnemigo;

    [Header("Configuración del Temporizador")]
    public float tiempoParaOcultar = 5f;

    [Header("Error Instancias")]
    public GameObject chorroReboteInst;
    public GameObject instLineRenderer;
    public GameObject chorroIsnt;

    [Header("Cámara")]
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
        #region MACHETAZO - Revisar.

        chorroIsnt = GameObject.Find("Chorro de agua(Clone)");
        chorroReboteInst = GameObject.Find("ChorroNew(Clone)");
        instLineRenderer = GameObject.Find("InstanciaChorroEscudo(Clone)");

        if (chorroIsnt == null)
        {
            Destroy(chorroReboteInst);
            Destroy(instLineRenderer);
        }

        #endregion

        if (camaraApuntar.gameObject.activeSelf)
        {
            escudoRebote.SetActive(true);
                        
            Vector3 puntoPantalla = new Vector3(Screen.width / 2, Screen.height / 2 - 7f, 0f);
                        
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
            }

            if (Physics.Raycast(ray, out hit, distanciaMaxima, capaValvulas))
            {
                ControladorGolpeValvula valvula = hit.collider.GetComponent<ControladorGolpeValvula>();                

                if (valvula != null)
                {
                    valvula.ActivarPuntero();                    
                }
            }
        }
        else
        {

            if (chorroReboteInst != null) // Con esto... Machete
            {
                Destroy(chorroReboteInst);
                Destroy(instLineRenderer);            
            }

            escudoRebote.SetActive(false);

            if (ocultarInfoCoroutine == null)
            {
                ocultarInfoCoroutine = StartCoroutine(TemporizadorOcultarUI());
            }
        }
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
