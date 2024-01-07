using LaserCrush.Entity;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public struct LaserInfo
{
    public Vector2 Position;
    public Vector2 Direction;

    public LaserInfo(Vector2 position, Vector2 direction)
    {
        Position = position;
        Direction = direction;
    }
}

public class InstalledItem : MonoBehaviour, ICollisionable
{
    #region Variable
    [SerializeField] private Transform[] m_EjectionPortsTransform;

    /// <summary>
    /// m_EjectionPorts : 각 사출구의 방향벡터
    /// </summary>
    protected List<LaserInfo> m_EjectionPorts = new List<LaserInfo>();
    private Laser m_HittingLaser;
    protected const int m_MaxUsingCount = 3;
    private const int m_ChargingTime = 120;
    
    protected int m_UsingCount = 0;
    private int m_ChargingWait;

    protected bool m_IsActivate = false;

    private bool m_IsFixedDirection;

    public int RowNumber { get; private set; }
    public int ColNumber { get; private set; }
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
    public void Init(int rowNumber, int colNumber)
    {
        RowNumber = rowNumber;
        ColNumber = colNumber;
        foreach (Transform tr in m_EjectionPortsTransform)
        {
            m_EjectionPorts.Add(new LaserInfo(position : tr.position, direction : tr.up));
        }

        m_UsingCount = m_MaxUsingCount;
        m_IsActivate = false;
    }

    public void FixDirection()
    {
        if (m_IsFixedDirection) return;
        m_IsFixedDirection = true;

        for(int i = 0; i < m_EjectionPorts.Count; i++)
        {
            m_EjectionPorts[i] = new LaserInfo(
                position: m_EjectionPortsTransform[i].position, 
                direction: m_EjectionPortsTransform[i].up
                );
        }
    }

    /// <summary>
    /// 해당 함수가 호출되면 프리즘이 활성화되며 사용 횟수가1회 차감
    /// 야매로 처리 충돌 튕기는거 잘 처리해야할듯
    /// </summary>
    /// <param name="hit"></param>
    /// <param name="parentDirVector"></param>
    /// <returns></returns>
    public List<LaserInfo> Hitted(RaycastHit2D hit, Vector2 parentDirVector, Laser laser)
    {
        if (m_IsActivate)
        {
            List<LaserInfo> answer = new List<LaserInfo>();
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
        if (m_IsFixedDirection) return;
        Vector2 direction = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(transform.forward, direction);
    }
}