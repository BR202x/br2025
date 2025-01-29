using UnityEngine;

public class DashState : IPlayerState
{
    float timer;
    public void ExitState(PlayerMovement player)
    {
    }

    public void FixedUpdateState(PlayerMovement player)
    {
    }

    public void StartState(PlayerMovement player)
    {
        Debug.Log("HAciendo Rollo");

        timer = 0;
        Vector3 forward = Camera.main.transform.forward;
        Vector3 right = Camera.main.transform.right;
        //camara vector
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        Vector3 moveInput = player.input.GetMoveInput();
        Vector3 direction = forward * moveInput.z + right * moveInput.x;
        direction.y = 0f;
        player.rb.AddForce(direction * player.dashForce, ForceMode.Impulse);
        player.ChangeAnimation("Roll");

    }

    public void UpdateState(PlayerMovement player)
    {
        Vector3 direction = player.rb.linearVelocity;
        direction.y = 0f;
        player.model.transform.rotation = Quaternion.Slerp(player.model.transform.rotation, Quaternion.LookRotation(direction), player.rotationSpeed);

        timer += Time.deltaTime;
        if (timer > player.dashDuration)
        {
            player.ChangeState(player.stateIdle);
        }
    }
}
