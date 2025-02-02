using UnityEngine;

public class DamageState : IPlayerState
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
        Vector3 knockBackDirection = (player.transform.forward * -player.knockBackForce);
        Vector3 upForce = (Vector3.up * player.knockBackForce);
        player.rb.AddForce(knockBackDirection, ForceMode.Impulse);
        player.rb.AddForce(upForce, ForceMode.Impulse);
        player.ChangeAnimation("Damage", 0);
    }

    public void UpdateState(PlayerMovement player)
    {
        timer += Time.deltaTime;
        if (timer > player.damageDuration)
        {
            player.ChangeState(player.stateIdle);
            timer = 0;
        }
    }
}
