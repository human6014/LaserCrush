using LaserCrush.Entity;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Rendering;
using TMPro.EditorUtilities;

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
    private Vector2 m_DirVector;

    private const int m_MaxUsingCount = 3;
    private const float m_ChargingTime = 0.5f;

    private int m_UsingCount = 0;
    private float m_ChargingWait;

    private Vector2 m_Posion;

    private static List<Vector2> UnitCircle = new List<Vector2>();

    private bool m_IsActivate;
    
    private Action<bool> m_OnMouseItemAction;
    #endregion



    #region Property
    public int RowNumber { get; private set; }
    public int ColNumber { get; private set; }
    public bool IsFixedDirection { get; private set; }
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

    //아래 나눌 숫자를 바꾸고 싶으면 180적혀있는 칸에 갯수를 적으면 됨
    private void Awake()
    {
        m_CircleCollider2D = GetComponent<CircleCollider2D>();
        m_CircleCollider2D.enabled = false;
        //단위원에서 각도별 좌표 생성
        if(UnitCircle.Count == 0)
        {
            float dif = 360 / 180;
            for(int i = 1; i < 181; i++) 
            {
                UnitCircle.Add(new Vector2(
                    Mathf.Cos(dif * i * Mathf.Deg2Rad),
                    Mathf.Sin(dif * i * Mathf.Deg2Rad)).normalized);
            }
        } 
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
        IsFixedDirection = false;
        m_IsActivate = false;
        m_OnMouseItemAction = onMouseItemAction;

    }

    public void FixDirection()
    {
        if (IsFixedDirection) return;
        IsFixedDirection = true;

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
    /// 
    /// </summary>
    /// <param name="vec">마우스로 가리킬 프리즘의 방향</param>
    /// <param name="div">360도를 몇개로 나눌지</param>
    /// <returns>프리즘의 방향백터</returns>
    private Vector2 Rotate(Vector2 vec)
    {
        Vector2 answer = Vector2.one;

        //프리즘을 기준으로 마우스가 가르키는 방향의 단위 백터
        Vector2 dir = (vec - m_Posion).normalized;

        float dif = float.MaxValue;
        foreach(Vector2 unit in UnitCircle)
        {
            if(Vector2.Distance(dir, unit) < dif)
            {
                answer = unit;
                dif = Vector2.Distance(dir, unit);
            }
        }
        //기존 방향벡터와 새로 선택된 방향백터의 각도 차이
        GetAngle(dir, m_DirVector);

        m_DirVector = answer;
        return answer;
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

    public void SetDirection(Vector2 pos)
    {
        Vector2 direction = (pos - (Vector2)transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(transform.forward, direction);
    }

    //private void OnMouseDown()
    //{
    //    if (IsFixedDirection) return;
    //    m_OnMouseItemAction?.Invoke(true);
    //}

    //private void OnMouseDrag()
    //{
    //    if (IsFixedDirection) return;
    //    Vector2 direction = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position).normalized;
    //    transform.rotation = Quaternion.LookRotation(transform.forward, direction);
    //}

    //private void OnMouseUp()
    //{
    //    if (IsFixedDirection) return;
    //    m_OnMouseItemAction?.Invoke(false);
    //}

    private void OnDestroy()
    {
        m_OnMouseItemAction = null;
    }

    private float GetAngle(Vector3 from, Vector3 to)
    {
        Vector3 v = to - from;
        return Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg;
    }

    public EEntityType GetEEntityType()
    {
        return EEntityType.Prisim;
    }
}