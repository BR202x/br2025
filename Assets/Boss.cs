using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR;
using static BossHand;

public class Boss : MonoBehaviour
{
    public float timer;
    public float timeToChangeAttack = 5;
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
    public void TryAttack(int attackleft, int attackRight)
    {
        if (attackleft == 1)// 1 = followPunch
        {

            if (leftHand.stateHand == BossHand.HandState.Idle && rightHand.stateHand != BossHand.HandState.FollowPunch)
            {
                leftHand.ChangeState(BossHand.HandState.FollowPunch);
            }
            else if (leftHand.stateHand != BossHand.HandState.Idle)
            {
                Debug.Log("mano izquierda ocupada");
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
            else if (rightHand.stateHand != BossHand.HandState.Idle)
            {
                Debug.Log("mano derecha ocupada");

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
