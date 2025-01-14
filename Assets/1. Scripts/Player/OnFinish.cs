using UnityEngine;

public class OnFinish : StateMachineBehaviour
{

    [SerializeField] string animation;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.transform.GetComponentInParent<PlayerMovement>().ChangeAnimation(animation, 0.3f, stateInfo.length);
    }


}
