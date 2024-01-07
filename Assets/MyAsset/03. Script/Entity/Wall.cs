using System.Collections.Generic;
using UnityEngine;

namespace LaserCrush.Entity
{

    public sealed class Wall : MonoBehaviour, ICollisionable
    {
        public List<Vector2> Hitted(RaycastHit2D hit, Vector2 parentDirVector, Laser laser)
        {
            Energy.CollideWithWall();
            //Vector2 dir = (hit.normal + parentDirVector + hit.normal).normalized;
            Vector2 dir = Vector2.Reflect(parentDirVector, hit.normal);
            laser.ChangeLaserState(ELaserStateType.Hitting);
            return new List<Vector2>() { dir };
        }

        public bool IsGetDamageable()
        {
            return false;
        }

        public bool GetDamage(int damage)
        {
            return false;
        }

        public bool Waiting()
        {
            return true;
        }
    }
}