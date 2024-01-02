using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LaserCrush.Entity
{
    public class DroppedItem : Item
    {
        #region Property
        private AcquiredItem m_AcquiredItem; // 이건 없어도 될듯 생성자로 바로 생성해서 넘기려고 함
        #endregion

        public AcquiredItem GetItem()
        {
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
            return m_AcquiredItem; // 여기서 바로 위에 생성한 인스턴스 넘기자
        }

        private void GetAnimationEnergy()
        {

        }

        private void GetAnimationPrism()
        {

        }

    }
}
