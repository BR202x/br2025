using System.Collections.Generic;
using UnityEngine;

public class BossHand : MonoBehaviour, Idamageable
{
    public enum HandState
    {
        Idle,
        FollowPunch,
        Stunned,
        SpinAttack,
        FingerShooter
    }
    public Collider hitbox;
    public HandState stateHand = HandState.Idle;
    public Transform idlePosition;
    public List<Transform> spiningsPosition = new List<Transform>();
    [HideInInspector] public Transform spinPosition;
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
    public float punchOffsetY;
    public GameObject bubblePrefab;
    float timer;
    int shots;
    public int damageAttack;
    [HideInInspector] public Animator animator;

    public float timeMaxStunned;
    public float timeMaxSpining;
    Boss boss;

    public void Configure(Boss bossRef)
    {
        boss = bossRef;
    }

    private void Awake()
    {
        hitbox = GetComponent<Collider>();
        animator = GetComponent<Animator>();
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

                if (timer > 3f)
                {
                    if(hitbox.isTrigger == false)
                    {
                    hitbox.isTrigger = true;
                    }
                    Vector3 punchPos = new Vector3(transform.position.x, punchOffsetY, transform.position.z);
                    transform.position = Vector3.Lerp(transform.position, punchPos, speedPunch * Time.fixedDeltaTime);
                    if (transform.position == punchPos)
                    {
                        ChangeState(HandState.Stunned);
                    }
                    animator.SetTrigger("Punch");

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
                if (timer > timeMaxSpining)
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
                if (shots >= 10)
                {
                    ChangeState(HandState.Idle);
                    shots = 0;
                }
                if (timer > shootCadence && shots < 10)
                {
                    //disparar
                    Instantiate(bubblePrefab, transform.position, transform.rotation);
                    animator.SetTrigger("Shoot");


                    shots++;
                    timer = 0;
                }
               

                break;


        }
    }
    public void ChangeState(HandState state)
    {
        timer = 0;
        stateHand = state;
        switch (stateHand)
        {
            case HandState.Idle:
                animator.CrossFade("Idle", 0.3f);
                hitbox.isTrigger = true;

                break;
            case HandState.FollowPunch:
                animator.CrossFade("FollowPunch", 0.3f);
                hitbox.isTrigger = false;

                break;
            case HandState.FingerShooter:
                animator.CrossFade("ShootPose", 0.3f);
                hitbox.isTrigger = false;


                break;
            case HandState.SpinAttack:
                animator.CrossFade("Spining", 0.3f);
                hitbox.isTrigger = true;

                break;
                case HandState.Stunned:
                hitbox.isTrigger = false;
                break;


        }



    }

    public void DealDamage(int damage)
    {
        boss.health.DealDamage(damage);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<PlayerMovement>(out PlayerMovement target))
        {
            target.GetComponent<Idamageable>().DealDamage(damageAttack);
        }
    }
}
