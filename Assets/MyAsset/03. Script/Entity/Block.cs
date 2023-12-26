using System.Collections;
using System.Collections.Generic;
using Unity.Android.Types;
using UnityEngine;
using static ICollisionable;

namespace Laser.Object
{
    public class Block : MonoBehaviour, ICollisionable
    {
        #region Property
        private int m_HP;
        private Vector2 m_Position;
        private DroppedItem m_Item = null;
        private EntityType m_Type = EntityType.NormalBlock;
        private List<Vector2> m_Direction = new List<Vector2>();
        #endregion

        /// <summary>
        /// 화면에 띄우기 전 반드시 초기화함수 호출할 것
        /// </summary>
        /// <param name="droppedItem"></param>
        /// 드랍 아이템이 없을 경우 널값을 대입
        /// <param name="entityType"></param>
        /// <param name="hp"></param>
        public void Init(DroppedItem droppedItem, EntityType entityType, int hp)
        {
            m_HP = hp;
            m_Type = entityType;
            m_Item = droppedItem;
        }
        private void Destroy()
        {
            //가지고 있는 아이템을 필드에 생성 -> 턴 종료 후 획등 방식
        }
        public EntityType GetEntityType()
        {
            return m_Type;
        }

        public void GetDamage(int damage)
        {
            if (m_HP < damage) // 남은 피가 데미지보다 작을 경우
            {
                int getDamage = Energy.UseEnergy(m_HP); //사용 가능한 에너지를 반환받는다. -> 에너지 차감
                if (m_HP - getDamage == 0)
                {
                    Destroy();
                }
                else
                {
                    m_HP -= getDamage;
                }
            }
            else
            {
                int getDamage = Energy.UseEnergy(damage);  //사용 가능한 에너지를 반환받는다.
                m_HP -= getDamage;
            }
        }

        public bool IsAttackable()
        {
            if(m_Type == ICollisionable.EntityType.NormalBlock || m_Type == ICollisionable.EntityType.ReflectBlock)
            {
                return true;
            }
            return false;
        }


    }
}