using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Condition : MonoBehaviour
{
    public float curValue;      // ���簪
    public float startValue;    // ���۰�
    public float maxValue;      // �ִ밪
    public float passiveValue;  // �ð��� ���� ����Ǵ� �� (���׹̳�, ���)
    public Image uiBar;

    
    void Start()
    {
        curValue = startValue;
    }

    
    void Update()
    {
        // ui������Ʈ
        uiBar.fillAmount = GetPercentage();
    }

    float GetPercentage()    // ���� �� ���� ���ϱ� (fill amount�� ������ �� ���ϱ�)
    {
        return curValue / maxValue;
    }

    public void Add(float value)    // ���� ������ �� �޼��� (ȸ��)
    {
        curValue = Mathf.Min(curValue + value, maxValue);
    }

    public void Subtract(float value)   // ���� ���ҵ� �� �޼��� (������)
    {
        curValue = Mathf.Max(curValue - value, 0f);
    }

}
