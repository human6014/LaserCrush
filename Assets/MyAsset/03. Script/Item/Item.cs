using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LaserCrush.Entity
{
    
    
    /// <summary>
    /// 블럭을 부숴 획득한 경우, 필드에 떨어진 상태와 가방에 있는 상태
    /// 설치 중인 상태 등 설치되기 전의 아이템 설치되면 해당 프리즘을 호출해서 인스턴시에이트 해준다.
    /// Item != Prism
    /// </summary>
    public class Item : MonoBehaviour
    {
        #region Property
        protected EItemState m_State;
        protected EItemType m_Type;
        protected Vector2 m_Posion { get; }
        #endregion

    }

}