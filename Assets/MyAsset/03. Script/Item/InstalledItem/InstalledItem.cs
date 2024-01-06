using LaserCrush.Entity;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

/// <summary>
/// 레이저가 프리즘과 충돌 시 활성화 되어 있으면 port의 vector를 받아
/// 해당 방향으로 레이저를 추가 생산
/// </summary>

public struct LaserInfo
{
    public Vector2 Posion;
    public Vector2 Direction;
}


public class InstalledItem : MonoBehaviour, ICollisionable
{
    #region Variable
    [SerializeField] private Transform[] m_EjectionPortsTransform;

    /// <summary>
    /// m_EjectionPorts : 각 사출구의 방향벡터
    /// </summary>
    protected List<Vector2> m_EjectionPorts = new List<Vector2>();

    private const int m_MaxUsingCount = 3;
    private const int m_ChargingTime = 10;
    
    private int m_UsingCount = 0;
    private int m_ChargingWait;

    protected bool m_IsActivate = false;
    #endregion

    /// <summary>
    /// 화면에서 유저 입력을 받아 설치할때 호출할 함수
    /// 아이템에서 보조선이 나와 각도를 시각화 해준다
    /// 수정 필요
    /// </summary>
    void RotateEjectionPorts()
    {
    }

    /// <summary>
    /// 이거 가상함수로 자식 클래스에서 함수가 호출이 안됨 원인 알면 알려줘
    /// </summary>
    public virtual void Init()     {    }

    /// <summary>
    /// 활성화 되어있지 않으면 Charging후 레이저에서 새로운 레이저 생성
    /// 활성화 된 상태면 레이저 생성 x 이미 레이저 존재
    /// </summary>
    /// <returns></returns>
    public bool IsActivate()
    {
        return m_IsActivate;
    }

    public void Charging()
    {
        //시간경과
        m_ChargingWait++;
        if(m_ChargingWait >= m_ChargingTime)
        {
            m_IsActivate = true;
        }
    }

    /// <summary>
    /// 해당 함수가 호출되면 프리즘이 활성화되며 사용 횟수가1회 차감
    /// 야매로 처리 충돌 튕기는거 잘 처리해야할듯
    /// </summary>
    /// <param name="hit"></param>
    /// <param name="parentDirVector"></param>
    /// <returns></returns>
    public List<Vector2> Hitted(RaycastHit2D hit, Vector2 parentDirVector)
    {
        if (m_IsActivate)
        {
            List<Vector2> answer = new List<Vector2>();
            return answer;
        }
        m_IsActivate = true;
        m_UsingCount--;
        return m_EjectionPorts;
    }

    public bool IsOverloaded()
    {
        m_IsActivate = false;
        if (m_UsingCount == 0)
        {
            return true;
        }
        return false; 
    }

    public bool IsGetDamageable()
    {
        return false;
    }

    public bool GetDamage(int damage)
    {
        return false;
    }

    private void OnMouseDrag()
    {
        Vector2 direction = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(transform.forward, direction);
    }
}