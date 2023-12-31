using System;
using System.Collections.Generic;
using UnityEngine;

namespace Laser.Entity
{
    //enum 필요없음 이거
    public enum EntityType
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
    public abstract class ICollisionable : MonoBehaviour
    {
        protected EntityType m_Type;
        public abstract void GetDamage(int damage);

        public abstract bool IsAttackable();

        public abstract List<Vector2> Hitted(RaycastHit2D hit, Vector2 parentDirVector);

    }
}