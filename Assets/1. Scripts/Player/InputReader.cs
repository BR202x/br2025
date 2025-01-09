using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class InputReader : MonoBehaviour
{
    public event EventHandler OnAttack;
    public event EventHandler OnDefense;
    public event EventHandler OnAim;
    public event EventHandler OnDash;
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
    }


    public Vector3 GetMoveInput()
    {

        return new Vector3(controls.Player.Move.ReadValue<Vector2>().x, 0, controls.Player.Move.ReadValue<Vector2>().y);
    }

    public void AttackInteraction(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnAttack?.Invoke(this, EventArgs.Empty);
    }

    public void DefenseInteraction(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnDefense?.Invoke(this, EventArgs.Empty);
    }

    public void AimInteraction(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnAim?.Invoke(this, EventArgs.Empty);
    }
    public void DashInteraction(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnDash?.Invoke(this, EventArgs.Empty);
    }




}
