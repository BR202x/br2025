using UnityEngine;

public class WalkState : IPlayerState
{
    public void ExitState(PlayerMovement player)
    {
    }

    public void FixedUpdateState(PlayerMovement player)
    {
        Vector3 forward = Camera.main.transform.forward;
        Vector3 right = Camera.main.transform.right;

        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        Vector3 moveInput = player.input.GetMoveInput();

        Vector3 direction = forward * moveInput.z + right * moveInput.x;
        Vector3 SpeedTarget = new Vector3(direction.x, player.rb.linearVelocity.y, direction.z);
        SpeedTarget *= player.moveSpeed;
        SpeedTarget.y = player.rb.linearVelocity.y;
        player.rb.linearVelocity = Vector3.SmoothDamp(player.rb.linearVelocity, SpeedTarget, ref player.currentVelocity, player.moveSmooth);
        player.model.transform.rotation = Quaternion.Slerp(player.model.transform.rotation, Quaternion.LookRotation(direction), player.rotationSpeed); 
    }

    public void StartState(PlayerMovement player)
    {
        //player.ChangeAnimation("Walk");

    }

    public void UpdateState(PlayerMovement player)
    {

        if (player.input.GetMoveInput().magnitude == 0)
        {
            player.ChangeState(player.stateIdle);
        }
        
    }
}
