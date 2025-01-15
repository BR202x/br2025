using UnityEngine;
using UnityEngine.Events;

public class HealthController : MonoBehaviour, Idamageable
{
    [SerializeField]int vida;
    bool isDead;
    public UnityEvent OnDamage;
    public UnityEvent OnDead;





    public void DealDamage(int damage)
    {
        if (!isDead)
        {
            vida -= damage;
            OnDamage?.Invoke();
            
            if(vida <= 0)
            {
                vida = 0;
                isDead = true;
                OnDead?.Invoke();
            }
        }

    }




}
