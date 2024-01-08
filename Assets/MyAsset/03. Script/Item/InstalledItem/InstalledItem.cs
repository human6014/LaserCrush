using LaserCrush.Entity;
using System.Collections.Generic;
using System;
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
    //시간 기준 아님 수정 필요함 - 승현이가 나중에 할 예정
    
    protected int m_UsingCount = 0;
    private int m_ChargingWait;

    protected bool m_IsActivate;
    private bool m_IsFixedDirection;
    #endregion

    private Action<bool> m_OnMouseItemAction;

    #region Property
    public int RowNumber { get; private set; }
    public int ColNumber { get; private set; }
    #endregion

    /// <summary>
    /// 해야할 역할
    /// 1. m_EjectionPorts 위치와 방향 초기화
    /// 2. 사용횟수 초기화
    /// </summary>
    public void Init(int rowNumber, int colNumber, Action<bool> onMouseItemAction)
    {
        RowNumber = rowNumber;
        ColNumber = colNumber;
        foreach (Transform tr in m_EjectionPortsTransform)
        {
            m_EjectionPorts.Add(new LaserInfo(position : tr.position, direction : tr.up));
        }

        m_UsingCount = m_MaxUsingCount;
        m_IsFixedDirection = false;
        m_IsActivate = false;
        m_OnMouseItemAction = onMouseItemAction;
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

        m_OnMouseItemAction?.Invoke(false);
    }

    public List<LaserInfo> Hitted(RaycastHit2D hit, Vector2 parentDirVector, Laser laser)
    {
        if (m_IsActivate)
        {
            return new List<LaserInfo>();
        }
        laser.ChangeLaserState(ELaserStateType.Wait);
        m_HittingLaser = laser;
        m_IsActivate = true;
        m_UsingCount--;
        return m_EjectionPorts;
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

    private void OnMouseDown()
    {
        if (m_IsFixedDirection) return;
        m_OnMouseItemAction?.Invoke(true);
    }

    private void OnMouseDrag()
    {
        if (m_IsFixedDirection) return;
        Vector2 direction = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(transform.forward, direction);
    }

    private void OnMouseUp()
    {
        if (m_IsFixedDirection) return;
        m_OnMouseItemAction?.Invoke(false);
    }

    private void OnDestroy()
    {
        m_OnMouseItemAction = null;
    }
}