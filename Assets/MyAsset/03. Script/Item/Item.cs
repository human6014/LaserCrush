using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LaserCrush.Entity
{
    public enum ItemType
    {
        Energy,
        Prism_1,
        Prism_2,
        Prism_3,
        Prism_4,
        Prism_5,
    }

    public enum ItemState
    {
        Dropped,
        Installing,
        InMyBag
    }
    /// <summary>
    /// 블럭을 부숴 획득한 경우, 필드에 떨어진 상태와 가방에 있는 상태
    /// 설치 중인 상태 등 설치되기 전의 아이템 설치되면 해당 프리즘을 호출해서 인스턴시에이트 해준다.
    /// Item != Prism
    /// </summary>
    public class Item : MonoBehaviour
    {
        #region Property
        private Vector2 m_Posion { get; }
        private ItemType m_Type;
        private ItemState m_State;
        #endregion


        public void GetItem()
        {
            if(m_State == ItemState.Dropped)
            {
                m_State = ItemState.InMyBag;
            }   
        }

        /// <summary>
        /// 해당 위치에 프리즘개체 인스턴시에이트 및 초기화 해주기
        /// </summary>
        public void InstallPrism()
        {

        }
    }

}