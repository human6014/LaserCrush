using System;
using System.Collections.Generic;
using UnityEngine;

namespace LaserCrush.Entity
{
    interface ICollisionable
    {
        public List<LaserInfo> Hitted(RaycastHit2D hit, Vector2 parentDirVector, Laser laser);

        public bool IsGetDamageable();

        public bool GetDamage(int damage);

        public bool Waiting();
    }
}