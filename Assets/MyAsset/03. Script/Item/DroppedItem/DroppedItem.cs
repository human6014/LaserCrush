using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LaserCrush.Entity
{
    public class DroppedItem : MonoBehaviour
    {
        #region Variable
        [SerializeField] private AcquiredItem m_AcquiredItem;
        
        private EItemState m_State;
        private EItemType m_Type;
        #endregion

        /// <summary>
        /// AcquiredItem 오브젝트를 생성 후 반환
        /// </summary>
        /// <returns></returns>
        public AcquiredItem GetItem()
        {
            AcquiredItem acquiredItem = Instantiate(m_AcquiredItem);
            /*TODO
             *해당 인스턴스를 파괴하고 AcquiredItem 개체를 생성한다
             *위 과정에서 애니메이션 효과 사용하면 된다.
             */
            switch (m_Type) //애니메이션용 분기
            {
                case EItemType.Energy:
                    GetAnimationEnergy();
                    break;
                default:
                    GetAnimationPrism();
                    break;
            }
            return acquiredItem; // 여기서 바로 위에 생성한 인스턴스 넘기자
        }

        private void GetAnimationEnergy()
        {

        }

        private void GetAnimationPrism()
        {

        }
    }
}
