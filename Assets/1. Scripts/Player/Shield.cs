using UnityEngine;
using Cinemachine;

public class Shield : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] float speedBack;
    [SerializeField] int damage;
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
        else if (hitSomething)
        {

            rb.position = Vector3.MoveTowards(transform.position, playerTransform.position + Vector3.up, speedBack * Time.deltaTime);
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
            if(other.TryGetComponent<Idamageable>(out Idamageable target))
            {
                target.DealDamage(damage);
            }
            hitSomething = true;
            rb.linearVelocity = Vector3.zero;

        }

    }

}