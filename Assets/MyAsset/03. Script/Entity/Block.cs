using Laser.Entity;
using System.Collections.Generic;
using UnityEngine;

namespace Laser.Object
{
    public class Block : ICollisionable
    {
        #region Property
        private int m_HP;
        private Vector2 m_Position;
        private Item m_Item = null;
        private List<Vector2> m_Direction = new List<Vector2>();
        #endregion

        /// <summary>
        /// 화면에 띄우기 전 반드시 초기화함수 호출할 것
        /// </summary>
        /// <param name="droppedItem"></param>
        /// 드랍 아이템이 없을 경우 널값을 대입
        /// <param name="entityType"></param>
        /// <param name="hp"></param>
        public void Init(Item droppedItem, EntityType entityType, int hp)
        {
            m_HP = hp;
            m_Type = entityType;
            m_Item = droppedItem;
        }
        private void Destroy()
        {
            //가지고 있는 아이템을 필드에 생성 -> 턴 종료 후 획등 방식
        }

        public override void GetDamage(int damage)
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

        public override bool IsAttackable()
        {
            return true;
        }
        public override List<Vector2> Hitted(RaycastHit2D hit, Vector2 parentDirVector)
        {
            List<Vector2> answer = new List<Vector2>();
            if (m_Type == EntityType.ReflectBlock)//반사 블럭일 경우만 자식 생성
            {
                Vector2 dir = (hit.normal + parentDirVector + hit.normal).normalized;
                return new List<Vector2>() { dir };
            }
            return answer;
        }
    }
}