using Cinemachine;
using UnityEngine;

public class TimelineTest : MonoBehaviour
{
    [Tooltip("Reproducir Timeline")]
    public bool test;

    public GameObject timeLineObject;
    public CinemachineVirtualCamera virtualCamera;
    public ChorroTargetController targetController;
    public GameObject follow;
    public GameObject targetChorro;
    public GameObject player;
    [Header("Posiciones")]
    public Transform targetPosTest;
    public Transform targetPosTimeline;
    public Transform playerPosTest;
    public Transform playerPosTimeline;

    void Start()
    {
        if (test)
        {
            timeLineObject.gameObject.SetActive(false);
            targetController.test = true;
            AsignarFollowCinemachine();
            ActivarSecuenciaChorro();
            targetChorro.transform.position = targetPosTest.position;
            player.transform.position = playerPosTest.position;
        }

        if (!test)
        {
            player.transform.position = playerPosTimeline.position;
            targetChorro.transform.position= targetPosTimeline.position;        
        }
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
