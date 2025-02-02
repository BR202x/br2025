using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class HealthController : MonoBehaviour, Idamageable
{
    [SerializeField] public int vidaActual;

    [SerializeField] public int vida;
    [SerializeField] float cooldownDealDamage;
    bool canDealDamage = true;
    public bool isDead;
    public UnityEvent OnDamage;
    public UnityEvent OnDead;
    [SerializeField] bool hasBar;
    [SerializeField] SliderVida health;

    private void Start()
    {
        vidaActual = vida;
    }


    public void DealDamage(int damage)
    {
        if (!isDead)
        {
            if (canDealDamage)
            {
                canDealDamage = false;
                Invoke(nameof(DealDamageCooldown), cooldownDealDamage);
                vidaActual -= damage;

                if (vidaActual <= 0)
                {
                    vidaActual = 0;
                    isDead = true;
                    OnDead?.Invoke();
                }

                OnDamage?.Invoke();
                UpdateHealthBar();
            }
        }

    }

    public void DealDamageCooldown()
    {
        canDealDamage = true;
    }
    public void UpdateHealthBar()
    {
        if (hasBar)
        {
            float healthPercent = (float)vidaActual / (float)vida;

            health.UpdateHealthBar(healthPercent);
        }
    }




}
