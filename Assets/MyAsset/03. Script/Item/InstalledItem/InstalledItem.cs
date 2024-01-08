using LaserCrush.Entity;
using System.Collections.Generic;
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
    protected List<LaserInfo> m_EjectionPorts = new List<LaserInfo>();
    private Laser m_HittingLaser;
    protected const int m_MaxUsingCount = 3;
    private const int m_ChargingTime = 100;
    
    protected int m_UsingCount = 0;
    private int m_ChargingWait;
    private Vector2 m_DirVector;
    protected bool m_IsActivate = false;

    private bool m_IsFixedDirection;

    public int RowNumber { get; private set; }
    public int ColNumber { get; private set; }
    #endregion

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

        //todo
        //m_DirVector -> 방향벡터 초기화
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
        m_ChargingWait = 0;

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

    /// <summary>
    /// 반시계 방향의 각도
    /// </summary>
    /// <param name="angle"></param>
    /// <returns></returns>
    private Vector2 Rotate(float angle)
    {
        m_DirVector.x = m_DirVector.x * Mathf.Cos(angle) - m_DirVector.y*Mathf.Sin(angle);
        m_DirVector.y = m_DirVector.x*Mathf.Sin(angle) + m_DirVector.y * Mathf.Cos(angle);
        //FixDirection();
        //TODO
        //위에 함수를 잘 수정해야 할 듯

        return m_DirVector;
    }
}