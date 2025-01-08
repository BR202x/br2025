using UnityEngine;

public class IdleState : IPlayerState
{

    public void FixedUpdateState(PlayerMovement player)
    {

    }

    public void StartState(PlayerMovement player)
    {
        //player.ChangeAnimation("Idle");

    }

    public void UpdateState(PlayerMovement player)
    {
        if (player.input.GetMoveInput().magnitude != 0)
        {
            player.ChangeState(player.stateWalk);
        }
    }
    public void ExitState(PlayerMovement player)
    {

    }
}
