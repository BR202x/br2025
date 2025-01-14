using UnityEngine;

public class IdleState : IPlayerState
{

    public void FixedUpdateState(PlayerMovement player)
    {

    }

    public void StartState(PlayerMovement player)
    {
       

    }

    public void UpdateState(PlayerMovement player)
    {
        Vector3 speedTarget = Vector3.zero;
        speedTarget.y = player.rb.linearVelocity.y;
        player.rb.linearVelocity = Vector3.SmoothDamp(player.rb.linearVelocity, speedTarget, ref player.currentVelocity, player.moveSmooth);

        if (player.input.GetMoveInput().magnitude != 0)
        {
            player.ChangeState(player.stateWalk);
        }
        if (!player.IsShield())
        {
            player.ChangeAnimation("Idle");

        }
    }
    public void ExitState(PlayerMovement player)
    {

    }
}
