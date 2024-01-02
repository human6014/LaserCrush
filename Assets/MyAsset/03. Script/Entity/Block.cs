using LaserCrush.Entity;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace LaserCrush
{
    public class Block : MonoBehaviour, ICollisionable
    {
        #region Property
        [SerializeField] private TextMeshProUGUI m_Text;
        [SerializeField] private EEntityType m_Type;
        private EItemType m_Item;

        private EEntityType m_EntityType;
        private int m_HP = 1000;
        private bool m_IsDestroyed;
        #endregion

        private void Awake()
        {
            Init(1000, m_Type, EItemType.None);
        }

        /// <summary>
        /// 화면에 띄우기 전 반드시 초기화함수 호출할 것
        /// </summary>
        /// <param name="droppedItem"></param>
        /// 드랍 아이템이 없을 경우 널값을 대입
        /// <param name="entityType"></param>
        /// <param name="hp"></param>
        public void Init(int hp, EEntityType entityType, EItemType ItemType)
        {
            m_HP = hp;
            m_EntityType = entityType;
            m_Item = ItemType;
            m_Text.text = m_HP.ToString();
        }

        private void Destroy()
        {
            if (m_IsDestroyed) return;
            m_IsDestroyed = true;
            Destroy(gameObject);

            if (m_Item != EItemType.None)
            {
                /* TODO
                 * 가지고 있는 아이템으로 DroppedItem개체 인스턴시에이트 해야함
                 * AddDroppedItem -> 델리게이트 함수를 사용해서 배열에 추가해야함.
                 */
            }
        }

        public bool GetDamage(int damage)
        {
            //체력 구분으로 부서진 경우는 바로 return 하도록 변경
            if (m_HP <= damage) // 남은 피가 데미지보다 작을 경우
            {
                int getDamage = Energy.UseEnergy(m_HP); //사용 가능한 에너지를 반환받는다. -> 에너지 차감
                if (m_HP - getDamage == 0)
                {
                    Destroy();
                    return false;
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
            m_Text.text = m_HP.ToString();
            return true;
        }

        public bool IsGetDamageable()
        {
            return true;
        }

        public List<Vector2> Hitted(RaycastHit2D hit, Vector2 parentDirVector)
        {
            List<Vector2> answer = new List<Vector2>();
            if (m_EntityType == EEntityType.ReflectBlock)//반사 블럭일 경우만 자식 생성
            {
                Vector2 dir = (hit.normal + parentDirVector + hit.normal).normalized;
                return new List<Vector2>() { dir };
            }
            m_Text.text = m_HP.ToString();
            return answer;
        }

        public void MoveDown()
        {

        }
    }
}