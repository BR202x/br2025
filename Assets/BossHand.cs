using System.Collections.Generic;
using UnityEngine;

public class BossHand : MonoBehaviour
{
    public enum HandState
    {
        Idle,
        FollowPunch,
        Stunned,
        SpinAttack,
        FingerShooter
    }
    public HandState stateHand = HandState.Idle;
    public Transform idlePosition;
    public List<Transform> spiningsPosition = new List<Transform>();
    [HideInInspector]public Transform spinPosition;
    public Transform shootPosition;
     Transform playerTransform;
    public Transform floorTransform;
    Transform parentTransform;
    public float speedIdle;
    public float speedFollow;
    public float speedPunch;
    public float speedRotation;
    public float playerOffsetY;
    public float shootCadence;
    public GameObject bubblePrefab;
    float timer;
    int shots;

    public float timeMaxStunned;
    public float timeMaxSpining;

    private void Awake()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        parentTransform = transform.parent;
    }
    private void FixedUpdate()
    {
        switch (stateHand)
        {
            case HandState.Idle:
                //Idle
                transform.position = Vector3.Lerp(transform.position, idlePosition.position, speedIdle * Time.fixedDeltaTime);
                transform.rotation = Quaternion.Lerp(transform.rotation, idlePosition.rotation, speedFollow * Time.fixedDeltaTime);


                break;
            case HandState.FollowPunch:
                //followingPlayer
                timer += Time.fixedDeltaTime;

                if (timer > 3.5f)
                {
                    Vector3 punchPos = new Vector3(transform.position.x, -0.2f, transform.position.z);
                    transform.position = Vector3.Lerp(transform.position, punchPos, speedPunch * Time.fixedDeltaTime);
                    if (transform.position == punchPos)
                    {
                        ChangeState(HandState.Stunned);
                    }

                }
                else
                {
                    Vector3 playerTarget = new Vector3(playerTransform.position.x, playerTransform.position.y + Vector3.up.y * playerOffsetY, playerTransform.position.z);
                    transform.position = Vector3.Lerp(transform.position, playerTarget, speedFollow * Time.fixedDeltaTime);
                }


                break;
            case HandState.SpinAttack:
                //spining
                transform.position = Vector3.Lerp(transform.position, spinPosition.position, speedFollow * Time.fixedDeltaTime);
                transform.rotation = Quaternion.Lerp(transform.rotation, spinPosition.rotation, speedFollow * Time.fixedDeltaTime);
                timer += Time.fixedDeltaTime;
                if(timer > timeMaxSpining)
                {
                    ChangeState(HandState.Idle);
                }
                break;
            case HandState.Stunned:
                timer += Time.fixedDeltaTime;
                transform.parent = floorTransform;
                if (timer > timeMaxStunned)
                {
                    ChangeState(HandState.Idle);
                    timer = 0;
                    transform.parent = parentTransform;
                }
                break;
            case HandState.FingerShooter:
                transform.position = Vector3.Lerp(transform.position, shootPosition.position, speedIdle * Time.fixedDeltaTime);
                transform.LookAt(playerTransform);
                timer += Time.fixedDeltaTime;
                if (timer > shootCadence && shots < 10)
                {
                    //disparar
                    Instantiate(bubblePrefab, transform.position, transform.rotation);
                    shots++;
                    timer = 0;
                }
                if (shots == 10)
                {
                    ChangeState(HandState.Idle);
                    shots = 0;
                }

                break;


        }
    }
    public void ChangeState(HandState state)
    {
        timer = 0;
        stateHand = state;
        
    }

    public void ChangeState(int state)
    {
        timer = 0;
        stateHand = (HandState)state;
    }
}
