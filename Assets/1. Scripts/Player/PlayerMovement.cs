using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.XR;

public class PlayerMovement : MonoBehaviour
{
    [Header("Entradas")]
    IPlayerState currentState;
    [HideInInspector] public IdleState stateIdle = new IdleState();
    [HideInInspector] public WalkState stateWalk = new WalkState();
    [HideInInspector] public DashState stateDash = new DashState();
    [HideInInspector] public AttackState stateAttack = new AttackState();

    [HideInInspector] public Rigidbody rb;
    [HideInInspector] public InputReader input;

    [Header("Parametros")]
    [Header("movimiento")]
    [HideInInspector] public float moveSpeed = 0;
    public float moveShieldSpeed = 4;
    public float moveNormalSpeed = 10;
    public float rotationSpeed;
    public float moveSmooth;
    private bool isAiming;
    [Header("Dash")]
    public float dashForce;
    public float dashDuration;
    [Header("Ataque")]
    public float attackMoveVelocity = 5;
    public float attackDuration = 0.3f;
    public float attackMoveSmooth;

    public int attackDamage = 5;

    [HideInInspector] public Vector3 currentVelocity;
    [HideInInspector] public float currentRotation;
    [HideInInspector] public Transform model;
    [HideInInspector] public CameraController controllerCam;
    private Animator anim;
    private string currentAnimation;

    [Header("Testing y debug")]
    [SerializeField] TMP_Text currentStateText;




    private void Awake()
    {
        input = GetComponent<InputReader>();
        anim = transform.GetChild(0).GetComponent<Animator>();
        model = transform.GetChild(0);
        rb = GetComponent<Rigidbody>();
        controllerCam = GetComponent<CameraController>();

    }
    void Start()
    {
        //ocultar cursor
        Cursor.lockState = CursorLockMode.Locked;
        //eventos
        input.OnDash += Dash;
        input.OnAttack += Attack;
        input.OnDefense += Shield;
        input.OnNoDefense += UnShield;

        //estados
        currentState = stateIdle;
        currentState.StartState(this);
    }

    void Update()
    {
        currentState.UpdateState(this);
        currentStateText.text = currentState.ToString();

    }

    private void FixedUpdate()
    {
        currentState.FixedUpdateState(this);
    }

    public void ChangeState(IPlayerState state)
    {
        currentState.ExitState(this);
        currentState = state;
        currentState.StartState(this);
    }

    public void ChangeAnimation(string animation, float crossfade = 0.3f, float time = 0)
    {
        if (time > 0)
        {
            StartCoroutine(Wait());
        }
        else
        {
            Validate();
        }

        IEnumerator Wait()
        {
            yield return new WaitForSeconds(time);
            Validate();
        }
        void Validate()
        {

            if (currentAnimation != animation)
            {
                currentAnimation = animation;
                anim.CrossFade(animation, crossfade);
            }
        }
    }
    public string GetCurrentAnimation()
    {
        Debug.Log(currentAnimation);
        return currentAnimation;
    }
    public IPlayerState GetCurrentState()
    {
        return currentState;

    }

    private void Dash(object sender, EventArgs e)
    {
        if (currentState == stateWalk)
        {
            ChangeState(stateDash);
        }
    }
    private void Attack(object sender, EventArgs e)
    {
        if (IsShield()) { return; }


        if (currentState == stateWalk || currentState == stateIdle)
        {
            Debug.Log("atacando");

            ChangeState(stateAttack);
        }
    }
    public bool IsShield()
    {
        return isAiming;
    }
    private void Shield(object sender, EventArgs e)
    {
        if (controllerCam.GetCanShield())
        {
            isAiming = true;
            moveSpeed = moveShieldSpeed;

        }

    }
    private void UnShield(object sender, EventArgs e)
    {
        isAiming = false;
        moveSpeed = moveNormalSpeed;
        ChangeAnimation("Unshield");

    }

}
