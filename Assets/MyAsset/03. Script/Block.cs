using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BlockType
{
    Normal,
    Reflect
}

public class Block : MonoBehaviour
{
    #region Property
    private int m_HP;
    private Vector2 m_Position;
    private DroppedItem m_Item = null;
    #endregion

    void getDamage(int damage)
    {
        if(m_HP - damage <= 0)
        {
            Destroy();
        }
        else
        {
            m_HP -= damage;
        }
    }

    private void Destroy()
    {
        //가지고 있는 아이템을 필드에 생성 -> 턴 종료 후 획등 방식
    }
}