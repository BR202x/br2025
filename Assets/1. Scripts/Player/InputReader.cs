using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class InputReader : MonoBehaviour
{
    public event EventHandler OnAttack;
    public event EventHandler OnDefense;
    public event EventHandler OnNoDefense;
    public event EventHandler OnDash;
    public event EventHandler OnJump;
    PlayerControls controls;


    private void Awake()
    {
        controls = new PlayerControls();
        controls.Player.Enable();

    }
    private void Start()
    {
        controls.Player.MeleeAttack.started += AttackInteraction;
        controls.Player.Dash.started += DashInteraction;
        controls.Player.Defense.started += DefenseInteraction;
        controls.Player.Defense.canceled += NoDefenseInteraction;
        controls.Player.Jump.started += JumpInteraction;
    }


    public Vector3 GetMoveInput()
    {

        return new Vector3(controls.Player.Move.ReadValue<Vector2>().x, 0, controls.Player.Move.ReadValue<Vector2>().y);
    }
    public Vector2 GetMouseInput()
    {
        return controls.Player.CameraMove.ReadValue<Vector2>();
    }
    public void AttackInteraction(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnAttack?.Invoke(this, EventArgs.Empty);
    }

    public void DefenseInteraction(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnDefense?.Invoke(this, EventArgs.Empty);
    }

    public void NoDefenseInteraction(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnNoDefense?.Invoke(this, EventArgs.Empty);
    }
    public void DashInteraction(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnDash?.Invoke(this, EventArgs.Empty);
    }
    public void JumpInteraction(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnJump?.Invoke(this, EventArgs.Empty);
    }



}
