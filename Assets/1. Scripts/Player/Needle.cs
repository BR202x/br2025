using System;
using UnityEngine;

public class Needle : MonoBehaviour
{
    [SerializeField] int damage;
    [SerializeField] float attackDuration = 0.3f;
    private Collider hitBox;
    private float timer;


    private void Awake()
    {
        hitBox = GetComponent<Collider>();
        PlayerMovement.OnAttack += Attack;
    }


    private void OnEnable()
    {
    }

    private void Attack(object sender, EventArgs e)
    {
        hitBox.enabled = true;
        timer = 0;
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if(timer > attackDuration)
        {
            hitBox.enabled = false;
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent<Idamageable>(out Idamageable target))
        {
            target.DealDamage(damage);
        }
    }

    //private void OnCollisionEnter(Collision collision)
    //{
    //    if (collision.collider.TryGetComponent<Idamageable>(out Idamageable target))
    //    {
    //        target.DealDamage(damage);
    //    }
    //}
}
