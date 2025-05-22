using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Condition : MonoBehaviour
{
    public float curValue;      // 현재값
    public float startValue;    // 시작값
    public float maxValue;      // 최대값
    public float passiveValue;  // 시간에 따라 변경되는 값 (스테미나, 헝거)
    public Image uiBar;

    
    void Start()
    {
        curValue = startValue;
    }

    
    void Update()
    {
        // ui업데이트
        uiBar.fillAmount = GetPercentage();
    }

    float GetPercentage()    // 현재 값 비율 구하기 (fill amount를 설정할 값 구하기)
    {
        return curValue / maxValue;
    }

    public void Add(float value)    // 값이 더해질 때 메서드 (회복)
    {
        curValue = Mathf.Min(curValue + value, maxValue);
    }

    public void Subtract(float value)   // 값이 감소될 때 메서드 (데미지)
    {
        curValue = Mathf.Max(curValue - value, 0f);
    }

}
