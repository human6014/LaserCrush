using System.Collections.Generic;
using UnityEngine;
using System;
using LaserCrush.Entity;

namespace LaserCrush.Manager
{
    [Serializable]
    public class BlockManager
    {
        [SerializeField] private Transform m_BlockTransform;
        [SerializeField] private GameObject m_BlockObject;
        [SerializeField] private Vector2 m_InitPos;
        [SerializeField] private Vector2 m_Offset;

        private List<Block> m_Blocks;
        public event Func<GameObject, GameObject> m_InstantiateFunc;

        public void Init(Func<GameObject, GameObject> instantiateFunc)
        {
            m_Blocks = new List<Block>();
            m_InstantiateFunc = instantiateFunc;
        }

        public void GenerateBlock(int num)
        {
            GameObject obj;
            Block block;
            for(int i = 0; i < num; i++)
            {
                obj = m_InstantiateFunc?.Invoke(m_BlockObject);
                obj.transform.SetParent(m_BlockTransform);

                block = obj.GetComponent<Block>();
                block.transform.position = new Vector3(m_InitPos.x + m_Offset.x * i, m_InitPos.y, 0);
                block.Init(1000, EEntityType.NormalBlock, null);
                m_Blocks.Add(block);
            }
        }

        public int GetBlockCount()
        {
            return m_Blocks.Count;
        }
    }
}
