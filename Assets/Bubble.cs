using UnityEngine;

public class Bubble : MonoBehaviour
{
    Rigidbody rb;
    public float speed;
    public int damage;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.linearVelocity = transform.forward * speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<PlayerMovement>(out PlayerMovement target))
        {
            target.GetComponent<Idamageable>().DealDamage(damage);
        }
    }


}
