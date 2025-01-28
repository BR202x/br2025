using UnityEngine;

public class ActivacionPanelTargets : MonoBehaviour
{
    public GameObject canvasTarget;

    public void activarCanvasTarget()
    { 
        canvasTarget.SetActive(true);
    }  
}
