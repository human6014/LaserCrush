using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface ICollisionable
{
    [Flags]
    enum EntityType
    {
        //Attable//
        NormalBlock,
        ReflectBlock,

        ///////
        Prisim,
        Floor,
        Wall,
        Launcher
    }

    public EntityType GetEntityType();

    public void GetDamage(int damage);

    public bool IsAttackable();
}
