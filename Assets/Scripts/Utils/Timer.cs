using System;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public float duration = 10f; 
    private float elapsedTime = 0f; 
    private bool isRunning = false;

    public event Action<float> OnTimeChanged; 
    public event Action OnTimerCompleted;

    public bool IsActive = false;

    private void Start()
    {
        gameObject.SetActive(IsActive);
    }

    public void StartTimer()
    {
        gameObject.SetActive(true);
        elapsedTime = 0f;
        isRunning = true;
    }

    void Update()
    {
        if (isRunning)
        {
            elapsedTime += Time.deltaTime;
            
            OnTimeChanged?.Invoke(Mathf.Clamp01(1f - elapsedTime / duration));

            if (elapsedTime >= duration)
            {
                CompleteTimer();
            }
        }
    }

    public void ForceCompleteTimer()
    {
        if (isRunning)
        {
            elapsedTime = duration;
            CompleteTimer();
        }
    }

    private void CompleteTimer()
    {
        isRunning = false;
        OnTimeChanged?.Invoke(0f);
        OnTimerCompleted?.Invoke();
        gameObject.SetActive(false);
    }
    
    public void ResetTimer()
    {
        elapsedTime = 0f;
        isRunning = false;
        gameObject.SetActive(false);
    }
}