using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using Cinemachine;

public class shield : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] float speedBack;
    [SerializeField] bool hitSomething = false;
    
    Transform playerTransform;
    Rigidbody rb;
    CinemachineImpulseSource cameraShake;


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        cameraShake = GetComponent<CinemachineImpulseSource>();
    }
    public void Configure(Transform player)
    {
        playerTransform = player;
    }
    private void Update()
    {
        if (!hitSomething)
        {
            rb.linearVelocity = transform.forward * speed;
        }
        else if(hitSomething)
        {

            rb.position = Vector3.MoveTowards(transform.position, playerTransform.position, speedBack * Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hitSomething)
        {
            if (other.TryGetComponent<CameraController>(out CameraController player))
            {
                player.RecoverShield();
                cameraShake.GenerateImpulse();
                Destroy(gameObject);
            }
        }
        else
        {
            hitSomething = true;
            rb.linearVelocity = Vector3.zero;

        }

    }

}
