using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class Energy : MonoBehaviour
{
    #region Property
    [SerializeField] private TextMeshProUGUI m_Text;

    private static event UnityAction m_TextUpdate;
    
    private int m_Energy = 10000;
    private Vector2 m_Postion;
    #endregion

    private void Awake()
    {
        m_TextUpdate = null;
        m_TextUpdate += () => m_Text.text = m_Energy.ToString();
        m_TextUpdate?.Invoke();
    }

    /// <summary>
    /// 무조건 에너지는 1만 사용한다.
    /// 에너지를 소모할 수 있을 경우 참 반환
    /// </summary>
    /// <param name="energy"></param>
    /// <returns></returns>
    public bool UseEnergy()
    {
        if(m_Energy <= 0)
        {
            return false;
        }
        m_Energy -= 1;
        return true;
    }

    public int GetEnergy()
    {
        return m_Energy;
    }

    public Vector2 GetPosion()
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
    public int UseEnergy(int energy)
    {
        if(energy <= m_Energy) 
        {
            m_Energy -= energy;
        }
        else
        {
            energy = m_Energy;
            m_Energy = 0;
        }
        m_TextUpdate?.Invoke();
        return energy;
    }
    
    public bool CheckEnergy()
    {
        return m_Energy > 0;
    }

    public bool IsAvailable()
    {
        return m_Energy > 0;
    }

    /// <summary>
    /// 일단 부딪힐떄 마다 30퍼 삭제
    /// </summary>
    public void CollidWithWall()
    {
        m_Energy -= (m_Energy / 3);
    }
}
