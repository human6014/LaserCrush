using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Laser.Manager
{
    public class BlockManager : MonoBehaviour
    {
        [SerializeField] private Transform m_BlockTransform;
        [SerializeField] private Block m_BlockObject;
        [SerializeField] private Vector2 m_InitPos;
        [SerializeField] private Vector2 m_Offset;

        
        private List<Block> m_Blocks = new List<Block>();

        public void GenerateBlock(int num)
        {
            Block block;
            for(int i = 0; i < num; i++)
            {
                block = Instantiate(m_BlockObject, m_BlockTransform);
                block.transform.position = new Vector3(m_InitPos.x + m_Offset.x * i, m_InitPos.y, 0);
                block.Init(1000, Entity.EntityType.NormalBlock, null);
                m_Blocks.Add(block);
            }
        }

        public int GetBlockCount()
        {
            return m_Blocks.Count;
        }
    }
}
