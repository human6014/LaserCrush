using LaserCrush.Entity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class Floor : MonoBehaviour, ICollisionable
{
    #region Variable
    private EEntityType m_EntityType;
    #endregion
    public void Awake()
    {
        m_EntityType = EEntityType.Floor;
        Debug.Log("바닥 초기화");
    }


    public List<Vector2> Hitted(RaycastHit2D hit, Vector2 parentDirVector)
    {
        Debug.Log("바닥과 충돌 에너지 감소");
        Energy.UseEnergy(int.MaxValue);//에너지 모두 소진
        return new List<Vector2>();
    }

    public bool IsGetDamageable()
    {
        return false;
    }

    public bool GetDamage(int  damage)
    {
        return false;
    }
}
