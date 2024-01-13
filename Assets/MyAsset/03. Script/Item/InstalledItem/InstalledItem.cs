using LaserCrush.Entity;
using System.Collections.Generic;
using System;
using UnityEngine;
using LaserCrush.Extension;
using LaserCrush.Manager;

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
    [SerializeField] private LineRenderer[] m_LineRenderers;
    [SerializeField] private GameObject[] m_AdjustModeObjects;
    [SerializeField] private GameObject[] m_InitDeActiveObjects;

    private CircleCollider2D m_CircleCollider2D;

    private List<LaserInfo> m_EjectionPorts = new List<LaserInfo>();

    private const int m_MaxUsingCount = 3;
    private const float m_ChargingTime = 0.5f;

    private int m_UsingCount = 0;
    private float m_ChargingWait;

    private bool m_IsActivate;
    private bool m_IsAdjustMode;

    private Action<bool> m_OnMouseItemAction;
    #endregion

    #region Property
    public bool IsAdjustMode 
    { 
        get => m_IsAdjustMode;
        set
        {
            m_IsAdjustMode = value;
            foreach(GameObject go in m_AdjustModeObjects)
                go.SetActive(value);

            if(m_IsAdjustMode) PaintLineRenderer();
        } 
    }
    public int RowNumber { get; private set; }
    public int ColNumber { get; private set; }
    public bool IsFixedDirection { get; private set; }
    #endregion

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
            m_EjectionPorts.Add(new LaserInfo(position: tr.position, direction: tr.up));
        }

        foreach(GameObject go in m_InitDeActiveObjects)
            go.SetActive(false);

        m_CircleCollider2D.enabled = true;
        m_UsingCount = m_MaxUsingCount;
        IsFixedDirection = false;
        m_IsActivate = false;
        m_OnMouseItemAction = onMouseItemAction;
    }

    public void FixDirection()
    {
        if (IsFixedDirection) return;
        IsFixedDirection = true;

        for (int i = 0; i < m_EjectionPorts.Count; i++)
        {
            m_EjectionPorts[i] = new LaserInfo(
                position: m_EjectionPortsTransform[i].position,
                direction: m_EjectionPortsTransform[i].up
                );
        }
        m_OnMouseItemAction?.Invoke(false);
    }

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

    public List<LaserInfo> Hitted(RaycastHit2D hit, Vector2 parentDirVector, Laser laser)
    {
        if (m_IsActivate)
        {
            return new List<LaserInfo>();
        }
        laser.ChangeLaserState(ELaserStateType.Wait);
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

    private void PaintLineRenderer()
    {
        for (int i = 0; i < m_LineRenderers.Length; i++)
        {
            Vector2 linePos = m_LineRenderers[i].transform.position;
            Vector2 lineDir = m_LineRenderers[i].transform.up;

            RaycastHit2D hit = Physics2D.Raycast(linePos, lineDir, Mathf.Infinity, RayManager.s_LaserHitableLayer);

            m_LineRenderers[i].positionCount = 2;
            m_LineRenderers[i].SetPosition(0, linePos);
            m_LineRenderers[i].SetPosition(1, hit.point);
        }
    }

    public void SetDirection(Vector2 pos)
    {
        Vector2 direction = (pos - (Vector2)transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(transform.forward, direction.DiscreteDirection(5));
        PaintLineRenderer();
    }

    public EEntityType GetEEntityType()
    {
        return EEntityType.Prisim;
    }

    private void OnDestroy()
    {
        m_OnMouseItemAction = null;
    }
}