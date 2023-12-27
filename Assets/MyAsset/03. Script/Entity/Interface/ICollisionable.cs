using System;
using UnityEngine;

namespace Laser.Entity
{
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
    abstract public class ICollisionable : MonoBehaviour
    {
        protected EntityType m_Type;
        public abstract EntityType GetEntityType();

        public abstract void GetDamage(int damage);

        public abstract bool IsAttackable();
    }
}