using Laser.Entity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 레이저가 프리즘과 충돌 시 활성화 되어 있으면 port의 vector를 받아
/// 해당 방향으로 레이저를 추가 생산
/// </summary>

public class Prism : ICollisionable
{
    /// <summary>
    /// m_EjectionPorts : 각 사출구의 방향벡터
    /// </summary>
    #region Property
    private const int m_MaxUsingCount = 3;
    private const int m_CHARGINTIME = 10;
    private List<Vector2> m_EjectionPorts = new List<Vector2>();
    private int m_UsingCount = 0;
    private bool m_IsActivate = false;
    private int m_ChargingWait;
    #endregion

    /// <summary>
    /// 화면에서 유저 입력을 받아 설치할때 호출할 함수
    /// 아이템에서 보조선이 나와 각도를 시각화 해준다
    /// 수정 필요
    /// </summary>
    void SetEjectionPorts()
    {
        //입력을 받아 rot를 만든다.
        for(int i = 0; i < m_EjectionPorts.Count; i++) 
        {
            m_EjectionPorts[i] = Quaternion.Euler(0f, 0f, 0f) * m_EjectionPorts[i];
        }
    }

    /// <summary>
    /// 활성화 되어있지 않으면 Charging후 레이저에서 새로운 레이저 생성
    /// 활성화 된 상태면 레이저 생성 x 이미 레이저 존재
    /// </summary>
    /// <returns></returns>
    public bool IsActivate()
    {
        return m_IsActivate;
    }

    public List<Vector2> GetEjectionPorts()
    {
        return m_EjectionPorts;
    }

    public void Charging()
    {
        //시간경과
        m_ChargingWait++;
        if(m_ChargingWait >= m_CHARGINTIME)
        {
            m_IsActivate=true;
        }
    }

    public override void GetDamage(int damage)
    {
        return;
    }

    public override bool IsAttackable()
    {
        return false;
    }

    public override List<Vector2> Hitted(RaycastHit2D hit, Vector2 parentDirVector)
    {
        List<Vector2> answer = new List<Vector2>();
        return answer;
    }
}