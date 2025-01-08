using UnityEngine;
using UnityEngine.XR;

public class PlayerMovement : MonoBehaviour
{
    [Header("Entradas")]
    IPlayerState currentState;
    [HideInInspector] public IdleState stateIdle = new IdleState();
    [HideInInspector] public WalkState stateWalk = new WalkState();

    [HideInInspector] public Rigidbody rb;
    [HideInInspector] public InputReader input;

    [Header("Parametros")]
    public float moveSpeed;
    public float rotationSpeed;
    public float moveSmooth;
    [HideInInspector] public Vector3 currentVelocity;
    [HideInInspector] public float currentRotation;
    [HideInInspector] public Transform model;
    private Animator anim;
    

    private void Awake()
    {
        input = GetComponent<InputReader>();
        anim = transform.GetChild(0).GetComponent<Animator>();
        model = transform.GetChild(0);
        rb = GetComponent<Rigidbody>();
       
    }
    void Start()
    {
        currentState = stateIdle;
        currentState.StartState(this);
    }

    void Update()
    {
        currentState.UpdateState(this);
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
}
