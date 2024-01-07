using LaserCrush.Entity;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

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
    private Laser m_HittingLaser;
    protected const int m_MaxUsingCount = 3;
    private const int m_ChargingTime = 120;
    
    protected int m_UsingCount = 0;
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

    public bool Waiting()
    {
        m_ChargingWait++;
        if (m_ChargingWait >= m_ChargingTime)
        {
            m_IsActivate = true;
            return true;
        }
        return false;
    }

    /// <summary>
    /// 해야할 역할
    /// 1. m_EjectionPorts 위치와 방향 초기화
    /// 2. 사용횟수 초기화
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

    /// <summary>
    /// 해당 함수가 호출되면 프리즘이 활성화되며 사용 횟수가1회 차감
    /// 야매로 처리 충돌 튕기는거 잘 처리해야할듯
    /// </summary>
    /// <param name="hit"></param>
    /// <param name="parentDirVector"></param>
    /// <returns></returns>
    public List<Vector2> Hitted(RaycastHit2D hit, Vector2 parentDirVector, Laser laser)
    {
        if (m_IsActivate)
        {
            List<Vector2> answer = new List<Vector2>();
            return answer;
        }
        laser.ChangeLaserState(ELaserStateType.Wait);
        m_HittingLaser = laser;
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