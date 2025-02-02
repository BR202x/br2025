using UnityEngine;

public class TimelineController : MonoBehaviour
{
    public GameObject timeLineIntro;
    public GameObject timeLineCanon;
    public ChorroTargetController inicioChorro;
    public ControladorScripts controladorScripts;
    public MenuPausaManager menuPausaManager;
    
    public float contadorCanon;
    public bool esCanon;
    void Start()
    {
        contadorCanon = 0;
        inicioChorro = GameObject.Find("Chorro_Manager").GetComponent<ChorroTargetController>();
        controladorScripts = GameObject.Find("GameManager").GetComponent <ControladorScripts>();
        menuPausaManager = GameObject.Find("Controlador_MenuPausa").GetComponent<MenuPausaManager>();        

        if (esCanon)
        {
            controladorScripts.enabled = false;
            menuPausaManager.enabled = false;            
        }
    }
        
    void FixedUpdate()
    {
        if (esCanon)
        { 
            contadorCanon += Time.fixedDeltaTime;

            if (contadorCanon >= 8 && contadorCanon <=9f)
            {                
                controladorScripts.enabled = true;
                menuPausaManager.enabled = true;
            }

            if (contadorCanon >= 18 && timeLineIntro.activeSelf)
            {                
                timeLineCanon.SetActive(true);
                inicioChorro.iniciar = true;
                inicioChorro.EmpezarEstado0();
                timeLineIntro.SetActive(false);
            }        
        }
    }
}
