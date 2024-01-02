using System;
using System.Collections.Generic;
using UnityEngine;

namespace LaserCrush.Entity
{
    interface ICollisionable
    {
        public List<Vector2> Hitted(RaycastHit2D hit, Vector2 parentDirVector);

        public bool IsGetDamageable();

        public bool GetDamage(int damage); 
    }
}