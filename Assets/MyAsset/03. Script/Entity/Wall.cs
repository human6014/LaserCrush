using System.Collections.Generic;
using UnityEngine;
using LaserCrush.Entity.Interface;

namespace LaserCrush.Entity
{
    public sealed class Wall : MonoBehaviour, ICollisionable
    {
        public List<LaserInfo> Hitted(RaycastHit2D hit, Vector2 parentDirVector, Laser laser)
        {
            laser.ChangeLaserState(ELaserStateType.Hitting);
            LaserInfo info = new LaserInfo();
            info.Direction = Vector2.Reflect(parentDirVector, hit.normal);
            info.Position = hit.point;

            return new List<LaserInfo>() { info };
        }

        public bool IsGetDamageable()
        {
            return false;
        }

        public bool GetDamage()
        {
            return false;
        }

        public EEntityType GetEEntityType()
        {
            return EEntityType.Wall;
        }
    }
}