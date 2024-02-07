using System.Collections.Generic;
using UnityEngine;
using LaserCrush.Entity.Interface;

namespace LaserCrush.Entity
{
    public sealed class Wall : MonoBehaviour, ICollisionable
    {
        public List<LaserInfo> Hitted(RaycastHit2D hit, Vector2 parentDirVector, Laser laser)
        {
            Energy.CollideWithWall();
            laser.ChangeLaserState(ELaserStateType.Hitting);
            //Vector2 dir = (hit.normal + parentDirVector + hit.normal).normalized;
            LaserInfo info = new LaserInfo();
            info.Direction = Vector2.Reflect(parentDirVector, hit.normal);
            info.Position = hit.point;

            return new List<LaserInfo>() { info };
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

        public EEntityType GetEEntityType()
        {
            return EEntityType.Wall;
        }
    }
}