using Cinemachine;
using UnityEngine;

public class TimelineTest : MonoBehaviour
{
    [Tooltip("Reproducir Timeline")]
    public bool test;
    public bool esCanon;
    public bool esPato;

    public GameObject timeLineObject;
    public CinemachineVirtualCamera virtualCamera;
    public ChorroTargetController targetController;
    public TimelineController timelineController;

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
        timelineController = GameObject.Find("TimelineManager").GetComponent<TimelineController>();

        if (esCanon)
        {
            if (test)
            {
                timelineController.enabled = false;
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
                targetChorro.transform.position = targetPosTimeline.position;
                timelineController.enabled = true;
            }
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
