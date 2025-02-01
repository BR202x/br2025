using Unity.VisualScripting;
using UnityEngine;

public class TimelineController : MonoBehaviour
{
    public GameObject timeLineIntro;
    public GameObject timeLineCanon;
    public ChorroTargetController inicioChorro;
    public float contadorCanon;
    public bool esCanon;
    void Start()
    {
        contadorCanon = 0;
        inicioChorro = GameObject.Find("Chorro_Manager").GetComponent<ChorroTargetController>();
    }
        
    void Update()
    {
        if (esCanon)
        { 
            contadorCanon += Time.deltaTime;

            if (contadorCanon >= 15 && timeLineIntro.activeSelf)
            {                
                timeLineCanon.SetActive(true);
                inicioChorro.iniciar = true;
                inicioChorro.EmpezarEstado0();
                timeLineIntro.SetActive(false);
            }        
        }
    }
}
