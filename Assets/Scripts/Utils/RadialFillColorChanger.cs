using UnityEngine;
using UnityEngine.UI;

public class RadialFillColorChanger : MonoBehaviour
{
    public Image radialImage; 
    public Gradient fillGradient; 
    public Timer timer;
    public bool AutomaticFill = false;

    void Start()
    {
        if (timer == null)
        {
            return;
        }
        
        timer.OnTimeChanged += UpdateFillAmountAndColor;
        timer.OnTimerCompleted += OnTimerCompleted;
    }
    
    void Update()
    {
        if (AutomaticFill)
        {
            float fillAmount = radialImage.fillAmount;
            
            radialImage.color = fillGradient.Evaluate(fillAmount);
        }
    }

    void OnDestroy()
    {
        if (timer == null)
        {
            return;
        }
        
        timer.OnTimeChanged -= UpdateFillAmountAndColor;
        timer.OnTimerCompleted -= OnTimerCompleted;
    }

    private void UpdateFillAmountAndColor(float fillAmount)
    {
        radialImage.fillAmount = fillAmount;
        radialImage.color = fillGradient.Evaluate(fillAmount);
    }

    private void OnTimerCompleted()
    {
        Debug.Log("Timer completed!");
    }
}