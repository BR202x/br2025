using Cinemachine;
using UnityEngine;

public class TestIntroCamara : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera;
    public GameObject follow;
    
    void Start()
    {
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
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
}
