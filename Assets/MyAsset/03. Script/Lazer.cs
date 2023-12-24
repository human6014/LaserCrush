using System.Collections;
using System.Collections.Generic;
using UnityEngine;
enum CollisionType
{
    Wall,
    NormalBlock,
    ReflectBlock,
    Floor,
    Item,
    Launcher
}

public class Lazer : MonoBehaviour
{
    /// <summary>
    /// m_StartPoint : 레이저 시작점 -> 소멸시 시작점이 이동함
    /// m_EndPoint : 레이저 끝점 -> 발사 시 끝점이 이동함
    /// </summary>
    #region Property
    private Vector2 m_StartPoint;
    private Vector2 m_EndPoint;
    private Vector2 m_DirectionVector;
    private float m_EraseVelocity;
    private float m_ShootingVelocity;
    private List<Lazer> m_ChildLazers = new List<Lazer>();
    private bool m_IsComplite;
    #endregion

    public void GenerateLazer(Vector2 direction)
    {
        //레이저 생성
    }

    public bool HasChild()
    {
        if(m_ChildLazers.Count == 0)
        {
            return false;
        }
        return true;
    }

    public List<Lazer> GetChildLazer()
    {
        return m_ChildLazers;
    }

    /// <summary>
    /// 일정 속도만큼 startPoint를 endPoint방향으로 이동
    /// </summary>
    public void Erase()
    {
        if(Vector2.Distance(m_StartPoint, m_EndPoint) <= m_EraseVelocity)
        {
            //삭제
            m_StartPoint = m_EndPoint;
            return;
        }
        m_StartPoint += (m_DirectionVector * m_EraseVelocity);
    }
     
    public void Shoot()
    {
        List<ICollisionable> obj;
        obj = new List<ICollisionable>();
        for (int i = 0; i < obj.Count; i++) 
        {
            //이동거리안에 있을때 
            if (Vector2.Distance(obj[i].GetPosition(), m_EndPoint) <= m_ShootingVelocity)
            {

            }
        }
        m_EndPoint += (m_DirectionVector * m_ShootingVelocity);
    }

    //이동거리안에 물체가 있을 경우
    public bool Collision(Vector2 obj)
    {
        if (Vector2.Distance(obj, m_EndPoint) <= float.Epsilon)
        {
            return true;
        }
        return false;
    }

    public bool IsComplite()
    {
        return m_IsComplite;
    }

    public void Init(Vector2 start, Vector2 end, Vector2 dir)
    {
        m_StartPoint = start;
        m_EndPoint = end;
        m_DirectionVector = dir;
    }
}

