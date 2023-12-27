using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ICollisionable;

public class Wall : MonoBehaviour, ICollisionable
{
    private EntityType m_Type = EntityType.Wall;

    public void GetDamage(int damage)
    {
        return;
    }

    public ICollisionable.EntityType GetEntityType()
    {
        return m_Type;
    }

    public bool IsAttackable()
    {
        return false;
    }
}
