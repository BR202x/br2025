using UnityEngine;

public class SonidosAtaquesBoss : MonoBehaviour
{
    public BossHand bossR; // Referencia a la mano derecha
    public BossHand bossL; // Referencia a la mano izquierda
    public Animator animatorR; // Animator de la mano derecha
    public Animator animatorL; // Animator de la mano izquierda

    private BossHand.HandState lastStateR;
    private BossHand.HandState lastStateL;
    private bool isPunchingR = false;
    private bool isPunchingL = false;
    private bool isSpiningR = false;
    private bool isSpiningL = false;

    private void Start()
    {
        if (bossR != null)
            lastStateR = bossR.stateHand;

        if (bossL != null)
            lastStateL = bossL.stateHand;
    }

    private void Update()
    {
        if (bossR != null && lastStateR != bossR.stateHand && bossR.stateHand == BossHand.HandState.FollowPunch)
        {
            AudioImp.Instance.Reproducir("BossFistMove");
            lastStateR = bossR.stateHand;
        }

        if (bossL != null && lastStateL != bossL.stateHand && bossL.stateHand == BossHand.HandState.FollowPunch)
        {
            AudioImp.Instance.Reproducir("BossFistMove");
            lastStateL = bossL.stateHand;
        }

        if (animatorR != null)
        {
            AnimatorStateInfo stateInfoR = animatorR.GetCurrentAnimatorStateInfo(0);
            if (stateInfoR.IsName("Punch") && !isPunchingR)
            {
                AudioImp.Instance.Reproducir("BossFistAttack");
                isPunchingR = true;
            }
            else if (!stateInfoR.IsName("Punch"))
            {
                isPunchingR = false;
            }

            if (stateInfoR.IsName("Spining") && !isSpiningR)
            {
                AudioImp.Instance.Reproducir("BossFistAttack");
                isSpiningR = true;
            }
            else if (!stateInfoR.IsName("Spining"))
            {
                isSpiningR = false;
            }
        }

        if (animatorL != null)
        {
            AnimatorStateInfo stateInfoL = animatorL.GetCurrentAnimatorStateInfo(0);
            if (stateInfoL.IsName("Punch") && !isPunchingL)
            {
                AudioImp.Instance.Reproducir("BossFistAttack");
                isPunchingL = true;
            }
            else if (!stateInfoL.IsName("Punch"))
            {
                isPunchingL = false;
            }

            if (stateInfoL.IsName("Spining") && !isSpiningL)
            {
                AudioImp.Instance.Reproducir("BossFistAttack");
                isSpiningL = true;
            }
            else if (!stateInfoL.IsName("Spining"))
            {
                isSpiningL = false;
            }
        }
    }
}
