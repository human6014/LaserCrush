using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class Energy : MonoBehaviour
{
    #region Variable
    [SerializeField] private TextMeshProUGUI m_Text;

    private static event UnityAction m_TextUpdate;

    private static int m_MaxEnergy = 10000;
    private static int m_CurrentEnergy = 10000;
    private static Vector2 m_Postion;
    #endregion

    private static int CurrentEnergy 
    { 
        get => m_CurrentEnergy;
        set
        {
            m_CurrentEnergy = value;
            m_TextUpdate?.Invoke();
        }
    }

    private void Awake()
    {
        m_TextUpdate = null;
        m_TextUpdate += () => m_Text.text = m_CurrentEnergy.ToString();

        m_MaxEnergy = 10000;
        CurrentEnergy = 10000;
        m_Postion = Vector2.zero;
    }

    /// <summary>
    /// 무조건 에너지는 1만 사용한다.
    /// 에너지를 소모할 수 있을 경우 참 반환
    /// </summary>
    /// <param name="energy"></param>
    /// <returns></returns>

    public static int GetEnergy()
    {
        return CurrentEnergy;
    }

    public static Vector2 GetPosion()
    {
        return m_Postion;
    }

    /// <summary>
    /// 반환형은 총 사용한 에너지의 양이다.
    /// 적게남
    /// </summary>
    /// <param name="energy">
    /// 사용할 에너지
    /// </param>
    /// <returns></returns>
    public static int UseEnergy(int energy)
    {
        if (energy <= CurrentEnergy) 
        {
            CurrentEnergy -= energy;
        }
        else
        {
            energy = CurrentEnergy;
            CurrentEnergy = 0;
        }
        return energy;
    }
    
    public static bool CheckEnergy()
    {
        return CurrentEnergy > 0;
    }

    public static bool IsAvailable()
    {
        return CurrentEnergy > 0;
    }

    /// <summary>
    /// 일단 부딪힐 마다 10퍼 삭제
    /// </summary>
    public static void CollideWithWall()
    {
        CurrentEnergy -= (CurrentEnergy / 10);
    }

    public static void ChargeEnergy()
    {
        CurrentEnergy = m_MaxEnergy;
    }

    public static void EnergyUpgrade(int additionalEnergy)
    {
        m_MaxEnergy += additionalEnergy;
    }
}
