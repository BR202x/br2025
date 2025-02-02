using JetBrains.Annotations;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR;
using static BossHand;

public class Boss : MonoBehaviour
{
    public float timer;
    public float timeToChangeAttack = 5;
    private Animator anim;
    private enum BossState
    {
        intro,
        Idle,
        damage,
        dead
    }
    private BossHand.HandState handState;

    public BossHand rightHand;
    public BossHand leftHand;
    public HealthController health;

    private void Awake()
    {
        health = GetComponent<HealthController>();
        rightHand.Configure(this);
        leftHand.Configure(this);
        anim = GetComponent<Animator>();

    }
    private void Start()
    {
        health.OnDamage.AddListener(DamageAnimation);
        health.OnDead.AddListener(DeadAnimation);
    }


    public void DamageAnimation()
    {
        if(health.isDead) return;
        anim.SetTrigger("Damage");
    }
    public void DeadAnimation()
    {
        anim.Play("BossDead");
        DeadHands();
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer > timeToChangeAttack)
        {
            int leftHandAttack = Random.Range(1, 4);
            int rightHandAttack = Random.Range(1, 4);
            TryAttack(leftHandAttack, rightHandAttack);
            timer = 0;
        }

    }
    public void DeadHands()
    {
        leftHand.gameObject.SetActive(false);
        rightHand.gameObject.SetActive(false);
    }
    public void Dead()
    {
        gameObject.SetActive(false);
    }
    public void TryAttack(int attackleft, int attackRight)
    {
        if (attackleft == 1)// 1 = followPunch
        {

            if (leftHand.stateHand == BossHand.HandState.Idle && rightHand.stateHand != BossHand.HandState.FollowPunch)
            {
                leftHand.ChangeState(BossHand.HandState.FollowPunch);
            }

            else
            {
                attackleft = Random.Range(2, 4);
            }
        }
        if (attackRight == 1)
        {
            if (rightHand.stateHand == BossHand.HandState.Idle && leftHand.stateHand != BossHand.HandState.FollowPunch)
            {
                rightHand.ChangeState(BossHand.HandState.FollowPunch);

            }

            else
            {
                attackRight = Random.Range(2, 4);
            }
        }
        if (attackleft == 2)// 2 = spin attack
        {
            if (leftHand.stateHand == HandState.Idle)
            {
                leftHand.spinPosition = leftHand.spiningsPosition[Random.Range(0, leftHand.spiningsPosition.Count)];
                leftHand.ChangeState(BossHand.HandState.SpinAttack);

            }


        }
        if (attackRight == 2)
        {
            if (rightHand.stateHand == HandState.Idle)
            {
                rightHand.spinPosition = rightHand.spiningsPosition[Random.Range(0, rightHand.spiningsPosition.Count)];
                rightHand.ChangeState(BossHand.HandState.SpinAttack);

            }




        }
        if (attackleft == 3)// 3 = shoot
        {
            if (leftHand.stateHand == HandState.Idle)
            {
                leftHand.ChangeState(BossHand.HandState.FingerShooter);


            }


        }
        if (attackRight == 3)
        {
            if (rightHand.stateHand == HandState.Idle)
            {
                rightHand.ChangeState(BossHand.HandState.FingerShooter);

            }


        }


    }


}
