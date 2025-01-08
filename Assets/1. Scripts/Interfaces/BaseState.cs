using UnityEngine;

public interface IPlayerState
{
    public abstract void StartState(PlayerMovement player);
    public abstract void UpdateState(PlayerMovement player);
    public abstract void FixedUpdateState(PlayerMovement player);
    public abstract void ExitState(PlayerMovement player);
}
