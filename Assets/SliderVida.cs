using UnityEngine;
using UnityEngine.UI;

public class SliderVida : MonoBehaviour
{
    public Image slider;

    
    public void UpdateHealthBar(float healthPercent)
    {
        slider.fillAmount = healthPercent;
        Debug.Log(healthPercent);
    }

}
