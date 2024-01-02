using System.Collections.Generic;
using UnityEngine;
using System;
using LaserCrush.Entity;
using TMPro.EditorUtilities;

namespace LaserCrush.Manager
{
    [Serializable]
    public class BlockManager
    {
        [SerializeField] private Transform m_BlockTransform;
        [SerializeField] private GameObject m_BlockObject;
        [SerializeField] private Vector2 m_InitPos;
        [SerializeField] private Vector2 m_Offset;

        private Vector3 m_MoveDownVector;

        private ItemManager m_ItemManager;
        private List<Block> m_Blocks;
        private event Func<GameObject, GameObject> m_InstantiateFunc;

        public void Init(Func<GameObject, GameObject> instantiateFunc, ItemManager itemManager)
        {
            m_Blocks = new List<Block>();
            m_InstantiateFunc = instantiateFunc;
            m_ItemManager = itemManager;

            m_MoveDownVector = new Vector3(0, m_Offset.y, 0);
        }

        public void GenerateBlock(int num)
        {
            GameObject obj;
            Block block;
            for (int i = 0; i < num; i++)
            {
                obj = m_InstantiateFunc?.Invoke(m_BlockObject);
                obj.transform.SetParent(m_BlockTransform);

                block = obj.GetComponent<Block>();
                block.transform.position = new Vector3(m_InitPos.x + m_Offset.x * i, m_InitPos.y, 0);
                block.Init(1000, GenerateEntityType(), GenerateItemType(), RemoveBlock);
                m_Blocks.Add(block);
            }
        }

        private void RemoveBlock(Block block)
        {
            //m_ItemManager.AddDroppedItem(block.DroppedItem);
            m_Blocks.Remove(block);
        }

        public void MoveDownAllBlocks()
        {
            for (int i = 0; i < m_Blocks.Count; i++)
            {
                m_Blocks[i].transform.position += m_MoveDownVector;
            }
        }

        /// <summary>
        /// 확률 표
        /// None = 50
        /// Energy = 30
        /// Prism_1 = 10
        /// Prism_2 = 10
        /// </summary>
        /// <returns></returns>
        private EItemType GenerateItemType()
        {
            //0~99사이 숫자 생성
            int prob = UnityEngine.Random.Range(0, 100);

            if (prob < 50)
            {
                return EItemType.None;
            }
            else if (prob < 80)
            {
                return EItemType.Energy;
            }
            else if (prob < 90)
            {
                return EItemType.Prism_1;
            }
            else
            {
                return EItemType.Prism_2;
            }
        }

        /// <summary>
        /// 확률 표
        /// 일반 블럭 = 50
        /// 반사 블럭 = 50
        /// </summary>
        /// <returns></returns>
        private EEntityType GenerateEntityType()
        {
            int prob = UnityEngine.Random.Range(0, 100);

            if (prob < 50)
            {
                return EEntityType.NormalBlock;
            }
            else
            {
                return EEntityType.ReflectBlock;
            }
        }
    }
}
