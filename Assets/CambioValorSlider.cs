using UnityEngine;
using UnityEngine.UI;

public class CambioValorSlider : MonoBehaviour
{
    [SerializeField] private SliderVida sliderVida;
    private float lastFillAmount;

    private void Start()
    {
        if (sliderVida != null && sliderVida.slider != null)
        {
            lastFillAmount = sliderVida.slider.fillAmount;
        }
    }

    private void Update()
    {
        if (sliderVida == null || sliderVida.slider == null) return;

        float currentFillAmount = sliderVida.slider.fillAmount;

        if (!Mathf.Approximately(lastFillAmount, currentFillAmount))
        {
            AudioImp.Instance.Reproducir("PlayerHurt");
            lastFillAmount = currentFillAmount;
        }
    }
}
