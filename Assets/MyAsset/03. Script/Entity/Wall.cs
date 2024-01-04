using System.Collections.Generic;
using UnityEngine;

namespace LaserCrush.Entity
{

    public sealed class Wall : MonoBehaviour, ICollisionable
    {
        #region Variable
        private EEntityType m_EntityType;
        #endregion
        public void Awake()
        {
            m_EntityType = EEntityType.Wall;
            Debug.Log("벽 초기화");
        }

        public List<Vector2> Hitted(RaycastHit2D hit, Vector2 parentDirVector)
        {
            Debug.Log("벽과 충돌 후 자식생성");
            Debug.Log("벽과 충돌 후 에너지 사용");
            Energy.CollideWithWall();
            //Vector2 dir = (hit.normal + parentDirVector + hit.normal).normalized;
            Vector2 dir = Vector2.Reflect(parentDirVector, hit.normal);
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
    }
}