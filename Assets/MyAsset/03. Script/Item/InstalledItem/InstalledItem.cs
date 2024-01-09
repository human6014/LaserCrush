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

public sealed class InstalledItem : MonoBehaviour, ICollisionable
{
    #region Variable
    [SerializeField] private Transform[] m_EjectionPortsTransform;

    private CircleCollider2D m_CircleCollider2D;

    private List<LaserInfo> m_EjectionPorts = new List<LaserInfo>();
    private Laser m_HittingLaser;

    private const int m_MaxUsingCount = 3;
    private const float m_ChargingTime = 0.5f;

    private int m_UsingCount = 0;
    private float m_ChargingWait;

    private Vector2 m_DirVector;

    private bool m_IsActivate;
    private bool m_IsFixedDirection;
    #endregion

    private Action<bool> m_OnMouseItemAction;

    #region Property
    public int RowNumber { get; private set; }
    public int ColNumber { get; private set; }
    #endregion

    public bool Waiting()
    {
        m_ChargingWait += Time.deltaTime;
        if (m_ChargingWait >= m_ChargingTime)
        {
            m_IsActivate = true;
            return true;
        }
        return false;
    }

    private void Awake()
    {
        m_CircleCollider2D = GetComponent<CircleCollider2D>();
        m_CircleCollider2D.enabled = false;
    }

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

        //todo
        //m_DirVector -> 방향벡터 초기화
        m_CircleCollider2D.enabled = true;
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

    /// <summary>
    /// 반시계 방향의 각도
    /// </summary>
    /// <param name="angle"></param>
    /// <returns></returns>
    private Vector2 Rotate(float angle)
    {
        m_DirVector.x = m_DirVector.x * Mathf.Cos(angle) - m_DirVector.y * Mathf.Sin(angle);
        m_DirVector.y = m_DirVector.x * Mathf.Sin(angle) + m_DirVector.y * Mathf.Cos(angle);
        m_DirVector.Normalize();
        //FixDirection();
        //TODO
        //위에 함수를 잘 수정해야 할 듯

        return m_DirVector;
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