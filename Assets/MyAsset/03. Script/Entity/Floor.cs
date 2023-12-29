using Laser.Entity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floor : ICollisionable
{
    public void Awake()
    {
        m_Type = EntityType.Floor;
        Debug.Log("바닥 초기화");
    }

    public override void GetDamage(int damage)
    {
        return;
    }

    public override bool IsAttackable()
    {
        return false;
    }

    public override List<Vector2> Hitted(RaycastHit2D hit, Vector2 parentDirVector)
    {
        Debug.Log("바닥과 충돌 에너지 감소");
        Energy.UseEnergy(int.MaxValue);//에너지 모두 소진
        Debug.Log(Energy.GetEnergy());

        return new List<Vector2>();
    }
}
