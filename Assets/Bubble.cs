using UnityEngine;

public class Bubble : MonoBehaviour
{
    Rigidbody rb;
    public float speed;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.linearVelocity = transform.forward * speed;
    }

    
}
