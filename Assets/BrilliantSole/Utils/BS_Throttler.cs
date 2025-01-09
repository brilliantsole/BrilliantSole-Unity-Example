using System;
using UnityEngine;

public class BS_Throttler
{
    private float lastExecutionTime = 0f;
    private Action trailingAction = null;
    private float throttleDelay = 0f;

    public void Throttle(Action action, float delay)
    {
        float currentTime = Time.time;

        if (currentTime - lastExecutionTime >= delay)
        {
            action?.Invoke();
            lastExecutionTime = currentTime;
        }

        // Store the trailing action for later execution
        trailingAction = action;
        throttleDelay = delay;
    }

    public void Update()
    {
        if (trailingAction != null && Time.time - lastExecutionTime >= throttleDelay)
        {
            trailingAction?.Invoke();
            trailingAction = null;
        }
    }
}
