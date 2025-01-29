using UnityEngine;

public class JumpState : IPlayerState
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
        if (moveInput.magnitude > 0.1f) // Si el jugador está moviéndose
        {
            Vector3 speedTarget = new Vector3(direction.x, player.rb.linearVelocity.y, direction.z);
            speedTarget *= player.moveSpeed;
            speedTarget.y = player.rb.linearVelocity.y;
            player.rb.linearVelocity = Vector3.SmoothDamp(player.rb.linearVelocity, speedTarget, ref player.currentVelocity, player.moveSmooth);

            player.model.transform.rotation = Quaternion.Slerp(player.model.transform.rotation, Quaternion.LookRotation(direction), player.rotationSpeed);
        }
        else
        {
            Vector3 speedTarget = new Vector3(0, player.rb.linearVelocity.y, 0);

            player.rb.linearVelocity = Vector3.SmoothDamp(player.rb.linearVelocity, speedTarget, ref player.currentVelocity, player.moveSmooth);

            if (player.rb.linearVelocity.magnitude < 0.1f)
            {
                player.rb.linearVelocity = Vector3.zero;
            }
        }

    }

    public void StartState(PlayerMovement player)
    {
        player.ChangeAnimation("Jump");
        player.rb.AddForce(Vector3.up * player.jumpForce, ForceMode.Impulse);

    }

    public void UpdateState(PlayerMovement player)
    {
       if(player.rb.linearVelocity.y < 0 &&!player.GetIsGround())
        {
            player.ChangeState(player.stateFall);
        }
    }


}
