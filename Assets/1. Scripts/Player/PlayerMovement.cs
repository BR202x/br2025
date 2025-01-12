using System;
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
    public float moveSpeed;
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
    private Animator anim;

    [Header("Testing y debug")]
    [SerializeField] TMP_Text currentStateText;




    private void Awake()
    {
        input = GetComponent<InputReader>();
        anim = transform.GetChild(0).GetComponent<Animator>();
        model = transform.GetChild(0);
        rb = GetComponent<Rigidbody>();

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

    public void ChangeAnimation(string animation, float crossfade = 0.1f)
    {
        anim.CrossFade(animation, crossfade);
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
        isAiming = true;
        Debug.Log("defendiendo");
    }
    private void UnShield(object sender, EventArgs e)
    {
        isAiming = false;
                Debug.Log("defendiendo");


    }

}
