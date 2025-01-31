using Cinemachine;
using UnityEngine;

public class TestIntroCamara : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera;
    public ChorroTargetController targetController;
    public GameObject follow;
    
    void Start()
    {

    }
    
    void Update()
    {
        
    }

    public void AsignarFollowCinemachine()    
    { 
        virtualCamera.Follow = follow.transform;
    }

    public void DesAsignarFollow()
    {
        virtualCamera.Follow = null;
    }

    public void ActivarSecuenciaChorro()
    {
        targetController.enabled = true;    
    }
}
