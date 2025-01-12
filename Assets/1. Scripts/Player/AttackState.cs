using UnityEngine;

public class AttackState : IPlayerState
{
    float timer;
    Vector3 direction;
    public void ExitState(PlayerMovement player)
    {
        Vector3 stopVelocity = Vector3.zero;
        stopVelocity.y = player.rb.linearVelocity.y;
        player.rb.linearVelocity =  stopVelocity;
    }

    public void FixedUpdateState(PlayerMovement player)
    {
    }

    public void StartState(PlayerMovement player)
    {
        timer = 0;
        Vector3 forward = Camera.main.transform.forward;
        Vector3 right = Camera.main.transform.right;
        //camara vector
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        Vector3 moveInput = player.input.GetMoveInput();
        direction = forward * moveInput.z + right * moveInput.x;
        direction *= player.attackMoveVelocity;
        direction.y = 0f;
        player.rb.AddForce(direction, ForceMode.VelocityChange);

    }

    public void UpdateState(PlayerMovement player)
    {
        player.model.transform.rotation = Quaternion.Slerp(player.model.transform.rotation, Quaternion.LookRotation(direction), player.rotationSpeed);

        timer += Time.deltaTime;
        if (timer > player.attackDuration)
        {
            player.ChangeState(player.stateIdle);
        }
    }
}

