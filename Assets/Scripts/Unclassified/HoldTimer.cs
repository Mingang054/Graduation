using UnityEngine;
using System;
using UnityEngine.UI;

public class HoldTimer : MonoBehaviour
{
    private float holdThreshold = 1f;
    private float holdTime = 0f;
    private bool isActive = false;

    private VestPlacable currentPlacable; // 🔥 현재 눌려진 애를 기억

    public Slider slider;

    private void Awake()
    {
        enabled = false; // 기본 꺼두기
    }

    public void StartHold(VestPlacable placable)
    {
        slider.value = 0;
        currentPlacable = placable;
        holdThreshold = 1f;
        holdTime = 0f;
        isActive = true;
        enabled = true;
    }

    public void StopHold()
    {
        slider.value = 0;
        currentPlacable = null;
        holdTime = 0f;
        isActive = false;
        enabled = false;
    }

    private void Update()
    {
        if (!isActive || currentPlacable == null)
            return;

        holdTime += Time.deltaTime;
        slider.value = holdTime;    
        if (holdTime >= holdThreshold)
        {
            bool shouldStop = currentPlacable.OnHoldTick();
            holdTime = 0f;

            if (shouldStop)
            {
                StopHold(); // 🔥 바로 멈춰버리기
            }
        }
    }

}