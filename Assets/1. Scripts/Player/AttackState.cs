using UnityEngine;

public class AttackState : IPlayerState
{
    float timer;
    bool attack1;
    Vector3 direction;
    Vector3 right;
    Vector3 forward;
    public void ExitState(PlayerMovement player)
    {
        Vector3 stopVelocity = Vector3.zero;
        stopVelocity.y = player.rb.linearVelocity.y;
        player.rb.linearVelocity = stopVelocity;
    }

    public void FixedUpdateState(PlayerMovement player)
    {
    }

    public void StartState(PlayerMovement player)
    {
        attack1 = !attack1;
        timer = 0;
         forward = Camera.main.transform.forward;
         right = Camera.main.transform.right;
        //camara vector
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        Vector3 moveInput = player.input.GetMoveInput();
        if (moveInput.magnitude > 0.1f)
        {
            //si se mueve
            direction = forward * moveInput.z + right * moveInput.x;
            direction *= player.attackMoveVelocity;
            direction.y = 0f;
            player.rb.AddForce(direction, ForceMode.VelocityChange);
        }
        else
        {
            direction = forward * moveInput.z + right * moveInput.x;
            direction *= player.attackMoveVelocity;

        }

        //animation
        if (attack1)
        {
            player.ChangeAnimation("Attack", 0);
            player.slashAttack.Play();

            AudioImp.Instance.Reproducir("PlayerSword");
            //player.Sound.PlayOneShot(player.Sound.Attack,player.gameObject);
        }
        else
        {
            player.ChangeAnimation("Attack 2", 0);
            player.slashAttack2.Play();

            AudioImp.Instance.Reproducir("PlayerSword");
            // player.Sound.PlayOneShot(player.Sound.Attack,player.gameObject);



        }

    }

    public void UpdateState(PlayerMovement player)
    {
        Vector3 moveInput = player.input.GetMoveInput();
        if (moveInput.magnitude > 0.1f)
        {
            player.model.transform.rotation = Quaternion.Slerp(player.model.transform.rotation, Quaternion.LookRotation(direction), player.rotationSpeed);

        }
        else
        {
            player.model.transform.rotation = Quaternion.Slerp(player.model.transform.rotation, Quaternion.LookRotation(forward), player.rotationSpeed);

        }


        timer += Time.deltaTime;
        if (timer > player.attackDuration)
        {
            player.ChangeState(player.stateIdle);
        }
    }
}


