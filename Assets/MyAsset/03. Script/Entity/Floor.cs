using System.Collections.Generic;
using UnityEngine;
using LaserCrush.Entity.Interface;

namespace LaserCrush.Entity
{
    public sealed class Floor : MonoBehaviour, ICollisionable
    {
        [SerializeField] private float weight = 1.5f;

        public List<LaserInfo> Hitted(RaycastHit2D hit, Vector2 parentDirVector, Laser laser)
        {
            //Energy.UseEnergy(int.MaxValue);
            Energy.CollideWithFloor();//여기서 레이저 매니저의 레이저 갯수랑 바닥 갯수를 알아야한다.
            laser.ChangeLaserState(ELaserStateType.Hitting);
            return new List<LaserInfo>();
        }

        //꾸준히 딜 박히게 하고 싶으면 true로 바꿔주면됨
        //weight는 가중치 -> 레이저 데미지보다 몇배를 줄지 결정
        public bool IsGetDamageable()
            => true;

        //딜 계속 박히게 하고 싶으면 주석 풀고 true반환
        public bool GetDamage(int damage)
        {
            Energy.UseEnergy((int)(damage * weight));
            return true;
        }

        public bool Waiting()
            => true;
        
        public EEntityType GetEEntityType()
            => EEntityType.Floor;
    }
}
