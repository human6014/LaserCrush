using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Energy : MonoBehaviour
{
    #region Property
    private static int m_Energy;
    private static Vector2 m_Postion;
    #endregion

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

    public static int  GetEnergy()
    {
        return m_Energy;
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
        if(energy <= m_Energy) 
        {
            m_Energy -= energy;
            return energy;
        }
        else
        {
            energy = m_Energy;
            m_Energy = 0;
            return energy; 
        }
    }
    
    public static bool CheckEnergy()
    {
        return m_Energy > 0;
    }


    public static bool IsAvailable()
    {
        return m_Energy > 0;
    }
}
